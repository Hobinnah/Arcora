using System.Security.Claims;
using System.Reflection;
using Arcora.Api;
using Arcora.Api.Configurations;
using Arcora.Api.DTOs;
using Arcora.Api.Entities;
using Arcora.Api.Exceptions;
using Arcora.Api.Services.Implementations;
using Arcora.Api.Services.Interfaces;
using Arcora.Api.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Arcora.Api.Tests;

public sealed class LeaseCheckOutTests : IAsyncLifetime
{
    private readonly SqliteConnection connection = new("Data Source=:memory:;Foreign Keys=False");
    private ArcoraDbContext db = null!;
    private LeaseCheckInService service = null!;
    private readonly Guid leaseID = Guid.NewGuid();
    private readonly Guid organizationID = Guid.NewGuid();

    public async Task InitializeAsync()
    {
        await connection.OpenAsync();
        var configuration = new ConfigurationBuilder().AddInMemoryCollection().Build();
        db = new TestContext(new DbContextOptionsBuilder<ArcoraDbContext>().UseSqlite(connection).Options, configuration);
        await db.Database.EnsureCreatedAsync();
        var tenant = new Tenant { TenantID = Guid.NewGuid(), UserID = 42, Code = "tenant", Description = "Tenant" };
        var rentalUnitID = Guid.NewGuid();
        db.Tenants.Add(tenant);
        db.RentalUnits.Add(new RentalUnit { RentalUnitID = rentalUnitID, PropertyID = Guid.NewGuid(), Status = "ACTIVE" });
        db.OrganizationMembers.Add(new OrganizationMember
        {
            OrganizationMemberID = Guid.NewGuid(), OrganizationID = organizationID, UserID = 7,
            RoleName = "OWNER", Status = "ACTIVE", IsPrimaryOwner = true
        });
        db.Leases.Add(new Lease
        {
            LeaseID = leaseID,
            OrganizationID = organizationID,
            ListingID = Guid.NewGuid(),
            RentalUnitID = rentalUnitID,
            TenancyTypeID = 1,
            TenantID = tenant.TenantID,
            LeaseCode = "TEST-LEASE",
            LeaseNumber = "TEST-001",
            Status = "ACTIVE",
            StartDate = DateTime.UtcNow.Date,
            BaseRentAmount = 1000m,
            Currency = "CAD",
            GracePeriodDays = 5,
            AutoRenew = false
        });
        await db.SaveChangesAsync();

        service = new LeaseCheckInService(
            db,
            null!,
            null!,
            Options.Create(new BlobStorageConfiguration()),
            NullLogger<LeaseCheckInService>.Instance);
    }

    public async Task DisposeAsync()
    {
        await db.DisposeAsync();
        await connection.DisposeAsync();
    }

    [Fact]
    public async Task CheckInIsSavedBeforeMessagingClearsSharedTrackingAndRetryDoesNotDuplicate()
    {
        var unit = await db.RentalUnits.SingleAsync();
        db.Properties.Add(new Property
        {
            PropertyID = unit.PropertyID, OrganizationID = organizationID, AddressID = Guid.NewGuid(),
            Name = "Synthetic property", PropertyType = "APARTMENT", TimeZone = "America/Edmonton"
        });
        await db.SaveChangesAsync();
        var messaging = DispatchProxy.Create<IMessagingService, MessagingProxy>();
        var proxy = (MessagingProxy)(object)messaging;
        proxy.Handler = (method, _) =>
        {
            if (method.Name == nameof(IMessagingService.GetOrCreateDirectThreadAsync))
            {
                db.ChangeTracker.Clear();
                return Task.FromResult(new ConversationDto { ConversationID = Guid.NewGuid() });
            }
            Assert.Equal(nameof(IMessagingService.SendMessageAsync), method.Name);
            return Task.FromResult(new ConversationMessageDto());
        };
        service = new LeaseCheckInService(db, null!, messaging, Options.Create(new BlobStorageConfiguration()),
            NullLogger<LeaseCheckInService>.Instance);
        var user = CreateUser(42);
        var draft = (await service.CreateOrGetDraftAsync(leaseID, user)).Draft;
        var request = new SubmitLeaseCheckInRequestDto
        {
            InspectionID = draft.InspectionID, Version = draft.Version,
            ArrivalConfirmed = true, OccupancyReadiness = "NEEDS_ATTENTION", Notes = "Synthetic wall issue."
        };
        var first = await service.SubmitAsync(leaseID, request, "move-in-key", user);
        var replay = await service.SubmitAsync(leaseID, request, "move-in-key", user);
        db.ChangeTracker.Clear();
        var inspection = await db.Inspections.SingleAsync();
        Assert.Equal("SUBMITTED", inspection.Status);
        Assert.Equal("NEEDS_ATTENTION", inspection.OccupancyReadiness);
        Assert.NotNull(inspection.SubmittedAt);
        Assert.NotNull((await db.Leases.SingleAsync()).ActualMoveInAt);
        Assert.Equal(first.InspectionID, replay.InspectionID);
        Assert.Single(await db.Notifications.Where(n => n.TemplateCode == "MOVE_IN_CHECKIN_SUBMITTED").ToListAsync());
    }

