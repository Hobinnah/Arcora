using System.Security.Claims;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Arcora.Api.DTOs;
using Arcora.Api.Controllers;
using Arcora.Api.Configurations;
using Arcora.Api.DTOs.DtoProfiles;
using AutoMapper;
using Arcora.Api.Entities;
using Arcora.Api.Filters;
using Arcora.Api.Helpers;
using Arcora.Api.Models;
using Arcora.Api.Repositories.Implementations;
using Arcora.Api.Services.Implementations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Arcora.Api.Tests;

public sealed class ListingMapPrivacyTests : IAsyncLifetime
{
    private readonly SqliteConnection connection = new("Data Source=:memory:;Foreign Keys=False");
    private ArcoraDbContext db = null!;
    private ListingRepository repository = null!;
    private PropertyLocationAccess access = null!;
    private readonly Guid addressID = Guid.NewGuid();
    private readonly Guid propertyID = Guid.NewGuid();
    private readonly Guid unitID = Guid.NewGuid();
    private readonly Guid organizationID = Guid.NewGuid();
    private readonly Guid tenantID = Guid.NewGuid();
    private readonly Guid unitTypeID = Guid.NewGuid();
    private readonly Guid listingID = Guid.NewGuid();
    private readonly Guid listingTypeID = Guid.NewGuid();

    public async Task InitializeAsync()
    {
        await connection.OpenAsync();
        db = new TestContext(new DbContextOptionsBuilder<ArcoraDbContext>().UseSqlite(connection).Options,
            new ConfigurationBuilder().Build());
        await db.Database.EnsureCreatedAsync();
        repository = new ListingRepository(db);
        access = new PropertyLocationAccess(db);
        db.Organizations.Add(new Organization { OrganizationID = organizationID, LegalName = "Synthetic host", DisplayName = "Synthetic host", TimeZone = "UTC" });
        db.OrganizationMembers.Add(new OrganizationMember { OrganizationMemberID = Guid.NewGuid(), OrganizationID = organizationID,
            UserID = 10, RoleName = "OWNER", Status = "ACTIVE" });
        db.Tenants.Add(new Tenant { TenantID = tenantID, UserID = 42, Code = "SYNTHETIC", Description = "Map tests" });
        db.Addresses.Add(new Address { AddressID = addressID, Line1 = "1 Synthetic Street", Line2 = "Suite 1", City = "Toronto",
            ProvinceCode = "ON", PostalCode = "M5H 1A1", CountryCode = "CA", Latitude = 43.6501m, Longitude = -79.3833m });
        db.Properties.Add(new Property { PropertyID = propertyID, OrganizationID = organizationID, AddressID = addressID,
            PropertyType = "Apartment", TimeZone = "UTC", Name = "1 Synthetic Street" });
        db.RentalUnits.Add(new RentalUnit { RentalUnitID = unitID, PropertyID = propertyID, UnitTypeID = unitTypeID, UnitNumber = "Suite 1" });
        db.ListingTypes.Add(new ListingType { ListingTypeID = listingTypeID, Name = "Monthly", CapturedBy = "Tests" });
        db.Listings.Add(Home(listingID));
        await db.SaveChangesAsync();
    }

    private Listing Home(Guid id) => new()
    {
        ListingID = id, OrganizationID = organizationID, RentalUnitID = unitID, ListingTypeID = listingTypeID,
        Title = "Synthetic home", Status = "PUBLISHED", Currency = "CAD", BaseMonthlyRentAmount = 1000,
        YearBuilt = 2000, MinimumLeaseMonths = 1, MaximumLeaseMonths = 12, Notes = "", AcceptingApplications = true
    };

