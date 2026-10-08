using System.Reflection;
using System.Text.Json;
using Arcora.Api.Configurations;
using Arcora.Api.DTOs;
using Arcora.Api.DTOs.DtoProfiles;
using Arcora.Api.Email;
using Arcora.Api.Entities;
using Arcora.Api.Exceptions;
using Arcora.Api.Repositories.Interfaces;
using Arcora.Api.Services.Implementations;
using Arcora.Api.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Arcora.Api.Tests;

public sealed class TenantInvitationQuoteTests : IAsyncLifetime
{
    private readonly SqliteConnection connection = new("Data Source=:memory:;Foreign Keys=False");
    private readonly IDataProtectionProvider protection = new EphemeralDataProtectionProvider();
    private ArcoraDbContext db = null!;
    private TenantInvitationService service = null!;
    private IMapper mapper = null!;
    private Listing listing = null!;
    private Tenant tenant = null!;
    private readonly DateTime start = new(2027, 1, 31);

    public async Task InitializeAsync()
    {
        await connection.OpenAsync();
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["FrontendUrl"] = "https://arcora.example.test",
            ["JwtSettings:Key"] = "test-only-signing-key"
        }).Build();
        db = new TestContext(new DbContextOptionsBuilder<ArcoraDbContext>().UseSqlite(connection).Options, config);
        await db.Database.EnsureCreatedAsync();
        mapper = new MapperConfiguration(c =>
        {
            c.AddMaps(typeof(TenantInvitationProfile).Assembly);
        }, NullLoggerFactory.Instance).CreateMapper();
        var listingService = Proxy<IListingService>(method => method.Name == nameof(IListingService.GetListingAvailability)
            ? Task.FromResult<ListingAvailabilityResponseDto?>(new() { IsAvailable = true })
            : throw new InvalidOperationException($"Unexpected listing call: {method.Name}"));
        var payments = Proxy<IPaymentOnboardingService>(method => method.Name == nameof(IPaymentOnboardingService.GetStatusAsync)
            ? Task.FromResult(new PaymentOnboardingStatusResponse { HasVerifiedCard = true, HasVerifiedPad = true, PadMandateActive = true })
            : throw new InvalidOperationException($"Unexpected payment call: {method.Name}"));
        var preference = Proxy<IPreferenceService>(_ => Task.FromResult<Preference?>(null));
        service = new TenantInvitationService(mapper, new MemoryCache(new MemoryCacheOptions()),
            Options.Create(new CacheConfiguration()), NullLogger<TenantInvitationService>.Instance,
            null!, db, listingService, null!, preference, config, payments, protection);

        var org = Guid.NewGuid();
        listing = new Listing
        {
            ListingID = Guid.NewGuid(), OrganizationID = org, Title = "Quoted home", Notes = "",
            Currency = "CAD", BaseMonthlyRentAmount = 2000.25m, SecurityDepositAmount = 900.50m
        };
        tenant = new Tenant { TenantID = Guid.NewGuid(), UserID = 9, Code = "tenant", Description = "Tenant" };
        db.Listings.Add(listing);
        db.Tenants.Add(tenant);
        db.Users.Add(new User { Id = 9, Email = "tenant@example.test", UserName = "tenant@example.test" });
        db.OrganizationMembers.Add(new OrganizationMember
        {
            OrganizationMemberID = Guid.NewGuid(), OrganizationID = org, UserID = 7,
            IsPrimaryOwner = true, Status = "ACTIVE", RoleName = "OWNER"
        });
        db.TenancyTypes.Add(new TenancyType { Code = "SHORT", Name = "Short term", IsActive = true });
        await db.SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        await db.DisposeAsync();
        await connection.DisposeAsync();
    }

    private async Task<TenantInvitation> CreateAsync()
    {
        var result = await service.CreateTenantInvitationWithHold(new CreateTenantInvitationRequestDto
        {
            OrganizationID = listing.OrganizationID, ListingID = listing.ListingID,
            StartDate = start, LeaseTermMonths = 6, Email = "tenant@example.test",
            InvitationPurpose = "Direct lease invitation"
        }, 7);
        Assert.True(result.Success);
        return await db.TenantInvitations.SingleAsync();
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ListingEditsCannotChangeEmailLeaseDepositOrReturnedQuote(bool useTermPrice)
    {
        if (useTermPrice)
        {
            db.ListingTermPrices.Add(new ListingTermPrice
            {
                ListingTermPriceID = Guid.NewGuid(), ListingID = listing.ListingID,
                LeaseTermMonths = 6, MonthlyRentAmount = 1750.75m,
                SecurityDepositAmount = 650.25m, IsActive = true
            });
            await db.SaveChangesAsync();
        }
        var invitation = await CreateAsync();
        var rent = useTermPrice ? 1750.75m : 2000.25m;
        var deposit = useTermPrice ? 650.25m : 900.50m;
        Assert.Equal(rent, invitation.MonthlyRentAmount);
        Assert.Equal(deposit, invitation.SecurityDepositAmount);
        Assert.Equal(start, invitation.StartDate);
        Assert.Equal(start.AddMonths(6), invitation.EndDate);
        var email = await db.TenantInvitationEmails.SingleAsync();
        var message = JsonSerializer.Deserialize<EmailMessage>(
            protection.CreateProtector(TenantInvitationEmailWorker.ProtectionPurpose).Unprotect(email.ProtectedMessage))!;
        Assert.Contains(rent.ToString("N2", System.Globalization.CultureInfo.InvariantCulture), message.HtmlBody);
        Assert.Contains(deposit.ToString("N2", System.Globalization.CultureInfo.InvariantCulture), message.HtmlBody);

        listing.BaseMonthlyRentAmount = 9999;
        listing.SecurityDepositAmount = 8888;
        listing.Currency = "USD";
        foreach (var price in await db.ListingTermPrices.ToListAsync())
        {
            price.MonthlyRentAmount = 7777;
            price.SecurityDepositAmount = 6666;
            price.IsActive = false;
        }
        invitation.Status = "ACCEPTED";
        await db.SaveChangesAsync();
        Assert.Equal(rent, (await service.GetPendingInvitationsForTenantAsync(tenant.TenantID)).Single().Price);
        var result = await service.CreateLeaseFromTenantInvitationAsync(invitation.TenantInvitationID,
            tenant.TenantID, 9, "tenant@example.test", "9");
        Assert.NotNull(result);
        var lease = await db.Leases.SingleAsync();
        Assert.Equal(rent, lease.BaseRentAmount);
        Assert.Equal("CAD", lease.Currency);
        Assert.Equal(start, lease.StartDate);
        Assert.Equal(start.AddMonths(6), lease.EndDate);
        Assert.Equal((short)6, lease.LeaseTermMonths);
        var ledger = await db.SecurityDeposits.SingleAsync();
        Assert.Equal(deposit, ledger.RequiredAmount);
        Assert.Equal("CAD", ledger.Currency);
        Assert.Equal(lease.LeaseID, ledger.LeaseID);
        var dto = await service.GetAcceptedInvitationForUserAsync(invitation.TenantInvitationID, 9);
        Assert.Equal(rent, dto.Price);
        Assert.Equal(deposit, dto.SecurityDepositAmount);
        Assert.Null(dto.TokenHash);
        Assert.Equal(rent, (await service.GetByOrganizationAsync(listing.OrganizationID)).Single().Price);
        var leaseRepository = Proxy<ILeaseRepository>(_ => Task.FromResult<Lease?>(lease));
        var templateRepository = Proxy<ILeaseContractTemplateRepository>(_ => Task.FromResult<LeaseContractTemplate?>(new()
        {
            HtmlContent = "{{Listing.BaseMonthlyRentAmount}}|{{Listing.SecurityDepositAmount}}|{{Listing.Currency}}|{{Lease.BaseRentAmount}}"
        }));
        var contractService = new LeaseContractTemplateService(mapper, new MemoryCache(new MemoryCacheOptions()),
            Options.Create(new CacheConfiguration()), NullLogger<LeaseContractTemplateService>.Instance,
            templateRepository, leaseRepository, null!, db);
        var contract = await contractService.RenderContract(new LeaseContractRenderRequestDto
        {
            LeaseID = lease.LeaseID, LeaseContractTemplateID = Guid.NewGuid()
        });
        Assert.NotNull(contract);
        Assert.Contains($"{rent:N2}|{deposit:N2}|CAD|{rent:N2}", contract.RenderedHtml);
        Assert.True((await service.CreateLeaseFromTenantInvitationAsync(invitation.TenantInvitationID,
            tenant.TenantID, 9, "tenant@example.test", "9"))!.AlreadyExists);
        Assert.Single(await db.SecurityDeposits.ToListAsync());
    }

    [Fact]
    public async Task StoredQuoteCannotBeEditedThroughEfOrDtoMapping()
    {
        var invitation = await CreateAsync();
        var dto = mapper.Map<TenantInvitationDto>(invitation);
        dto.Price = 1;
        dto.SecurityDepositAmount = 1;
        dto.Currency = "USD";
        dto.StartDate = start.AddDays(1);
        mapper.Map(dto, invitation);
        Assert.Equal(2000.25m, invitation.MonthlyRentAmount);
        Assert.Equal(900.50m, invitation.SecurityDepositAmount);
        Assert.Equal("CAD", invitation.Currency);
        Assert.Equal(start, invitation.StartDate);
        invitation.MonthlyRentAmount = 1;
        await Assert.ThrowsAsync<InvalidOperationException>(() => db.SaveChangesAsync());
    }

    [Fact]
    public async Task QuoteEditsAreExplicitlyRejectedByTheUpdateEndpointService()
    {
        var invitation = await CreateAsync();
        var dto = mapper.Map<TenantInvitationDto>(invitation);
        dto.Price = 1;
        var error = await Assert.ThrowsAsync<ApiProblemException>(() =>
            service.UpdateTenantInvitation(invitation.TenantInvitationID, dto));
        Assert.Equal(400, error.StatusCode);
        Assert.Equal(2000.25m, (await db.TenantInvitations.AsNoTracking().SingleAsync()).MonthlyRentAmount);
    }

    [Fact]
    public async Task ZeroDepositIsPreservedEvenIfListingLaterRequiresADeposit()
    {
        listing.SecurityDepositAmount = 0;
        await db.SaveChangesAsync();
        var invitation = await CreateAsync();
        listing.SecurityDepositAmount = 500;
        invitation.Status = "ACCEPTED";
        await db.SaveChangesAsync();
        await service.CreateLeaseFromTenantInvitationAsync(invitation.TenantInvitationID,
            tenant.TenantID, 9, "tenant@example.test", "9");
        Assert.Equal(0m, (await db.SecurityDeposits.SingleAsync()).RequiredAmount);
    }

    [Fact]
    public async Task ChangedHoldDatesCannotReplaceTheSavedTerm()
    {
        var invitation = await CreateAsync();
        invitation.Status = "ACCEPTED";
        (await db.ReservationHolds.SingleAsync()).EndDate = start.AddMonths(7);
        await db.SaveChangesAsync();
        var error = await Assert.ThrowsAsync<ApiProblemException>(() =>
            service.CreateLeaseFromTenantInvitationAsync(invitation.TenantInvitationID,
                tenant.TenantID, 9, "tenant@example.test", "9"));
        Assert.Equal(409, error.StatusCode);
        Assert.Empty(await db.Leases.ToListAsync());
        Assert.Empty(await db.SecurityDeposits.ToListAsync());
    }

    [Theory]
    [InlineData(-1, 100, "CAD")]
    [InlineData(100, -1, "CAD")]
    [InlineData(100, 100, "12X")]
    public async Task InvalidPricingCannotCreateAnInvitationOrHold(int rent, int deposit, string currency)
    {
        listing.BaseMonthlyRentAmount = rent;
        listing.SecurityDepositAmount = deposit;
        listing.Currency = currency;
        await db.SaveChangesAsync();
        var error = await Assert.ThrowsAsync<ApiProblemException>(() => CreateAsync());
        Assert.Equal(409, error.StatusCode);
        Assert.Empty(await db.TenantInvitations.ToListAsync());
        Assert.Empty(await db.ReservationHolds.ToListAsync());
        Assert.Empty(await db.TenantInvitationEmails.ToListAsync());
    }

    [Fact]
    public async Task LegacyInvitationFailsExplicitlyInsteadOfUsingCurrentListingPrice()
    {
        var invitation = new TenantInvitation
        {
            TenantInvitationID = Guid.NewGuid(), ListingID = listing.ListingID,
            Email = "tenant@example.test", Status = "ACCEPTED", TokenHash = "legacy",
            InvitationPurpose = "Direct lease invitation", ExpiresAt = DateTime.UtcNow.AddDays(1)
        };
        db.TenantInvitations.Add(invitation);
        await db.SaveChangesAsync();
        var error = await Assert.ThrowsAsync<ApiProblemException>(() =>
            service.CreateLeaseFromTenantInvitationAsync(invitation.TenantInvitationID, tenant.TenantID, 9, "tenant@example.test", "9"));
        Assert.Equal(409, error.StatusCode);
        Assert.Contains("send a new invitation", error.Detail);
        Assert.Empty(await db.Leases.ToListAsync());
        Assert.Empty(await db.SecurityDeposits.ToListAsync());
    }

    [Fact]
    public async Task OtherAccountsCannotReadSavedInvitationQuote()
    {
        var invitation = await CreateAsync();
        invitation.Status = "ACCEPTED";
        await db.SaveChangesAsync();
        Assert.Equal(404, (await Assert.ThrowsAsync<ApiProblemException>(() =>
            service.GetAcceptedInvitationForUserAsync(invitation.TenantInvitationID, 7))).StatusCode);
    }

    private static T Proxy<T>(Func<MethodInfo, object?> handler) where T : class
    {
        var proxy = DispatchProxy.Create<T, TestProxy>();
        ((TestProxy)(object)proxy).Handler = handler;
        return proxy;
    }

    public class TestProxy : DispatchProxy
    {
        public Func<MethodInfo, object?> Handler { get; set; } = null!;
        protected override object? Invoke(MethodInfo? method, object?[]? args) => Handler(method!);
    }

    private sealed class TestContext(DbContextOptions<ArcoraDbContext> options, IConfiguration config) : ArcoraDbContext(options, config)
    {
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            foreach (var property in builder.Model.GetEntityTypes().SelectMany(entity => entity.GetProperties()))
                property.SetColumnType(null);
        }
    }
}