    public class MessagingProxy : DispatchProxy
    {
        public Func<MethodInfo, object?[], object?> Handler { get; set; } = null!;
        protected override object? Invoke(MethodInfo? method, object?[]? args) => Handler(method!, args!);
    }

    [Fact]
    public async Task CheckOutIsPersistedAndSameRequestRetryReturnsTheSavedRecord()
    {
        var user = CreateUser(42);
        var request = new SubmitLeaseCheckOutRequestDto { Confirmed = true, Notes = "Keys returned." };

        var first = await service.SubmitCheckOutAsync(leaseID, request, "checkout-key", user);
        var replay = await service.SubmitCheckOutAsync(leaseID, request, "checkout-key", user);

        Assert.True(first.Created);
        Assert.False(replay.Created);
        Assert.Equal(first.Inspection.InspectionID, replay.Inspection.InspectionID);
        Assert.NotNull(replay.Inspection.SubmittedAt);
        Assert.NotNull((await db.Leases.SingleAsync()).ActualMoveOutAt);
        Assert.Single(await db.Inspections.Where(x => x.InspectionType == "TENANT_MOVE_OUT").ToListAsync());
        Assert.Single(await db.Notifications.Where(x => x.TemplateCode == "TENANT_MOVE_OUT_SUBMITTED").ToListAsync());
    }

    [Fact]
    public async Task CheckOutReadbackPreservesEvidenceButDoesNotTerminateLeaseOrDisposeDeposit()
    {
        var depositID = Guid.NewGuid();
        db.SecurityDeposits.Add(new SecurityDeposit
        {
            SecurityDepositID = depositID, LeaseID = leaseID, OrganizationID = organizationID,
            TenantID = (await db.Tenants.SingleAsync()).TenantID,
            ReceivedAmount = 1000m, RequiredAmount = 1000m, Currency = "CAD", Status = "HELD"
        });
        await db.SaveChangesAsync();
        var user = CreateUser(42);
        Assert.Null((await service.GetCheckOutAsync(leaseID, user)).Inspection);
        await service.SubmitCheckOutAsync(leaseID,
            new SubmitLeaseCheckOutRequestDto { Confirmed = true, Notes = "  Two keys returned; unit empty.  " },
            "evidence-key", user);
        db.ChangeTracker.Clear();
        var saved = await service.GetCheckOutAsync(leaseID, user);
        Assert.NotNull(saved.Inspection);
        Assert.Equal("Two keys returned; unit empty.", saved.Inspection.Notes);
        Assert.NotNull(saved.Inspection.SubmittedAt);
        Assert.Equal("ACTIVE", (await db.Leases.SingleAsync()).Status);
        var deposit = await db.SecurityDeposits.SingleAsync();
        Assert.Equal("HELD", deposit.Status);
        Assert.Equal(0m, deposit.ReturnedAmount);
        Assert.Empty(await db.Refunds.ToListAsync());
    }

    [Theory]
    [InlineData(false, "valid-key", 0)]
    [InlineData(true, "", 0)]
    [InlineData(true, "valid-key", 1001)]
    public async Task InvalidCheckOutDoesNotPersistEvidenceOrMoveOutDate(bool confirmed, string key, int notesLength)
    {
        var invalid = await Assert.ThrowsAsync<LeaseCheckInException>(() =>
            service.SubmitCheckOutAsync(leaseID,
                new SubmitLeaseCheckOutRequestDto { Confirmed = confirmed, Notes = new string('x', notesLength) },
                key, CreateUser(42)));
        Assert.Equal(400, invalid.StatusCode);
        Assert.Null((await db.Leases.SingleAsync()).ActualMoveOutAt);
        Assert.Empty(await db.Inspections.ToListAsync());
        Assert.Empty(await db.Notifications.ToListAsync());
    }

    [Fact]
    public async Task CheckOutRejectsDifferentPayloadForTheSameKeyAndOtherTenants()
    {
        var request = new SubmitLeaseCheckOutRequestDto { Confirmed = true, Notes = "Keys returned." };
        await service.SubmitCheckOutAsync(leaseID, request, "checkout-key", CreateUser(42));

        var conflict = await Assert.ThrowsAsync<LeaseCheckInException>(() =>
            service.SubmitCheckOutAsync(leaseID, new SubmitLeaseCheckOutRequestDto { Confirmed = true, Notes = "Different" }, "checkout-key", CreateUser(42)));
        Assert.Equal(409, conflict.StatusCode);

        var forbidden = await Assert.ThrowsAsync<LeaseCheckInException>(() =>
            service.GetCheckOutAsync(leaseID, CreateUser(99)));
        Assert.Equal(404, forbidden.StatusCode);
        Assert.Single(await db.Inspections.Where(x => x.InspectionType == "TENANT_MOVE_OUT").ToListAsync());
    }

    private static ClaimsPrincipal CreateUser(long userID) => new(new ClaimsIdentity(
        [new Claim("UserId", userID.ToString()), new Claim(ClaimTypes.NameIdentifier, userID.ToString()), new Claim(ClaimTypes.Role, "User")], "test"));

    private sealed class TestContext(DbContextOptions<ArcoraDbContext> options, IConfiguration configuration)
        : ArcoraDbContext(options, configuration)
    {
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            foreach (var property in builder.Model.GetEntityTypes().SelectMany(entity => entity.GetProperties()))
                property.SetColumnType(null);
        }
    }
}