    [Fact]
    public void SearchValidationAllowsNoBoundsAndDateLineButRejectsIncompleteBounds()
    {
        static List<System.ComponentModel.DataAnnotations.ValidationResult> Errors(ListingSearchCriteria criteria)
        {
            var errors = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
            System.ComponentModel.DataAnnotations.Validator.TryValidateObject(criteria,
                new System.ComponentModel.DataAnnotations.ValidationContext(criteria), errors, validateAllProperties: true);
            return errors;
        }
        Assert.Empty(Errors(new ListingSearchCriteria()));
        Assert.Empty(Errors(new ListingSearchCriteria { South = -10, North = 10, West = 170, East = -170 }));
        Assert.NotEmpty(Errors(new ListingSearchCriteria { South = -10 }));
        Assert.NotEmpty(Errors(new ListingSearchCriteria { South = 20, North = 10, West = 30, East = 40 }));
        Assert.NotEmpty(Errors(new ListingSearchCriteria { StayLengthMonths = 999 }));
    }

    [Fact]
    public async Task AllMapPinsAppearBeyondTheCardPaginationCap()
    {
        for (var i = 0; i < 60; i++) db.Listings.Add(Home(Guid.NewGuid()));
        var missingAddressID = Guid.NewGuid();
        var missingPropertyID = Guid.NewGuid();
        var missingUnitID = Guid.NewGuid();
        db.Addresses.Add(new Address { AddressID = missingAddressID, Line1 = "2 Synthetic Street", City = "Toronto", CountryCode = "CA" });
        db.Properties.Add(new Property { PropertyID = missingPropertyID, OrganizationID = organizationID, AddressID = missingAddressID, PropertyType = "Apartment", TimeZone = "UTC" });
        db.RentalUnits.Add(new RentalUnit { RentalUnitID = missingUnitID, PropertyID = missingPropertyID });
        db.Listings.Add(new Listing { ListingID = Guid.NewGuid(), OrganizationID = organizationID, RentalUnitID = missingUnitID,
            ListingTypeID = listingTypeID, Title = "No location yet", Status = "APPROVED", Currency = "CAD",
            MinimumLeaseMonths = 1, MaximumLeaseMonths = 12, Notes = "" });
        await db.SaveChangesAsync();
        var criteria = new ListingSearchCriteria { PageSize = 10, PageNumber = 2 };
        var map = await repository.SearchMapAsync(criteria, default);
        var page = await repository.SearchListingsAsync(criteria);
        Assert.Equal(62, map.TotalCount);
        Assert.Equal(61, map.Data.Count);
        Assert.Equal(1, map.MissingLocationCount);
        Assert.Equal(10, page.Items.Count);
        Assert.Equal(map.TotalCount, page.TotalCount);
        Assert.Equal(61, map.Data.Select(pin => pin.ListingID).Distinct().Count());
        Assert.All(map.Data, pin => { Assert.Equal(43.655m, pin.Latitude); Assert.Equal(-79.385m, pin.Longitude); });
    }

    [Fact]
    public async Task BoundsUsePublicCellsAndCannotProbePrivateCoordinates()
    {
        var aroundPublic = new ListingSearchCriteria { South = 43.654, North = 43.656, West = -79.386, East = -79.384 };
        var map = await repository.SearchMapAsync(aroundPublic, default);
        Assert.Single(map.Data);
        Assert.Equal(map.TotalCount, (await repository.SearchListingsAsync(aroundPublic)).TotalCount);
        var aroundPrivate = new ListingSearchCriteria { South = 43.650, North = 43.651, West = -79.384, East = -79.382 };
        Assert.Empty((await repository.SearchMapAsync(aroundPrivate, default)).Data);
        Assert.Equal(0, (await repository.SearchListingsAsync(aroundPrivate)).TotalCount);
    }

    [Fact]
    public async Task UnitTypeAndLocationFiltersAreSharedAndStreetCannotBeProbed()
    {
        Assert.Single((await repository.SearchMapAsync(new ListingSearchCriteria { UnitTypeID = unitTypeID, Where = "Toronto" }, default)).Data);
        Assert.Empty((await repository.SearchMapAsync(new ListingSearchCriteria { UnitTypeID = Guid.NewGuid() }, default)).Data);
        Assert.Empty((await repository.SearchMapAsync(new ListingSearchCriteria { Where = "1 Synthetic Street" }, default)).Data);
    }

