using Arcora.Api;
using Arcora.Api.Configurations;
using Arcora.Api.DTOs;
using Arcora.Api.Entities;
using Arcora.Api.Services.Implementations;
using AutoMapper;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Arcora.Api.Tests;

public sealed class RentalApplicationAccessTests : IAsyncLifetime
{
    private readonly SqliteConnection connection = new("Data Source=:memory:;Foreign Keys=False");
    private ArcoraDbContext db = null!;
    private RentalApplicationService service = null!;
    private readonly Guid organizationID = Guid.NewGuid();
    private readonly Guid listingID = Guid.NewGuid();
    private readonly Guid tenantID = Guid.NewGuid();

    public async Task InitializeAsync()
    {
        await connection.OpenAsync();
        var configuration = new ConfigurationBuilder().AddInMemoryCollection().Build();
        db = new TestContext(new DbContextOptionsBuilder<ArcoraDbContext>().UseSqlite(connection).Options, configuration);
        await db.Database.EnsureCreatedAsync();
        db.Users.AddRange(
            new User { Id = 10, UserName = "tenant@example.test", Email = "tenant@example.test" },
            new User { Id = 20, UserName = "other@example.test", Email = "other@example.test" });
        db.Tenants.Add(new Tenant { TenantID = tenantID, UserID = 10, Code = "tenant-10", Description = "Tenant" });
        db.Listings.Add(new Listing
        {
            ListingID = listingID,
            OrganizationID = organizationID,
            Title = "Test listing",
            Notes = string.Empty,
            Currency = "CAD",
            BaseMonthlyRentAmount = 1500m,
            SecurityDepositAmount = 750m
        });
        await db.SaveChangesAsync();

        var mapper = new MapperConfiguration(
            configuration => configuration.CreateMap<RentalApplication, RentalApplicationDto>(MemberList.None),
            NullLoggerFactory.Instance).CreateMapper();
        service = new RentalApplicationService(
            mapper,
            new MemoryCache(new MemoryCacheOptions()),
            Options.Create(new CacheConfiguration()),
            NullLogger<RentalApplicationService>.Instance,
            null!, null!, null!, null!, null!, null!, null!, null!, null!, null!, null!, null!, null!, null!,
            configuration,
            db);
    }

    public async Task DisposeAsync()
    {
        await db.DisposeAsync();
        await connection.DisposeAsync();
    }

    [Fact]
    public async Task CreateRejectsTenantIDOwnedByAnotherUser()
    {
        var request = new RentalApplicationDto
        {
            ListingID = listingID,
            OrganizationID = organizationID,
            TenantID = tenantID,
            ApplicationCode = "client-code",
            DesiredMoveInDate = DateTime.UtcNow.Date.AddMonths(1),
            RequestedLeaseTermMonths = 6,
            Currency = "CAD",
            Status = "APPROVED"
        };

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            service.CreateRentalApplicationForActor(request, actorUserID: 20, isAdmin: false));

        Assert.Empty(await db.RentalApplications.ToListAsync());
    }

    [Fact]
    public async Task UpdateRejectsApplicationOwnedByAnotherUser()
    {
        var applicationID = Guid.NewGuid();
        db.RentalApplications.Add(new RentalApplication
        {
            RentalApplicationID = applicationID,
            ApplicationCode = "TEST-APP",
            ListingID = listingID,
            TenantID = tenantID,
            OrganizationID = organizationID,
            DesiredMoveInDate = DateTime.UtcNow.Date.AddMonths(1),
            RequestedLeaseTermMonths = 6,
            Status = "SUBMITTED",
            ScreeningStatus = "PENDING",
            Currency = "CAD"
        });
        await db.SaveChangesAsync();

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            service.UpdateRentalApplicationForActor(
                applicationID,
                new RentalApplicationDto { RentalApplicationID = applicationID, Status = "APPROVED" },
                actorUserID: 20,
                isAdmin: false));

        Assert.Equal("SUBMITTED", (await db.RentalApplications.SingleAsync()).Status);
    }

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
