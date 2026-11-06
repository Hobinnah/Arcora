using Arcora.Api;
using Arcora.Api.Entities;
using Arcora.Api.Models;
using Arcora.Api.Repositories.Implementations;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Arcora.Api.Tests;

public sealed class ListingMarketplaceVisibilityTests : IAsyncLifetime
{
    private readonly SqliteConnection connection = new("Data Source=:memory:;Foreign Keys=False");
    private ArcoraDbContext db = null!;
    private ListingRepository repository = null!;

    public async Task InitializeAsync()
    {
        await connection.OpenAsync();
        var configuration = new ConfigurationBuilder().Build();
        db = new TestContext(new DbContextOptionsBuilder<ArcoraDbContext>().UseSqlite(connection).Options, configuration);
        await db.Database.EnsureCreatedAsync();
        repository = new ListingRepository(db);
        var organizationID = Guid.NewGuid();
        var addressID = Guid.NewGuid();
        var propertyID = Guid.NewGuid();
        var rentalUnitID = Guid.NewGuid();
        var listingTypeID = Guid.NewGuid();
        db.Organizations.Add(new Organization
        {
            OrganizationID = organizationID,
            LegalName = "Visibility Test Organization",
            DisplayName = "Visibility Test Organization",
            CountryCode = "CA",
            IsPersonal = false,
            Status = "ACTIVE",
            DefaultCurrency = "CAD",
            TimeZone = "UTC"
        });
        db.Add(new Address
        {
            AddressID = addressID,
            Line1 = "1 Test Street",
            City = "Toronto",
            CountryCode = "CA"
        });
        db.Add(new Property
        {
            PropertyID = propertyID,
            OrganizationID = organizationID,
            AddressID = addressID,
            PropertyType = "Apartment building",
            TimeZone = "UTC",
            Status = "ACTIVE"
        });
        db.RentalUnits.Add(new RentalUnit
        {
            RentalUnitID = rentalUnitID,
            PropertyID = propertyID,
            Status = "ACTIVE"
        });
        db.Add(new ListingType
        {
            ListingTypeID = listingTypeID,
            Name = "Long Term",
            CapturedBy = "Tests"
        });
        db.Listings.AddRange(
            CreateListing("Approved home", "APPROVED", organizationID, rentalUnitID, listingTypeID),
            CreateListing("Published home", "PUBLISHED", organizationID, rentalUnitID, listingTypeID),
            CreateListing("Draft home", "DRAFT", organizationID, rentalUnitID, listingTypeID));
        await db.SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        await db.DisposeAsync();
        await connection.DisposeAsync();
    }

    [Fact]
    public async Task MarketplaceSearchReturnsOnlyApprovedOrPublishedListings()
    {
        var (items, totalCount) = await repository.SearchListingsAsync(new ListingSearchCriteria
        {
            PageNumber = 1,
            PageSize = 20
        });

        Assert.Equal(2, totalCount);
        Assert.Equal(new[] { "APPROVED", "PUBLISHED" }, items.Select(item => item.Status).OrderBy(status => status).ToArray());
        Assert.DoesNotContain(items, item => item.Status == "DRAFT");
    }

    private static Listing CreateListing(string title, string status, Guid organizationID, Guid rentalUnitID, Guid listingTypeID) => new()
    {
        ListingID = Guid.NewGuid(),
        RentalUnitID = rentalUnitID,
        ListingTypeID = listingTypeID,
        OrganizationID = organizationID,
        Title = title,
        Status = status,
        Currency = "CAD",
        BaseMonthlyRentAmount = 1000,
        SecurityDepositAmount = 1000,
        YearBuilt = 2000,
        MinimumLeaseMonths = 1,
        MaximumLeaseMonths = 12,
        Notes = string.Empty,
        AcceptingApplications = true,
        CapturedDate = DateTime.UtcNow
    };

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