    [Theory]
    [InlineData("LEASE")]
    [InlineData("HOLD")]
    [InlineData("CALENDAR")]
    public async Task RealAvailabilityBlocksCardsAndMapTogether(string source)
    {
        var start = new DateTime(2050, 1, 1);
        if (source == "LEASE") db.Leases.Add(Lease("ACTIVE", start));
        if (source == "HOLD") db.ReservationHolds.Add(new ReservationHold { ReservationHoldID = Guid.NewGuid(), ListingID = listingID,
            StartDate = start, EndDate = start.AddMonths(3), ExpiresAt = DateTime.UtcNow.AddDays(1), Status = "ACTIVE" });
        if (source == "CALENDAR") db.CalendarEvents.Add(new CalendarEvent { CalendarEventID = Guid.NewGuid(), ListingID = listingID,
            EventType = "MAINTENANCE", StartAt = start, EndAt = start.AddMonths(3), BlocksAvailability = true, Status = "CONFIRMED" });
        await db.SaveChangesAsync();
        var criteria = new ListingSearchCriteria { MoveInDate = start, StayLengthMonths = 1 };
        Assert.Empty((await repository.SearchMapAsync(criteria, default)).Data);
        Assert.Equal(0, (await repository.SearchListingsAsync(criteria)).TotalCount);
        criteria.MoveInDate = start.AddMonths(3);
        Assert.Single((await repository.SearchMapAsync(criteria, default)).Data);
    }

    [Fact]
    public async Task CancelledCalendarEventDoesNotBlockAvailability()
    {
        var start = new DateTime(2050, 1, 1);
        db.CalendarEvents.Add(new CalendarEvent { CalendarEventID = Guid.NewGuid(), ListingID = listingID,
            EventType = "MAINTENANCE", StartAt = start, EndAt = start.AddMonths(3), BlocksAvailability = true, Status = "CANCELLED" });
        await db.SaveChangesAsync();
        Assert.Single((await repository.SearchMapAsync(new ListingSearchCriteria { MoveInDate = start, StayLengthMonths = 1 }, default)).Data);
    }

    [Theory]
    [InlineData("ACTIVE", true, true, 42, true)]
    [InlineData("ACTIVE", false, true, 42, false)]
    [InlineData("ACTIVE", true, false, 42, false)]
    [InlineData("PENDING_FIRST_PAYMENT", true, true, 42, false)]
    [InlineData("ENDED", true, true, 42, false)]
    [InlineData("ACTIVE", true, true, 99, false)]
    [InlineData("DRAFT", false, true, 10, true)]
    public async Task ExactAddressRequiresOwnActiveLeaseAndRealCompletedInitialPayment(
        string status, bool completedPayment, bool firstPayment, long userID, bool expected)
    {
        var lease = Lease(status, new DateTime(2050, 1, 1));
        db.Leases.Add(lease);
        var intentID = Guid.NewGuid();
        db.PaymentIntents.Add(new PaymentIntent { PaymentIntentID = intentID, LeaseID = lease.LeaseID, TenantID = tenantID,
            PaymentMethodID = Guid.NewGuid(), Amount = 1000, Status = "PAID", ProviderName = "STRIPE",
            IdempotencyKey = firstPayment ? $"pi_firstcharge_lease_{lease.LeaseID:N}" : "pi_recurring_synthetic", ScheduledChargeAt = DateTime.UtcNow });
        if (completedPayment) db.Payments.Add(new Payment { PaymentID = Guid.NewGuid(), PaymentIntentID = intentID, TenantID = tenantID,
            ProviderChargeID = "py_synthetic", GrossAmount = 1000, NetAmount = 1000, PaidAt = DateTime.UtcNow, Status = "PAID" });
        await db.SaveChangesAsync();
        var allowed = await access.AllowedAddressesAsync(User(userID), [addressID], default);
        Assert.Equal(expected, allowed.Contains(addressID));
        if (userID == 42)
            Assert.Equal(expected, await access.IsActiveAndPaidAsync(lease, default));
    }

    [Fact]
    public async Task SerializationBoundaryRedactsDeepGraphsWithoutMutatingCachedEntities()
    {
        var unit = await db.RentalUnits.Include(unit => unit.Property)!.ThenInclude(property => property!.Address).SingleAsync();
        var cachedAddress = new AddressDto { AddressID = addressID, Line1 = unit.Property!.Address!.Line1,
            City = "Toronto", PostalCode = "M5H 1A1", Latitude = 43.6501m, Longitude = -79.3833m };
        var dto = new ListingDto { ListingID = listingID, RentalUnitID = unitID,
            RentalUnit = new RentalUnitDto { RentalUnitID = unitID, PropertyID = propertyID, UnitNumber = "Suite 1",
                Property = new PropertyDto { PropertyID = propertyID, AddressID = addressID, OrganizationID = organizationID,
                    PropertyType = "Apartment", Address = cachedAddress } }, CheckInDoorCode = "SECRET" };
        var jsonOptions = new JsonOptions();
        jsonOptions.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        var filter = new PropertyLocationPrivacyFilter(access, Options.Create(jsonOptions));
        var http = new DefaultHttpContext();
        var action = new ActionContext(http, new RouteData(), new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor());
        var result = new OkObjectResult(dto);
        var executing = new ResultExecutingContext(action, new List<IFilterMetadata>(), result, new object());
        await filter.OnResultExecutionAsync(executing, () => Task.FromResult(new ResultExecutedContext(action, new List<IFilterMetadata>(), result, new object())));
        var json = Assert.IsType<JsonObject>(result.Value);
        var address = json["rentalUnit"]!["property"]!["address"]!;
        Assert.Null(address["line1"]);
        Assert.Null(address["postalCode"]);
        Assert.Null(address["latitude"]);
        Assert.Null(json["checkInDoorCode"]);
        Assert.Null(json["rentalUnit"]!["unitNumber"]);
        Assert.Equal("Toronto", address["city"]!.ToString());
        Assert.Equal(43.655m, json["publicLocation"]!["latitude"]!.GetValue<decimal>());
        Assert.Equal("1 Synthetic Street", unit.Property!.Address!.Line1);
        Assert.Equal(43.6501m, unit.Property.Address.Latitude);
        Assert.Equal("1 Synthetic Street", cachedAddress.Line1);
    }

    [Theory]
    [InlineData(90, 180)]
    [InlineData(-90, -180)]
    [InlineData(0, 0)]
    public void PublicCellsRemainInValidWorldBounds(decimal latitude, decimal longitude)
    {
        var location = PublicMapLocation.From(latitude, longitude)!;
        Assert.InRange(location.Latitude, -90, 90);
        Assert.InRange(location.Longitude, -180, 180);
        Assert.Equal(location, PublicMapLocation.From(latitude, longitude));
        Assert.Null(PublicMapLocation.From(null, longitude));
        Assert.Null(PublicMapLocation.From(91, longitude));
    }

    [Fact]
    public async Task PaidTenantCannotReadAnotherUnitsDetailsInTheSameBuilding()
    {
        var lease = Lease("ACTIVE", new DateTime(2050, 1, 1));
        db.Leases.Add(lease);
        var intentID = Guid.NewGuid();
        db.PaymentIntents.Add(new PaymentIntent { PaymentIntentID = intentID, LeaseID = lease.LeaseID, TenantID = tenantID,
            PaymentMethodID = Guid.NewGuid(), Amount = 1000, Status = "PAID", ProviderName = "STRIPE",
            IdempotencyKey = $"pi_firstcharge_lease_{lease.LeaseID:N}", ScheduledChargeAt = DateTime.UtcNow });
        db.Payments.Add(new Payment { PaymentID = Guid.NewGuid(), PaymentIntentID = intentID, TenantID = tenantID,
            GrossAmount = 1000, NetAmount = 1000, PaidAt = DateTime.UtcNow, Status = "PAID", ProviderChargeID = "ch_synthetic" });
        var otherUnitID = Guid.NewGuid();
        db.RentalUnits.Add(new RentalUnit { RentalUnitID = otherUnitID, PropertyID = propertyID, UnitNumber = "Suite 2" });
        await db.SaveChangesAsync();
        var resources = await access.ResourceAddressesAsync([listingID], [propertyID], [unitID, otherUnitID], default);
        var rights = await access.PermissionsAsync(User(42), [addressID], resources, default);
        Assert.Contains(addressID, rights.Addresses);
        Assert.Contains(unitID, rights.Resources);
        Assert.DoesNotContain(otherUnitID, rights.Resources);
        var json = JsonSerializer.SerializeToNode(new
        {
            rentalUnitID = otherUnitID, unitNumber = "Suite 2",
            property = new { propertyID, propertyType = "Apartment", address = new { addressID, line1 = "1 Synthetic Street", latitude = 43.6501m, longitude = -79.3833m } }
        })!;
        PropertyLocationPrivacyFilter.Redact(json, rights.Addresses, rights.Resources);
        PropertyLocationPrivacyFilter.RedactResources(json, rights.Resources);
        Assert.Null(json["unitNumber"]);
        Assert.Null(json["property"]!["address"]!["line1"]);
    }

    [Fact]
    public async Task DeactivatedHostCannotManageOrReadExactAddress()
    {
        var member = await db.OrganizationMembers.SingleAsync();
        member.DeactivatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        Assert.Empty(await access.AllowedAddressesAsync(User(10), [addressID], default));
        Assert.False(await access.CanManageAddressAsync(User(10), addressID, default));
        Assert.False(await access.HasHostMembershipAsync(User(10), default));
    }

    [Theory]
    [InlineData(typeof(PaymentController), nameof(PaymentController.CreatePayment))]
    [InlineData(typeof(PaymentController), nameof(PaymentController.UpdatePayment))]
    [InlineData(typeof(PaymentController), nameof(PaymentController.UpdatePaymentStatus))]
    [InlineData(typeof(PaymentIntentController), nameof(PaymentIntentController.CreatePaymentIntent))]
    [InlineData(typeof(PaymentIntentController), nameof(PaymentIntentController.UpdatePaymentIntent))]
    [InlineData(typeof(PaymentIntentController), nameof(PaymentIntentController.UpdatePaymentIntentStatus))]
    [InlineData(typeof(LeaseController), nameof(LeaseController.UpdateLeaseStatus))]
    public void UntrustedUsersCannotForgePaidOrActiveFlags(Type controller, string method)
    {
        var authorization = controller.GetMethod(method)!.GetCustomAttribute<AuthorizeAttribute>();
        Assert.NotNull(authorization);
        Assert.Equal("Admin", authorization.Roles);
    }

    [Theory]
    [InlineData(1000, 1000, false, false)]
    [InlineData(999, 0, false, false)]
    [InlineData(1000, 0, true, false)]
    [InlineData(1000, 0, false, true)]
    public async Task CompletedPaymentMustCoverInitialAmountAndBelongToTenant(
        decimal gross, decimal refunded, bool wrongTenant, bool expected)
    {
        var lease = Lease("ACTIVE", new DateTime(2050, 1, 1));
        db.Leases.Add(lease);
        var intentID = Guid.NewGuid();
        db.PaymentIntents.Add(new PaymentIntent { PaymentIntentID = intentID, LeaseID = lease.LeaseID, TenantID = tenantID,
            PaymentMethodID = Guid.NewGuid(), Amount = 1000, Status = "PAID", ProviderName = "STRIPE",
            IdempotencyKey = $"pi_firstcharge_lease_{lease.LeaseID:N}", ScheduledChargeAt = DateTime.UtcNow });
        db.Payments.Add(new Payment { PaymentID = Guid.NewGuid(), PaymentIntentID = intentID,
            TenantID = wrongTenant ? Guid.NewGuid() : tenantID, GrossAmount = gross, RefundedAmount = refunded,
            NetAmount = gross, PaidAt = DateTime.UtcNow, Status = "PAID", ProviderChargeID = "ch_synthetic" });
        await db.SaveChangesAsync();
        Assert.Equal(expected, await access.IsActiveAndPaidAsync(lease, default));
        Assert.Equal(expected, (await access.AllowedAddressesAsync(User(42), [addressID], default)).Contains(addressID));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task StreetEditsInvalidateCoordinatesUnlessHostConfirmsReplacement(bool confirmed)
    {
        await using var services = AddressServices();
        var service = AddressService(services);
        var update = await service.GetID(addressID);
        Assert.NotNull(update);
        update.Line1 = "2 Synthetic Street";
        update.AddressID = Guid.NewGuid();
        update.LocationConfirmed = confirmed;
        update.Latitude = 43.651m;
        update.Longitude = -79.381m;
        var saved = await service.UpdateAddress(addressID, update);
        Assert.NotNull(saved);
        Assert.Equal(addressID, saved.AddressID);
        Assert.Equal(confirmed ? 43.651m : (decimal?)null, saved.Latitude);
        Assert.Equal(confirmed ? -79.381m : (decimal?)null, saved.Longitude);
        Assert.Equal(1, await db.Addresses.CountAsync());
    }

    [Fact]
    public async Task SuiteOnlyEditsPreserveTheConfirmedBuildingLocation()
    {
        await using var services = AddressServices();
        var service = AddressService(services);
        var update = (await service.GetID(addressID))!;
        update.Line2 = "Suite 2";
        var saved = (await service.UpdateAddress(addressID, update))!;
        Assert.Equal(43.6501m, saved.Latitude);
        Assert.Equal(-79.3833m, saved.Longitude);
    }

    [Fact]
    public async Task AddressDeduplicationReturnsExistingIdButDoesNotMergeDifferentSuites()
    {
        await using var services = AddressServices();
        var service = AddressService(services);
        var request = (await service.GetID(addressID))!;
        var duplicate = await service.CreateAddress(request);
        Assert.Equal(addressID, duplicate.AddressID);
        request.Line2 = "Suite 2";
        var separate = await service.CreateAddress(request);
        Assert.NotEqual(addressID, separate.AddressID);
        Assert.Equal("Suite 2", separate.Line2);
        Assert.Equal(2, await db.Addresses.CountAsync());
    }

    private AddressService AddressService(ServiceProvider services) => new(
        new MapperConfiguration(config => config.AddProfile<AddressProfile>(), NullLoggerFactory.Instance).CreateMapper(),
        services.GetRequiredService<HybridCache>(), Options.Create(new CacheConfiguration()),
        NullLogger<AddressService>.Instance, new AddressRepository(db), new PostalLookupRepository(db),
        services.GetRequiredService<IHttpClientFactory>(), new ConfigurationBuilder().Build());

    private static ServiceProvider AddressServices()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddHybridCache();
        services.AddHttpClient();
        return services.BuildServiceProvider();
    }

    private Lease Lease(string status, DateTime start) => new()
    {
        LeaseID = Guid.NewGuid(), OrganizationID = organizationID, ListingID = listingID, RentalUnitID = unitID,
        TenantID = tenantID, TenancyTypeID = 1, LeaseCode = "SYNTHETIC", LeaseNumber = "SYNTHETIC",
        Status = status, StartDate = start, EndDate = start.AddMonths(3), BaseRentAmount = 1000
    };
    private static ClaimsPrincipal User(long id) => new(new ClaimsIdentity([new Claim("UserId", id.ToString())], "Test"));
    public async Task DisposeAsync() { await db.DisposeAsync(); await connection.DisposeAsync(); }
    private sealed class TestContext(DbContextOptions<ArcoraDbContext> options, IConfiguration configuration) : ArcoraDbContext(options, configuration)
    {
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            foreach (var property in builder.Model.GetEntityTypes().SelectMany(entity => entity.GetProperties()))
                property.SetColumnType(null);
        }
    }
}
