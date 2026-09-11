using Arcora.Api.Entities;

namespace Arcora.Api.Data;

/// <summary>
/// Provides in-memory, fully cross-referenced sample data for the core listing domain.
/// All foreign keys use the shared <see cref="Ids"/> constants so every child record
/// references a valid parent, without requiring a physical database.
/// </summary>
public static class SeedData
{
    private static readonly DateTime Now = new(2024, 1, 15, 9, 0, 0, DateTimeKind.Utc);
    private const string SystemUser = "seed";

    /// <summary>
    /// Stable identifiers shared across the seeded object graph so foreign keys line up.
    /// </summary>
    public static class Ids
    {
        // Organizations
        public static readonly Guid OrgMaple = new("11111111-1111-1111-1111-111111111111");
        public static readonly Guid OrgHarbor = new("22222222-2222-2222-2222-222222222222");

        // Addresses
        public static readonly Guid AddressMaple = new("a1111111-1111-1111-1111-111111111111");
        public static readonly Guid AddressHarbor = new("a2222222-2222-2222-2222-222222222222");

        // Properties
        public static readonly Guid PropertyMaple = new("b1111111-1111-1111-1111-111111111111");
        public static readonly Guid PropertyHarbor = new("b2222222-2222-2222-2222-222222222222");

        // Unit types
        public static readonly Guid UnitTypeStudio = new("c1111111-1111-1111-1111-111111111111");
        public static readonly Guid UnitTypeOneBed = new("c2222222-2222-2222-2222-222222222222");
        public static readonly Guid UnitTypeTwoBed = new("c3333333-3333-3333-3333-333333333333");
        public static readonly Guid UnitTypeApartment = new("c4444444-4444-4444-4444-444444444444");
        public static readonly Guid UnitTypeHouse = new("c5555555-5555-5555-5555-555555555555");
        public static readonly Guid UnitTypeLoft = new("c6666666-6666-6666-6666-666666666666");

        // Rental units
        public static readonly Guid UnitMaple101 = new("d1111111-1111-1111-1111-111111111111");
        public static readonly Guid UnitMaple102 = new("d2222222-2222-2222-2222-222222222222");
        public static readonly Guid UnitHarbor201 = new("d3333333-3333-3333-3333-333333333333");
        public static readonly Guid UnitMaple103 = new("d4444444-4444-4444-4444-444444444444");
        public static readonly Guid UnitMaple201 = new("d5555555-5555-5555-5555-555555555555");
        public static readonly Guid UnitMaple202 = new("d6666666-6666-6666-6666-666666666666");
        public static readonly Guid UnitHarbor202 = new("d7777777-7777-7777-7777-777777777777");
        public static readonly Guid UnitHarbor301 = new("d8888888-8888-8888-8888-888888888888");
        public static readonly Guid UnitHarbor302 = new("d9999999-9999-9999-9999-999999999999");
        public static readonly Guid UnitHarbor401 = new("daaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

        // Listing types
        public static readonly Guid ListingTypeLongTerm = new("e1111111-1111-1111-1111-111111111111");
        public static readonly Guid ListingTypeShortTerm = new("e2222222-2222-2222-2222-222222222222");

        // Listings
        public static readonly Guid ListingMaple101 = new("f1111111-1111-1111-1111-111111111111");
        public static readonly Guid ListingHarbor201 = new("f2222222-2222-2222-2222-222222222222");
        public static readonly Guid ListingMaple102 = new("f3333333-3333-3333-3333-333333333333");
        public static readonly Guid ListingMaple103 = new("f4444444-4444-4444-4444-444444444444");
        public static readonly Guid ListingMaple201 = new("f5555555-5555-5555-5555-555555555555");
        public static readonly Guid ListingMaple202 = new("f6666666-6666-6666-6666-666666666666");
        public static readonly Guid ListingHarbor202 = new("f7777777-7777-7777-7777-777777777777");
        public static readonly Guid ListingHarbor301 = new("f8888888-8888-8888-8888-888888888888");
        public static readonly Guid ListingHarbor302 = new("f9999999-9999-9999-9999-999999999999");
        public static readonly Guid ListingHarbor401 = new("faaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

        // Amenities (catalog)
        public static readonly Guid AmenityWifi = new("aa111111-1111-1111-1111-111111111111");
        public static readonly Guid AmenityParking = new("aa222222-2222-2222-2222-222222222222");
        public static readonly Guid AmenityLaundry = new("aa333333-3333-3333-3333-333333333333");
        public static readonly Guid AmenityAirConditioning = new("aa444444-4444-4444-4444-444444444444");
        public static readonly Guid AmenityHeating = new("aa555555-5555-5555-5555-555555555555");
        public static readonly Guid AmenityGym = new("aa666666-6666-6666-6666-666666666666");
        public static readonly Guid AmenityPool = new("aa777777-7777-7777-7777-777777777777");
        public static readonly Guid AmenityDishwasher = new("aa888888-8888-8888-8888-888888888888");
        public static readonly Guid AmenityBalcony = new("aa999999-9999-9999-9999-999999999999");
        public static readonly Guid AmenityElevator = new("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

        // Tenants
        public static readonly Guid TenantAlice = new("7a111111-1111-1111-1111-111111111111");
        public static readonly Guid TenantBob = new("7a222222-2222-2222-2222-222222222222");

        // Leases
        public static readonly Guid LeaseMaple101 = new("7e111111-1111-1111-1111-111111111111");
        public static readonly Guid LeaseHarbor201 = new("7e222222-2222-2222-2222-222222222222");
        public static readonly Guid LeaseMaple102 = new("7e333333-3333-3333-3333-333333333333");
        public static readonly Guid LeaseMaple103 = new("7e444444-4444-4444-4444-444444444444");
        public static readonly Guid LeaseMaple201 = new("7e555555-5555-5555-5555-555555555555");
        public static readonly Guid LeaseMaple202 = new("7e666666-6666-6666-6666-666666666666");
        public static readonly Guid LeaseHarbor202 = new("7e777777-7777-7777-7777-777777777777");
        public static readonly Guid LeaseHarbor301 = new("7e888888-8888-8888-8888-888888888888");
        public static readonly Guid LeaseHarbor302 = new("7e999999-9999-9999-9999-999999999999");
        public static readonly Guid LeaseHarbor401 = new("7eaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
    }

    /// <summary>Landlord organizations.</summary>
    public static IReadOnlyList<Organization> Organizations { get; } = new List<Organization>
    {
        new()
        {
            OrganizationID = Ids.OrgMaple,
            LegalName = "Maple Grove Properties Inc.",
            DisplayName = "Maple Grove Properties",
            BusinessNumber = "BN-100200300",
            CountryCode = "CA",
            ProvinceCode = "ON",
            IsPersonal = false,
            Status = "ACTIVE",
            DefaultCurrency = "CAD",
            TimeZone = "America/Toronto",
            InvoicePrefix = "MG-INV",
            ReceiptPrefix = "MG-RCP",
            LateFeeEnabled = true,
            AutoInvoiceGeneration = true,
            AutoPaymentRetry = true,
            PaymentProvider = "Stripe",
            RequireBackgroundCheck = true,
            BrandLogoUrl = "https://images.unsplash.com/photo-1560250097-0b93528c311a?auto=format&fit=crop&w=200&q=80",
            RankingScore = 4.95m,
            CapturedDate = Now,
            CapturedBy = SystemUser
        },
        new()
        {
            OrganizationID = Ids.OrgHarbor,
            LegalName = "Harbor View Rentals Ltd.",
            DisplayName = "Harbor View Rentals",
            BusinessNumber = "BN-400500600",
            CountryCode = "CA",
            ProvinceCode = "BC",
            IsPersonal = false,
            Status = "ACTIVE",
            DefaultCurrency = "CAD",
            TimeZone = "America/Vancouver",
            InvoicePrefix = "HV-INV",
            ReceiptPrefix = "HV-RCP",
            LateFeeEnabled = false,
            AutoInvoiceGeneration = true,
            AutoPaymentRetry = false,
            PaymentProvider = "Stripe",
            RequireBackgroundCheck = false,
            BrandLogoUrl = "https://images.unsplash.com/photo-1519085360753-af0119f7cbe7?auto=format&fit=crop&w=200&q=80",
            RankingScore = 4.9m,
            CapturedDate = Now,
            CapturedBy = SystemUser
        }
    };

    /// <summary>Members belonging to the organizations
    public static IReadOnlyList<OrganizationMember> OrganizationMembers { get; } = new List<OrganizationMember>
    {
        new()
        {
            OrganizationMemberID = new Guid("0a111111-1111-1111-1111-111111111111"),
            OrganizationID = Ids.OrgMaple,
            UserID = 1,
            RoleName = "OWNER",
            Status = "ACTIVE",
            IsPrimaryOwner = true,
            InvitedAt = Now.AddMonths(-6),
            AcceptedAt = Now.AddMonths(-6).AddDays(1),
            CapturedDate = Now,
            CapturedBy = SystemUser
        },
        new()
        {
            OrganizationMemberID = new Guid("0a222222-2222-2222-2222-222222222222"),
            OrganizationID = Ids.OrgMaple,
            UserID = 1,
            RoleName = "MANAGER",
            Status = "ACTIVE",
            IsPrimaryOwner = false,
            InvitedAt = Now.AddMonths(-3),
            AcceptedAt = Now.AddMonths(-3).AddDays(2),
            CapturedDate = Now,
            CapturedBy = SystemUser
        },
        new()
        {
            OrganizationMemberID = new Guid("0a333333-3333-3333-3333-333333333333"),
            OrganizationID = Ids.OrgHarbor,
            UserID = 1,
            RoleName = "OWNER",
            Status = "ACTIVE",
            IsPrimaryOwner = true,
            InvitedAt = Now.AddMonths(-8),
            AcceptedAt = Now.AddMonths(-8).AddDays(1),
            CapturedDate = Now,
            CapturedBy = SystemUser
        }
    };

    /// <summary>Addresses linked to organizations.</summary>
    public static IReadOnlyList<Address> Addresses { get; } = new List<Address>
    {
        new()
        {
            AddressID = Ids.AddressMaple,
            OrganizationID = Ids.OrgMaple,
            AddressType = "PROPERTY",
            Line1 = "120 Maple Grove Avenue",
            City = "Toronto",
            ProvinceCode = "ON",
            PostalCode = "M4C 1B5",
            CountryCode = "CA",
            Latitude = 43.6890m,
            Longitude = -79.2990m,
            CapturedDate = Now,
            CapturedBy = SystemUser
        },
        new()
        {
            AddressID = Ids.AddressHarbor,
            OrganizationID = Ids.OrgHarbor,
            AddressType = "PROPERTY",
            Line1 = "88 Harbor View Road",
            Line2 = "Building B",
            City = "Vancouver",
            ProvinceCode = "BC",
            PostalCode = "V6B 1A1",
            CountryCode = "CA",
            Latitude = 49.2827m,
            Longitude = -123.1207m,
            CapturedDate = Now,
            CapturedBy = SystemUser
        }
    };

    /// <summary>Properties referencing an organization and an address.</summary>
    public static IReadOnlyList<Property> Properties { get; } = new List<Property>
    {
        new()
        {
            PropertyID = Ids.PropertyMaple,
            OrganizationID = Ids.OrgMaple,
            AddressID = Ids.AddressMaple,
            Name = "Maple Grove Residences",
            PropertyType = "APARTMENT",
            YearBuilt = 2015,
            TimeZone = "America/Toronto",
            Description = "Modern low-rise apartment building with 24 units.",
            Status = "ACTIVE",
            CapturedDate = Now,
            CapturedBy = SystemUser
        },
        new()
        {
            PropertyID = Ids.PropertyHarbor,
            OrganizationID = Ids.OrgHarbor,
            AddressID = Ids.AddressHarbor,
            Name = "Harbor View Towers",
            PropertyType = "CONDO",
            YearBuilt = 2019,
            TimeZone = "America/Vancouver",
            Description = "Waterfront condo tower with ocean views.",
            Status = "ACTIVE",
            CapturedDate = Now,
            CapturedBy = SystemUser
        }
    };

    /// <summary>Unit type lookup values.</summary>
    public static IReadOnlyList<UnitType> UnitTypes { get; } = new List<UnitType>
    {
        new() { UnitTypeID = Ids.UnitTypeStudio, Name = "Studio", CapturedDate = Now, CapturedBy = SystemUser },
        new() { UnitTypeID = Ids.UnitTypeOneBed, Name = "1 Bedroom", CapturedDate = Now, CapturedBy = SystemUser },
        new() { UnitTypeID = Ids.UnitTypeTwoBed, Name = "2 Bedroom", CapturedDate = Now, CapturedBy = SystemUser },
        new() { UnitTypeID = Ids.UnitTypeApartment, Name = "Apartment", CapturedDate = Now, CapturedBy = SystemUser },
        new() { UnitTypeID = Ids.UnitTypeHouse, Name = "House", CapturedDate = Now, CapturedBy = SystemUser },
        new() { UnitTypeID = Ids.UnitTypeLoft, Name = "Loft", CapturedDate = Now, CapturedBy = SystemUser }
    };

    /// <summary>Listing type lookup values.</summary>
    public static IReadOnlyList<ListingType> ListingTypes { get; } = new List<ListingType>
    {
        new() { ListingTypeID = Ids.ListingTypeLongTerm, Name = "Long Term", CapturedDate = Now, CapturedBy = SystemUser },
        new() { ListingTypeID = Ids.ListingTypeShortTerm, Name = "Short Term", CapturedDate = Now, CapturedBy = SystemUser }
    };

    /// <summary>Tenancy type lookup values.</summary>
    public static IReadOnlyList<TenancyType> TenancyTypes { get; } = new List<TenancyType>
    {
        new()
        {
            Code = "FIXED",
            Name = "Fixed Term",
            Description = "A lease with a defined start and end date.",
            MinimumMonths = 6,
            MaximumMonths = 12,
            IsPeriodic = false,
            IsActive = true,
            CapturedDate = Now,
            CapturedBy = SystemUser
        },
        new()
        {
            Code = "MONTHLY",
            Name = "Month-to-Month",
            Description = "A periodic tenancy renewing every month.",
            MinimumMonths = 1,
            MaximumMonths = null,
            IsPeriodic = true,
            IsActive = true,
            CapturedDate = Now,
            CapturedBy = SystemUser
        }
    };

    /// <summary>Rental units referencing a property and a unit type.</summary>
    public static IReadOnlyList<RentalUnit> RentalUnits { get; } = new List<RentalUnit>
    {
        new()
        {
            RentalUnitID = Ids.UnitMaple101,
            PropertyID = Ids.PropertyMaple,
            UnitTypeID = Ids.UnitTypeOneBed,
            UnitNumber = "101",
            FloorNumber = "1",
            Bedrooms = 1m,
            Bathrooms = 1m,
            SquareFeet = 620,
            MaximumOccupants = 2,
            Notes = "Corner unit with extra window.",
            Status = "ACTIVE",
            CapturedDate = Now,
            CapturedBy = SystemUser
        },
        new()
        {
            RentalUnitID = Ids.UnitMaple102,
            PropertyID = Ids.PropertyMaple,
            UnitTypeID = Ids.UnitTypeStudio,
            UnitNumber = "102",
            FloorNumber = "1",
            Bedrooms = 0m,
            Bathrooms = 1m,
            SquareFeet = 450,
            MaximumOccupants = 1,
            Status = "ACTIVE",
            CapturedDate = Now,
            CapturedBy = SystemUser
        },
        new()
        {
            RentalUnitID = Ids.UnitHarbor201,
            PropertyID = Ids.PropertyHarbor,
            UnitTypeID = Ids.UnitTypeTwoBed,
            UnitNumber = "201",
            FloorNumber = "2",
            Bedrooms = 2m,
            Bathrooms = 2m,
            SquareFeet = 980,
            MaximumOccupants = 4,
            Notes = "Ocean-facing balcony.",
            Status = "ACTIVE",
            CapturedDate = Now,
            CapturedBy = SystemUser
        },
        new()
        {
            RentalUnitID = Ids.UnitMaple103,
            PropertyID = Ids.PropertyMaple,
            UnitTypeID = Ids.UnitTypeOneBed,
            UnitNumber = "103",
            FloorNumber = "1",
            Bedrooms = 1m,
            Bathrooms = 1m,
            SquareFeet = 640,
            MaximumOccupants = 2,
            Status = "ACTIVE",
            CapturedDate = Now,
            CapturedBy = SystemUser
        },
        new()
        {
            RentalUnitID = Ids.UnitMaple201,
            PropertyID = Ids.PropertyMaple,
            UnitTypeID = Ids.UnitTypeTwoBed,
            UnitNumber = "201",
            FloorNumber = "2",
            Bedrooms = 2m,
            Bathrooms = 1m,
            SquareFeet = 890,
            MaximumOccupants = 3,
            Notes = "Recently renovated kitchen.",
            Status = "ACTIVE",
            CapturedDate = Now,
            CapturedBy = SystemUser
        },
        new()
        {
            RentalUnitID = Ids.UnitMaple202,
            PropertyID = Ids.PropertyMaple,
            UnitTypeID = Ids.UnitTypeStudio,
            UnitNumber = "202",
            FloorNumber = "2",
            Bedrooms = 0m,
            Bathrooms = 1m,
            SquareFeet = 470,
            MaximumOccupants = 1,
            Status = "ACTIVE",
            CapturedDate = Now,
            CapturedBy = SystemUser
        },
        new()
        {
            RentalUnitID = Ids.UnitHarbor202,
            PropertyID = Ids.PropertyHarbor,
            UnitTypeID = Ids.UnitTypeOneBed,
            UnitNumber = "202",
            FloorNumber = "2",
            Bedrooms = 1m,
            Bathrooms = 1m,
            SquareFeet = 700,
            MaximumOccupants = 2,
            Status = "ACTIVE",
            CapturedDate = Now,
            CapturedBy = SystemUser
        },
        new()
        {
            RentalUnitID = Ids.UnitHarbor301,
            PropertyID = Ids.PropertyHarbor,
            UnitTypeID = Ids.UnitTypeTwoBed,
            UnitNumber = "301",
            FloorNumber = "3",
            Bedrooms = 2m,
            Bathrooms = 2m,
            SquareFeet = 1010,
            MaximumOccupants = 4,
            Notes = "Ocean-facing balcony.",
            Status = "ACTIVE",
            CapturedDate = Now,
            CapturedBy = SystemUser
        },
        new()
        {
            RentalUnitID = Ids.UnitHarbor302,
            PropertyID = Ids.PropertyHarbor,
            UnitTypeID = Ids.UnitTypeOneBed,
            UnitNumber = "302",
            FloorNumber = "3",
            Bedrooms = 1m,
            Bathrooms = 1m,
            SquareFeet = 720,
            MaximumOccupants = 2,
            Status = "ACTIVE",
            CapturedDate = Now,
            CapturedBy = SystemUser
        },
        new()
        {
            RentalUnitID = Ids.UnitHarbor401,
            PropertyID = Ids.PropertyHarbor,
            UnitTypeID = Ids.UnitTypeTwoBed,
            UnitNumber = "401",
            FloorNumber = "4",
            Bedrooms = 2m,
            Bathrooms = 2m,
            SquareFeet = 1050,
            MaximumOccupants = 4,
            Notes = "Penthouse level with panoramic views.",
            Status = "ACTIVE",
            CapturedDate = Now,
            CapturedBy = SystemUser
        }
    };

    /// <summary>Listings referencing a rental unit, listing type, and organization.</summary>
    public static IReadOnlyList<Listing> Listings { get; } = new List<Listing>
    {
        new()
        {
            ListingID = Ids.ListingMaple101,
            RentalUnitID = Ids.UnitMaple101,
            ListingTypeID = Ids.ListingTypeLongTerm,
            OrganizationID = Ids.OrgMaple,
            Title = "Bright 1-Bedroom at Maple Grove",
            Description = "Sunlit corner one-bedroom close to transit and parks.",
            Bedrooms = 1m,
            Bathrooms = 1m,
            SquareFeet = 620m,
            BaseMonthlyRentAmount = 1850m,
            SecurityDepositAmount = 1850m,
            YearBuilt = 2015,
            Status = "PUBLISHED",
            PublishedAt = Now.AddDays(-10),
            Currency = "CAD",
            AvailableFrom = Now.AddDays(15),
            MinimumLeaseMonths = 12,
            MaximumLeaseMonths = 24,
            Notes = "No smoking. Small pets considered.",
            WIFINetwork = "MapleGrove-101",
            WIFIPassword = "welcome101",
            AcceptingApplications = true,
            CapturedDate = Now,
            CapturedBy = SystemUser
        },
        new()
        {
            ListingID = Ids.ListingHarbor201,
            RentalUnitID = Ids.UnitHarbor201,
            ListingTypeID = Ids.ListingTypeShortTerm,
            OrganizationID = Ids.OrgHarbor,
            Title = "Waterfront 2-Bedroom with Ocean Views",
            Description = "Spacious two-bedroom condo with a private balcony overlooking the harbor.",
            Bedrooms = 2m,
            Bathrooms = 2m,
            SquareFeet = 980m,
            BaseMonthlyRentAmount = 3400m,
            SecurityDepositAmount = 3400m,
            YearBuilt = 2019,
            Status = "PUBLISHED",
            PublishedAt = Now.AddDays(-5),
            Currency = "CAD",
            AvailableFrom = Now.AddDays(30),
            MinimumLeaseMonths = 3,
            MaximumLeaseMonths = 12,
            Notes = "Fully furnished. Utilities included.",
            WIFINetwork = "HarborView-201",
            WIFIPassword = "ocean201",
            AcceptingApplications = true,
            CapturedDate = Now,
            CapturedBy = SystemUser
        },
        new()
        {
            ListingID = Ids.ListingMaple102,
            RentalUnitID = Ids.UnitMaple102,
            ListingTypeID = Ids.ListingTypeLongTerm,
            OrganizationID = Ids.OrgMaple,
            Title = "Cozy Studio at Maple Grove",
            Description = "Efficient studio perfect for students or young professionals.",
            Bedrooms = 0m,
            Bathrooms = 1m,
            SquareFeet = 450m,
            BaseMonthlyRentAmount = 1400m,
            SecurityDepositAmount = 1400m,
            YearBuilt = 2015,
            Status = "PUBLISHED",
            PublishedAt = Now.AddDays(-8),
            Currency = "CAD",
            AvailableFrom = Now.AddDays(20),
            MinimumLeaseMonths = 12,
            MaximumLeaseMonths = 24,
            Notes = "No smoking. No pets.",
            WIFINetwork = "MapleGrove-102",
            WIFIPassword = "welcome102",
            AcceptingApplications = true,
            CapturedDate = Now,
            CapturedBy = SystemUser
        },
        new()
        {
            ListingID = Ids.ListingMaple103,
            RentalUnitID = Ids.UnitMaple103,
            ListingTypeID = Ids.ListingTypeLongTerm,
            OrganizationID = Ids.OrgMaple,
            Title = "Spacious 1-Bedroom with Extra Storage",
            Description = "One-bedroom unit featuring a large closet and in-unit laundry.",
            Bedrooms = 1m,
            Bathrooms = 1m,
            SquareFeet = 640m,
            BaseMonthlyRentAmount = 1900m,
            SecurityDepositAmount = 1900m,
            YearBuilt = 2015,
            Status = "PUBLISHED",
            PublishedAt = Now.AddDays(-7),
            Currency = "CAD",
            AvailableFrom = Now.AddDays(25),
            MinimumLeaseMonths = 12,
            MaximumLeaseMonths = 24,
            Notes = "Cats allowed with deposit.",
            WIFINetwork = "MapleGrove-103",
            WIFIPassword = "welcome103",
            AcceptingApplications = true,
            CapturedDate = Now,
            CapturedBy = SystemUser
        },
        new()
        {
            ListingID = Ids.ListingMaple201,
            RentalUnitID = Ids.UnitMaple201,
            ListingTypeID = Ids.ListingTypeLongTerm,
            OrganizationID = Ids.OrgMaple,
            Title = "Renovated 2-Bedroom at Maple Grove",
            Description = "Bright two-bedroom with a modern kitchen and open living space.",
            Bedrooms = 2m,
            Bathrooms = 1m,
            SquareFeet = 890m,
            BaseMonthlyRentAmount = 2450m,
            SecurityDepositAmount = 2450m,
            YearBuilt = 2015,
            Status = "PUBLISHED",
            PublishedAt = Now.AddDays(-6),
            Currency = "CAD",
            AvailableFrom = Now.AddDays(18),
            MinimumLeaseMonths = 12,
            MaximumLeaseMonths = 24,
            Notes = "Ideal for a small family.",
            WIFINetwork = "MapleGrove-201",
            WIFIPassword = "welcome201",
            AcceptingApplications = true,
            CapturedDate = Now,
            CapturedBy = SystemUser
        },
        new()
        {
            ListingID = Ids.ListingMaple202,
            RentalUnitID = Ids.UnitMaple202,
            ListingTypeID = Ids.ListingTypeShortTerm,
            OrganizationID = Ids.OrgMaple,
            Title = "Furnished Studio - Short Term",
            Description = "Fully furnished studio available for short-term stays.",
            Bedrooms = 0m,
            Bathrooms = 1m,
            SquareFeet = 470m,
            BaseMonthlyRentAmount = 1750m,
            SecurityDepositAmount = 1000m,
            YearBuilt = 2015,
            Status = "PUBLISHED",
            PublishedAt = Now.AddDays(-4),
            Currency = "CAD",
            AvailableFrom = Now.AddDays(10),
            MinimumLeaseMonths = 1,
            MaximumLeaseMonths = 6,
            Notes = "Utilities and WiFi included.",
            WIFINetwork = "MapleGrove-202",
            WIFIPassword = "welcome202",
            AcceptingApplications = true,
            CapturedDate = Now,
            CapturedBy = SystemUser
        },
        new()
        {
            ListingID = Ids.ListingHarbor202,
            RentalUnitID = Ids.UnitHarbor202,
            ListingTypeID = Ids.ListingTypeLongTerm,
            OrganizationID = Ids.OrgHarbor,
            Title = "Modern 1-Bedroom Near the Harbor",
            Description = "Contemporary one-bedroom with easy access to the waterfront.",
            Bedrooms = 1m,
            Bathrooms = 1m,
            SquareFeet = 700m,
            BaseMonthlyRentAmount = 2600m,
            SecurityDepositAmount = 2600m,
            YearBuilt = 2019,
            Status = "PUBLISHED",
            PublishedAt = Now.AddDays(-9),
            Currency = "CAD",
            AvailableFrom = Now.AddDays(22),
            MinimumLeaseMonths = 12,
            MaximumLeaseMonths = 24,
            Notes = "Gym and rooftop access included.",
            WIFINetwork = "HarborView-202",
            WIFIPassword = "ocean202",
            AcceptingApplications = true,
            CapturedDate = Now,
            CapturedBy = SystemUser
        },
        new()
        {
            ListingID = Ids.ListingHarbor301,
            RentalUnitID = Ids.UnitHarbor301,
            ListingTypeID = Ids.ListingTypeShortTerm,
            OrganizationID = Ids.OrgHarbor,
            Title = "Luxury 2-Bedroom with Panoramic Views",
            Description = "High-floor two-bedroom condo with sweeping ocean and city views.",
            Bedrooms = 2m,
            Bathrooms = 2m,
            SquareFeet = 1010m,
            BaseMonthlyRentAmount = 3600m,
            SecurityDepositAmount = 3600m,
            YearBuilt = 2019,
            Status = "PUBLISHED",
            PublishedAt = Now.AddDays(-3),
            Currency = "CAD",
            AvailableFrom = Now.AddDays(28),
            MinimumLeaseMonths = 3,
            MaximumLeaseMonths = 12,
            Notes = "Fully furnished. Concierge service.",
            WIFINetwork = "HarborView-301",
            WIFIPassword = "ocean301",
            AcceptingApplications = true,
            CapturedDate = Now,
            CapturedBy = SystemUser
        },
        new()
        {
            ListingID = Ids.ListingHarbor302,
            RentalUnitID = Ids.UnitHarbor302,
            ListingTypeID = Ids.ListingTypeLongTerm,
            OrganizationID = Ids.OrgHarbor,
            Title = "Bright 1-Bedroom on the Third Floor",
            Description = "Well-lit one-bedroom with an open layout and modern finishes.",
            Bedrooms = 1m,
            Bathrooms = 1m,
            SquareFeet = 720m,
            BaseMonthlyRentAmount = 2700m,
            SecurityDepositAmount = 2700m,
            YearBuilt = 2019,
            Status = "DRAFT",
            Currency = "CAD",
            AvailableFrom = Now.AddDays(35),
            MinimumLeaseMonths = 12,
            MaximumLeaseMonths = 24,
            Notes = "Pending final inspection.",
            WIFINetwork = "HarborView-302",
            WIFIPassword = "ocean302",
            AcceptingApplications = false,
            CapturedDate = Now,
            CapturedBy = SystemUser
        },
        new()
        {
            ListingID = Ids.ListingHarbor401,
            RentalUnitID = Ids.UnitHarbor401,
            ListingTypeID = Ids.ListingTypeShortTerm,
            OrganizationID = Ids.OrgHarbor,
            Title = "Penthouse 2-Bedroom with Rooftop Access",
            Description = "Top-floor penthouse offering panoramic views and premium finishes.",
            Bedrooms = 2m,
            Bathrooms = 2m,
            SquareFeet = 1050m,
            BaseMonthlyRentAmount = 4200m,
            SecurityDepositAmount = 4200m,
            YearBuilt = 2019,
            Status = "PUBLISHED",
            PublishedAt = Now.AddDays(-2),
            Currency = "CAD",
            AvailableFrom = Now.AddDays(40),
            MinimumLeaseMonths = 3,
            MaximumLeaseMonths = 12,
            Notes = "Premium furnishings. Parking included.",
            WIFINetwork = "HarborView-401",
            WIFIPassword = "ocean401",
            AcceptingApplications = true,
            CapturedDate = Now,
            CapturedBy = SystemUser
        }
    };

    /// <summary>Photos referencing listings.</summary>
    /// <summary>
    /// Per-listing specification used to generate a fully cross-referenced child graph
    /// (photos, amenities, rules, policies, calendar exclusions, leases and reviews) so that
    /// every one of the 10 listings can drive a complete listing-detail page.
    /// </summary>
    private sealed record ListingSpec(
        Guid ListingId,
        Guid OrgId,
        Guid UnitId,
        Guid LeaseId,
        Guid TenantId,
        int Bedrooms,
        string[] BedConfig,
        decimal Rent,
        int LeaseTermMonths,
        string City,
        string CoverImage,
        string[] RoomImages,
        string[] BedroomImages);

    private const string ImgLivingA = "https://images.unsplash.com/photo-1505693416388-ac5ce068fe85?auto=format&fit=crop&w=900&q=85";
    private const string ImgLivingB = "https://images.unsplash.com/photo-1522708323590-d24dbb6b0267?auto=format&fit=crop&w=900&q=85";
    private const string ImgLivingC = "https://images.unsplash.com/photo-1600585154340-be6161a56a0c?auto=format&fit=crop&w=900&q=85";
    private const string ImgKitchen = "https://images.unsplash.com/photo-1600607687920-4e2a09cf159d?auto=format&fit=crop&w=900&q=85";
    private const string ImgExterior = "https://images.unsplash.com/photo-1494526585095-c41746248156?auto=format&fit=crop&w=900&q=85";
    private const string ImgBedroomA = "https://images.unsplash.com/photo-1616594039964-ae9021a400a0?auto=format&fit=crop&w=900&q=85";
    private const string ImgBedroomB = "https://images.unsplash.com/photo-1560448204-e02f11c3d0e2?auto=format&fit=crop&w=900&q=85";
    private const string ImgBathroom = "https://images.unsplash.com/photo-1620626011761-996317b8d101?auto=format&fit=crop&w=900&q=85";

    /// <summary>The 10 listing specifications the child graph is generated from.</summary>
    private static IReadOnlyList<ListingSpec> ListingSpecs { get; } = new List<ListingSpec>
    {
        new(Ids.ListingMaple101, Ids.OrgMaple, Ids.UnitMaple101, Ids.LeaseMaple101, Ids.TenantAlice, 1, new[] { "1 queen bed" }, 1850m, 12, "Toronto", ImgLivingA, new[] { ImgKitchen, ImgLivingB, ImgBathroom, ImgExterior }, new[] { ImgBedroomA }),
        new(Ids.ListingHarbor201, Ids.OrgHarbor, Ids.UnitHarbor201, Ids.LeaseHarbor201, Ids.TenantBob, 2, new[] { "1 king bed", "1 queen bed" }, 3400m, 6, "Vancouver", ImgExterior, new[] { ImgLivingC, ImgKitchen, ImgBathroom }, new[] { ImgBedroomA, ImgBedroomB }),
        new(Ids.ListingMaple102, Ids.OrgMaple, Ids.UnitMaple102, Ids.LeaseMaple102, Ids.TenantAlice, 0, new[] { "1 queen bed" }, 1400m, 12, "Toronto", ImgLivingC, new[] { ImgKitchen, ImgBathroom }, new[] { ImgBedroomB }),
        new(Ids.ListingMaple103, Ids.OrgMaple, Ids.UnitMaple103, Ids.LeaseMaple103, Ids.TenantBob, 1, new[] { "1 queen bed" }, 1900m, 12, "Toronto", ImgLivingB, new[] { ImgKitchen, ImgLivingA, ImgBathroom }, new[] { ImgBedroomA }),
        new(Ids.ListingMaple201, Ids.OrgMaple, Ids.UnitMaple201, Ids.LeaseMaple201, Ids.TenantAlice, 2, new[] { "1 king bed", "1 double bed" }, 2450m, 12, "Toronto", ImgLivingA, new[] { ImgKitchen, ImgLivingB, ImgBathroom, ImgExterior }, new[] { ImgBedroomA, ImgBedroomB }),
        new(Ids.ListingMaple202, Ids.OrgMaple, Ids.UnitMaple202, Ids.LeaseMaple202, Ids.TenantBob, 0, new[] { "1 queen bed" }, 1750m, 3, "Toronto", ImgLivingC, new[] { ImgKitchen, ImgBathroom }, new[] { ImgBedroomB }),
        new(Ids.ListingHarbor202, Ids.OrgHarbor, Ids.UnitHarbor202, Ids.LeaseHarbor202, Ids.TenantAlice, 1, new[] { "1 queen bed" }, 2600m, 12, "Vancouver", ImgLivingB, new[] { ImgKitchen, ImgLivingC, ImgBathroom }, new[] { ImgBedroomA }),
        new(Ids.ListingHarbor301, Ids.OrgHarbor, Ids.UnitHarbor301, Ids.LeaseHarbor301, Ids.TenantBob, 2, new[] { "1 king bed", "1 queen bed" }, 3600m, 6, "Vancouver", ImgExterior, new[] { ImgLivingA, ImgKitchen, ImgBathroom }, new[] { ImgBedroomA, ImgBedroomB }),
        new(Ids.ListingHarbor302, Ids.OrgHarbor, Ids.UnitHarbor302, Ids.LeaseHarbor302, Ids.TenantAlice, 1, new[] { "1 queen bed" }, 2700m, 12, "Vancouver", ImgBedroomA, new[] { ImgLivingB, ImgKitchen, ImgBathroom }, new[] { ImgBedroomA }),
        new(Ids.ListingHarbor401, Ids.OrgHarbor, Ids.UnitHarbor401, Ids.LeaseHarbor401, Ids.TenantBob, 2, new[] { "1 king bed", "1 king bed" }, 4200m, 6, "Vancouver", ImgExterior, new[] { ImgLivingC, ImgKitchen, ImgBathroom }, new[] { ImgBedroomA, ImgBedroomB }),
    };

    /// <summary>Rotating reviewer display names used to populate individual review content.</summary>
    private static readonly string[] ReviewerNames =
    {
        "Sarah", "James", "Emily", "Michael", "David", "Laura", "Olivia", "Noah",
        "Ava", "Ethan", "Mia", "Lucas", "Isabella", "Mason", "Charlotte", "Logan",
        "Amelia", "Benjamin", "Harper", "Elijah", "Evelyn", "Henry"
    };

    /// <summary>Reviewer home cities shown beneath each reviewer name.</summary>
    private static readonly string[] ReviewerLocations =
    {
        "Austin, Texas", "Portland, Oregon", "Denver, Colorado", "Chicago, Illinois",
        "Nashville, Tennessee", "Seattle, Washington", "Boston, Massachusetts", "Miami, Florida",
        "Phoenix, Arizona", "Atlanta, Georgia", "San Diego, California", "Minneapolis, Minnesota"
    };

    /// <summary>Rotating stay-context blurbs paired with each review.</summary>
    private static readonly string[] StayContexts =
    {
        "Stayed 3 months", "Stayed with family", "Stayed 6 months", "Group lease",
        "Stayed 1 month", "Stayed with kids", "Stayed 1 year", "Solo professional"
    };

    /// <summary>Rotating review bodies used to give each review distinct content.</summary>
    private static readonly string[] ReviewBodies =
    {
        "Wonderful place! Clean, well-maintained, and the location is perfect. Would absolutely rent here again.",
        "Great value for the price. The space is exactly as described. Management was responsive and helpful.",
        "We loved staying here. The neighborhood is quiet and safe, and the home has everything you need.",
        "Smooth move-in process and the autopay setup made rent collection effortless. Highly recommend.",
        "Everything was seamless from application to move-in. The home is well-kept and the neighbourhood is fantastic.",
        "Excellent value. Spacious layout, modern finishes, and management responds quickly to any question.",
        "Bright, comfortable and close to everything. The host was communicative throughout our stay.",
        "Accurate listing, fair pricing and an easy application process. We would happily rent again."
    };

    /// <summary>Deterministic-per-run helper to mint child record identifiers.</summary>
    private static Guid NewChildId() => Guid.NewGuid();

    /// <summary>Truncates a string to the supplied maximum length for fixed-width columns.</summary>
    private static string Truncate(string value, int maxLength) =>
        value.Length <= maxLength ? value : value.Substring(0, maxLength);

    public static IReadOnlyList<ListingPhoto> ListingPhotos { get; } = BuildListingPhotos();

    /// <summary>Builds cover, room and bedroom-by-bedroom photos for every listing.</summary>
    private static List<ListingPhoto> BuildListingPhotos()
    {
        var photos = new List<ListingPhoto>();
        foreach (var spec in ListingSpecs)
        {
            int order = 0;
            photos.Add(new ListingPhoto
            {
                ListingPhotoID = NewChildId(),
                ListingID = spec.ListingId,
                Url = spec.CoverImage,
                Location = "Exterior",
                Caption = "Featured photo",
                AltText = "Featured listing photo",
                DisplayOrder = order++,
                IsCoverPhoto = true,
                CapturedDate = Now,
                CapturedBy = SystemUser
            });

            foreach (var room in spec.RoomImages)
            {
                photos.Add(new ListingPhoto
                {
                    ListingPhotoID = NewChildId(),
                    ListingID = spec.ListingId,
                    Url = room,
                    Location = "LivingRoom",
                    Caption = "Interior",
                    AltText = "Interior photo",
                    DisplayOrder = order++,
                    IsCoverPhoto = false,
                    CapturedDate = Now,
                    CapturedBy = SystemUser
                });
            }

            for (int b = 0; b < spec.BedConfig.Length; b++)
            {
                var img = spec.BedroomImages[b % spec.BedroomImages.Length];
                var label = spec.Bedrooms == 0 ? "Studio" : $"Bedroom {b + 1}";
                photos.Add(new ListingPhoto
                {
                    ListingPhotoID = NewChildId(),
                    ListingID = spec.ListingId,
                    Url = img,
                    Location = "Bedroom",
                    Caption = $"{label} - {spec.BedConfig[b]}",
                    AltText = $"{label} with {spec.BedConfig[b]}",
                    DisplayOrder = order++,
                    IsCoverPhoto = false,
                    CapturedDate = Now,
                    CapturedBy = SystemUser
                });
            }
        }

        return photos;
    }

    // Legacy hand-authored photo list superseded by BuildListingPhotos(); retained for reference only.
    /*
    private static readonly ListingPhoto[] _legacyPhotos =
    {
        new()
        {
            ListingPhotoID = new Guid("1f111111-1111-1111-1111-111111111111"),
            ListingID = Ids.ListingMaple101,
            Url = "https://images.unsplash.com/photo-1505693416388-ac5ce068fe85?auto=format&fit=crop&w=900&q=85",
            Location = "LivingRoom",
            Caption = "Open-concept living room",
            AltText = "Living room with hardwood floors",
            DisplayOrder = 0,
            IsCoverPhoto = true,
            CapturedDate = Now,
            CapturedBy = SystemUser
        },
        new()
        {
            ListingPhotoID = new Guid("1f222222-2222-2222-2222-222222222222"),
            ListingID = Ids.ListingMaple101,
            Url = "https://images.unsplash.com/photo-1505693416388-ac5ce068fe85?auto=format&fit=crop&w=900&q=85",
            Location = "Bedroom",
            Caption = "Primary bedroom",
            AltText = "Bedroom with large window",
            DisplayOrder = 1,
            IsCoverPhoto = false,
            CapturedDate = Now,
            CapturedBy = SystemUser
        },
        new()
        {
            ListingPhotoID = new Guid("1f333333-3333-3333-3333-333333333333"),
            ListingID = Ids.ListingHarbor201,
            Url = "https://images.unsplash.com/photo-1494526585095-c41746248156?auto=format&fit=crop&w=900&q=85",
            Location = "Exterior",
            Caption = "Balcony ocean view",
            AltText = "Balcony overlooking the harbor",
            DisplayOrder = 0,
            IsCoverPhoto = true,
            CapturedDate = Now,
            CapturedBy = SystemUser
        },
        new()
        {
            ListingPhotoID = new Guid("1f444444-4444-4444-4444-444444444444"),
            ListingID = Ids.ListingMaple102,
            Url = "https://images.unsplash.com/photo-1600585154340-be6161a56a0c?auto=format&fit=crop&w=900&q=85",
            Location = "LivingRoom",
            Caption = "Compact living area",
            AltText = "Studio living space",
            DisplayOrder = 0,
            IsCoverPhoto = true,
            CapturedDate = Now,
            CapturedBy = SystemUser
        },
        new()
        {
            ListingPhotoID = new Guid("1f555555-5555-5555-5555-555555555555"),
            ListingID = Ids.ListingMaple103,
            Url = "https://images.unsplash.com/photo-1522708323590-d24dbb6b0267?auto=format&fit=crop&w=900&q=85",
            Location = "LivingRoom",
            Caption = "Bright living room with storage",
            AltText = "One-bedroom living room",
            DisplayOrder = 0,
            IsCoverPhoto = true,
            CapturedDate = Now,
            CapturedBy = SystemUser
        },
        new()
        {
            ListingPhotoID = new Guid("1f666666-6666-6666-6666-666666666666"),
            ListingID = Ids.ListingMaple201,
            Url = "https://images.unsplash.com/photo-1600607687920-4e2a09cf159d?auto=format&fit=crop&w=900&q=85",
            Location = "Kitchen",
            Caption = "Renovated modern kitchen",
            AltText = "Two-bedroom kitchen",
            DisplayOrder = 0,
            IsCoverPhoto = true,
            CapturedDate = Now,
            CapturedBy = SystemUser
        },
        new()
        {
            ListingPhotoID = new Guid("1f777777-7777-7777-7777-777777777777"),
            ListingID = Ids.ListingMaple202,
            Url = "https://images.unsplash.com/photo-1600047509807-ba8f99d2cdde?auto=format&fit=crop&w=900&q=85",
            Location = "LivingRoom",
            Caption = "Furnished studio interior",
            AltText = "Furnished studio living space",
            DisplayOrder = 0,
            IsCoverPhoto = true,
            CapturedDate = Now,
            CapturedBy = SystemUser
        },
        new()
        {
            ListingPhotoID = new Guid("1f888888-8888-8888-8888-888888888888"),
            ListingID = Ids.ListingHarbor202,
            Url = "https://images.unsplash.com/photo-1600210492486-724fe5c67fb0?auto=format&fit=crop&w=900&q=85",
            Location = "LivingRoom",
            Caption = "Contemporary living area",
            AltText = "One-bedroom near the harbor",
            DisplayOrder = 0,
            IsCoverPhoto = true,
            CapturedDate = Now,
            CapturedBy = SystemUser
        },
        new()
        {
            ListingPhotoID = new Guid("1f999999-9999-9999-9999-999999999999"),
            ListingID = Ids.ListingHarbor301,
            Url = "https://images.unsplash.com/photo-1600607687939-ce8a6c25118c?auto=format&fit=crop&w=900&q=85",
            Location = "Exterior",
            Caption = "Panoramic ocean and city view",
            AltText = "High-floor balcony view",
            DisplayOrder = 0,
            IsCoverPhoto = true,
            CapturedDate = Now,
            CapturedBy = SystemUser
        },
        new()
        {
            ListingPhotoID = new Guid("1faaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
            ListingID = Ids.ListingHarbor302,
            Url = "https://images.unsplash.com/photo-1505693416388-ac5ce068fe85?auto=format&fit=crop&w=900&q=85",
            Location = "Bedroom",
            Caption = "Bright bedroom with modern finishes",
            AltText = "Third-floor one-bedroom",
            DisplayOrder = 0,
            IsCoverPhoto = true,
            CapturedDate = Now,
            CapturedBy = SystemUser
        },
        new()
        {
            ListingPhotoID = new Guid("1fbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
            ListingID = Ids.ListingHarbor401,
            Url = "https://images.unsplash.com/photo-1600566753190-17f0baa2a6c3?auto=format&fit=crop&w=900&q=85",
            Location = "Exterior",
            Caption = "Penthouse rooftop view",
            AltText = "Penthouse panoramic view",
            DisplayOrder = 0,
            IsCoverPhoto = true,
            CapturedDate = Now,
            CapturedBy = SystemUser
        }
    };
    */

    /// <summary>Term-based pricing referencing listings.</summary>
    public static IReadOnlyList<ListingTermPrice> ListingTermPrices { get; } = new List<ListingTermPrice>
    {
        new()
        {
            ListingTermPriceID = new Guid("2f111111-1111-1111-1111-111111111111"),
            ListingID = Ids.ListingMaple101,
            LeaseTermMonths = 12,
            MonthlyRentAmount = 1850m,
            SecurityDepositAmount = 1850m,
            EffectiveFrom = Now,
            IsActive = true,
            CapturedDate = Now,
            CapturedBy = SystemUser
        },
        new()
        {
            ListingTermPriceID = new Guid("2f222222-2222-2222-2222-222222222222"),
            ListingID = Ids.ListingMaple101,
            LeaseTermMonths = 24,
            MonthlyRentAmount = 1780m,
            SecurityDepositAmount = 1780m,
            EffectiveFrom = Now,
            IsActive = true,
            CapturedDate = Now,
            CapturedBy = SystemUser
        },
        new()
        {
            ListingTermPriceID = new Guid("2f333333-3333-3333-3333-333333333333"),
            ListingID = Ids.ListingHarbor201,
            LeaseTermMonths = 6,
            MonthlyRentAmount = 3400m,
            SecurityDepositAmount = 3400m,
            EffectiveFrom = Now,
            IsActive = true,
            CapturedDate = Now,
            CapturedBy = SystemUser
        }
    };

    /// <summary>Access instructions referencing listings.</summary>
    public static IReadOnlyList<ListingAccessInstruction> ListingAccessInstructions { get; } = new List<ListingAccessInstruction>
    {
        new()
        {
            ListingAccessInstructionID = new Guid("3f111111-1111-1111-1111-111111111111"),
            ListingID = Ids.ListingMaple101,
            InstructionType = "LOCKBOX",
            Instructions = "Lockbox located to the right of the main entrance.",
            SecretReference = "vault://maple-101/lockbox",
            AvailableFrom = Now,
            AvailableUntil = Now.AddMonths(2),
            IsActive = true,
            CapturedDate = Now,
            CapturedBy = SystemUser
        },
        new()
        {
            ListingAccessInstructionID = new Guid("3f222222-2222-2222-2222-222222222222"),
            ListingID = Ids.ListingHarbor201,
            InstructionType = "CONCIERGE",
            Instructions = "Check in with the lobby concierge and present your booking reference.",
            AvailableFrom = Now,
            IsActive = true,
            CapturedDate = Now,
            CapturedBy = SystemUser
        }
    };

    /// <summary>Fee type lookup values.</summary>
    public static IReadOnlyList<FeeType> FeeTypes { get; } = new List<FeeType>
    {
        new() { Name = "RENT", IsPlatformFee = false, CapturedDate = Now, CapturedBy = SystemUser },
        new() { Name = "PARKING", IsPlatformFee = false, CapturedDate = Now, CapturedBy = SystemUser },
        new() { Name = "PET", IsPlatformFee = false, CapturedDate = Now, CapturedBy = SystemUser },
        new() { Name = "STORAGE", IsPlatformFee = false, CapturedDate = Now, CapturedBy = SystemUser },
        new() { Name = "UTILITIES", IsPlatformFee = false, CapturedDate = Now, CapturedBy = SystemUser },
        new() { Name = "LATE FEE", IsPlatformFee = false, CapturedDate = Now, CapturedBy = SystemUser },
        new() { Name = "REPAIR", IsPlatformFee = false, CapturedDate = Now, CapturedBy = SystemUser },
        new() { Name = "DISCOUNT", IsPlatformFee = false, CapturedDate = Now, CapturedBy = SystemUser },
        new() { Name = "CREDIT", IsPlatformFee = false, CapturedDate = Now, CapturedBy = SystemUser },
        new() { Name = "BOOKING FEE", IsPlatformFee = true, CapturedDate = Now, CapturedBy = SystemUser },
        new() { Name = "Tenant Placement Fee", IsPlatformFee = true, CapturedDate = Now, CapturedBy = SystemUser },
        new() { Name = "Host Placement Fee", IsPlatformFee = true, CapturedDate = Now, CapturedBy = SystemUser },
        new() { Name = "Host Active Unit Subscription Fee", IsPlatformFee = true, CapturedDate = Now, CapturedBy = SystemUser }
    };

    /// <summary>
    /// Sales tax rates for every Canadian province and territory. HST provinces have a single
    /// combined rate; the remaining jurisdictions charge federal GST (5%) plus, where applicable,
    /// a provincial sales tax (PST/RST) or Quebec's QST.
    /// </summary>
    public static IReadOnlyList<TaxRate> TaxRates { get; } = new List<TaxRate>
    {
        // ----- HST provinces (combined federal + provincial) -----
        NewTax("HST-ON", "Ontario HST", "ON", 0.13m),
        NewTax("HST-NB", "New Brunswick HST", "NB", 0.15m),
        NewTax("HST-NL", "Newfoundland and Labrador HST", "NL", 0.15m),
        NewTax("HST-NS", "Nova Scotia HST", "NS", 0.15m),
        NewTax("HST-PE", "Prince Edward Island HST", "PE", 0.15m),

        // ----- GST-only provinces and territories (5% federal) -----
        NewTax("GST-AB", "Alberta GST", "AB", 0.05m),
        NewTax("GST-NT", "Northwest Territories GST", "NT", 0.05m),
        NewTax("GST-NU", "Nunavut GST", "NU", 0.05m),
        NewTax("GST-YT", "Yukon GST", "YT", 0.05m),

        // ----- GST + provincial sales tax provinces -----
        NewTax("GST-BC", "British Columbia GST", "BC", 0.05m),
        NewTax("PST-BC", "British Columbia PST", "BC", 0.07m),

        NewTax("GST-MB", "Manitoba GST", "MB", 0.05m),
        NewTax("RST-MB", "Manitoba RST", "MB", 0.07m),

        NewTax("GST-SK", "Saskatchewan GST", "SK", 0.05m),
        NewTax("PST-SK", "Saskatchewan PST", "SK", 0.06m),

        NewTax("GST-QC", "Quebec GST", "QC", 0.05m),
        NewTax("QST-QC", "Quebec QST", "QC", 0.09975m)
    };

    /// <summary>Helper to build a Canadian <see cref="TaxRate"/> with the shared seed defaults.</summary>
    private static TaxRate NewTax(string code, string name, string provinceCode, decimal rate) => new()
    {
        Code = code,
        Name = name,
        CountryCode = "CA",
        ProvinceCode = provinceCode,
        Rate = rate,
        EffectiveFrom = Now.AddYears(-5),
        IsActive = true,
        CapturedDate = Now,
        CapturedBy = SystemUser
    };

    /// <summary>Amenity catalog lookup values.</summary>
    public static IReadOnlyList<AmenityCatalog> AmenityCatalogs { get; } = new List<AmenityCatalog>
    {
        new() { AmenityID = Ids.AmenityWifi, Name = "WiFi", Category = "Connectivity", IsActive = true, CapturedDate = Now, CapturedBy = SystemUser },
        new() { AmenityID = Ids.AmenityParking, Name = "Parking", Category = "Parking", IsActive = true, CapturedDate = Now, CapturedBy = SystemUser },
        new() { AmenityID = Ids.AmenityLaundry, Name = "In-Unit Laundry", Category = "Appliances", IsActive = true, CapturedDate = Now, CapturedBy = SystemUser },
        new() { AmenityID = Ids.AmenityAirConditioning, Name = "Air Conditioning", Category = "Climate", IsActive = true, CapturedDate = Now, CapturedBy = SystemUser },
        new() { AmenityID = Ids.AmenityHeating, Name = "Heating", Category = "Climate", IsActive = true, CapturedDate = Now, CapturedBy = SystemUser },
        new() { AmenityID = Ids.AmenityGym, Name = "Gym", Category = "Building", IsActive = true, CapturedDate = Now, CapturedBy = SystemUser },
        new() { AmenityID = Ids.AmenityPool, Name = "Swimming Pool", Category = "Building", IsActive = true, CapturedDate = Now, CapturedBy = SystemUser },
        new() { AmenityID = Ids.AmenityDishwasher, Name = "Dishwasher", Category = "Appliances", IsActive = true, CapturedDate = Now, CapturedBy = SystemUser },
        new() { AmenityID = Ids.AmenityBalcony, Name = "Balcony", Category = "Outdoor", IsActive = true, CapturedDate = Now, CapturedBy = SystemUser },
        new() { AmenityID = Ids.AmenityElevator, Name = "Elevator", Category = "Building", IsActive = true, CapturedDate = Now, CapturedBy = SystemUser }
    };

    /// <summary>Amenities associated with listings. Every listing receives a full amenity set.</summary>
    public static IReadOnlyList<ListingAmenity> ListingAmenities { get; } = BuildListingAmenities();

    private static List<ListingAmenity> BuildListingAmenities()
    {
        var amenityIds = new[]
        {
            Ids.AmenityWifi, Ids.AmenityParking, Ids.AmenityLaundry, Ids.AmenityAirConditioning,
            Ids.AmenityHeating, Ids.AmenityGym, Ids.AmenityPool, Ids.AmenityDishwasher,
            Ids.AmenityBalcony, Ids.AmenityElevator
        };

        var list = new List<ListingAmenity>();
        foreach (var spec in ListingSpecs)
        {
            foreach (var amenityId in amenityIds)
            {
                list.Add(new ListingAmenity
                {
                    ListingAmenityID = NewChildId(),
                    ListingID = spec.ListingId,
                    AmenityID = amenityId,
                    Notes = amenityId == Ids.AmenityWifi ? "Fibre internet included." : null,
                    CapturedDate = Now,
                    CapturedBy = SystemUser
                });
            }
        }

        return list;
    }

    /// <summary>Rules associated with listings. Every listing receives a consistent rule set.</summary>
    public static IReadOnlyList<ListingRule> ListingRules { get; } = BuildListingRules();

    private static List<ListingRule> BuildListingRules()
    {
        var list = new List<ListingRule>();
        foreach (var spec in ListingSpecs)
        {
            list.Add(new ListingRule { ListingRuleID = NewChildId(), ListingID = spec.ListingId, RuleType = "SMOKING", RuleTitle = "No Smoking", RuleDescription = "Smoking is not permitted anywhere on the premises.", IsAllowed = false, EffectiveFrom = Now, CapturedDate = Now, CapturedBy = SystemUser });
            list.Add(new ListingRule { ListingRuleID = NewChildId(), ListingID = spec.ListingId, RuleType = "PETS", RuleTitle = "Pets", RuleDescription = "Small pets considered with an additional deposit.", IsAllowed = true, EffectiveFrom = Now, CapturedDate = Now, CapturedBy = SystemUser });
            list.Add(new ListingRule { ListingRuleID = NewChildId(), ListingID = spec.ListingId, RuleType = "QUIET_HOURS", RuleTitle = "Quiet Hours", RuleDescription = "Quiet hours between 10pm and 7am.", IsAllowed = true, EffectiveFrom = Now, CapturedDate = Now, CapturedBy = SystemUser });
            list.Add(new ListingRule { ListingRuleID = NewChildId(), ListingID = spec.ListingId, RuleType = "PARTIES", RuleTitle = "No Parties or Events", RuleDescription = "Parties and events are not permitted.", IsAllowed = false, EffectiveFrom = Now, CapturedDate = Now, CapturedBy = SystemUser });
        }

        return list;
    }

    /// <summary>Policies associated with listings. Every listing receives a policy record.</summary>
    public static IReadOnlyList<ListingPolicy> ListingPolicies { get; } = BuildListingPolicies();

    private static List<ListingPolicy> BuildListingPolicies()
    {
        var list = new List<ListingPolicy>();
        foreach (var spec in ListingSpecs)
        {
            list.Add(new ListingPolicy
            {
                ListingPolicyID = NewChildId(),
                ListingID = spec.ListingId,
                AllowsPets = true,
                AllowsSmoking = false,
                AllowsChildren = true,
                MaximumOccupants = Math.Max(1, spec.Bedrooms * 2),
                Furnished = spec.LeaseTermMonths <= 6,
                ParkingIncluded = true,
                UtilitiesIncluded = spec.LeaseTermMonths <= 6,
                MinimumCreditScore = 650,
                RequiresBackgroundCheck = true,
                ApplicationInstructions = "Submit proof of income and references.",
                CapturedDate = Now,
                CapturedBy = SystemUser
            });
        }

        return list;
    }

    /// <summary>
    /// Calendar events associated with listings. Each listing gets a confirmed booking block
    /// (so future dates are correctly excluded) plus a non-blocking viewing event.
    /// </summary>
    public static IReadOnlyList<CalendarEvent> CalendarEvents { get; } = BuildCalendarEvents();

    private static List<CalendarEvent> BuildCalendarEvents()
    {
        var list = new List<CalendarEvent>();
        int i = 0;
        foreach (var spec in ListingSpecs)
        {
            var blockStart = Now.AddDays(20 + (i * 3));
            list.Add(new CalendarEvent
            {
                CalendarEventID = NewChildId(),
                ListingID = spec.ListingId,
                LeaseID = spec.LeaseId,
                EventType = "LEASE",
                Status = "CONFIRMED",
                StartAt = blockStart,
                EndAt = blockStart.AddMonths(spec.LeaseTermMonths),
                IsAllDay = true,
                Title = "Booked lease term",
                OccupantName = spec.TenantId == Ids.TenantAlice ? "Alice Tenant" : "Bob Tenant",
                OccupantCount = Math.Max(1, spec.Bedrooms),
                BlocksAvailability = true,
                CapturedDate = Now,
                CapturedBy = SystemUser
            });

            list.Add(new CalendarEvent
            {
                CalendarEventID = NewChildId(),
                ListingID = spec.ListingId,
                EventType = "VIEWING",
                Status = "CONFIRMED",
                StartAt = Now.AddDays(3 + i).AddHours(10),
                EndAt = Now.AddDays(3 + i).AddHours(11),
                IsAllDay = false,
                Title = "Open house viewing",
                BlocksAvailability = false,
                CapturedDate = Now,
                CapturedBy = SystemUser
            });

            i++;
        }

        return list;
    }


    /// <summary>
    /// Tenants referencing existing application users. Seeded separately from the core listing
    /// graph because they depend on <see cref="User"/> rows that may not exist.
    /// </summary>
    public static IReadOnlyList<Tenant> Tenants { get; } = new List<Tenant>
    {
        new()
        {
            TenantID = Ids.TenantAlice,
            Code = "TEN-0001",
            UserID = 1,
            Description = "Long-term tenant at Maple Grove.",
            PhoneNumber = "+1-416-555-0101",
            ProfileStatus = "ACTIVE",
            IsActive = true,
            CapturedDate = Now,
            CapturedBy = SystemUser
        },
        new()
        {
            TenantID = Ids.TenantBob,
            Code = "TEN-0002",
            UserID = 1,
            Description = "Short-term tenant at Harbor View.",
            PhoneNumber = "+1-604-555-0202",
            ProfileStatus = "ACTIVE",
            IsActive = true,
            CapturedDate = Now,
            CapturedBy = SystemUser
        }
    };

    /// <summary>
    /// Leases linking tenants to listings. Seeded separately because they depend on
    /// <see cref="Tenant"/> rows which in turn reference application users. Every listing has
    /// a lease so it can carry reviews and calendar-based availability exclusions.
    /// </summary>
    public static IReadOnlyList<Lease> Leases { get; } = BuildLeases();

    private static List<Lease> BuildLeases()
    {
        var list = new List<Lease>();
        int i = 1;
        foreach (var spec in ListingSpecs)
        {
            list.Add(new Lease
            {
                LeaseID = spec.LeaseId,
                OrganizationID = spec.OrgId,
                ListingID = spec.ListingId,
                RentalUnitID = spec.UnitId,
                TenancyTypeID = 1,
                TenantID = spec.TenantId,
                LeaseCode = $"LSE-{i:0000}",
                LeaseNumber = $"AR-2024-{i:0000}",
                Status = "ACTIVE",
                StartDate = Now.AddMonths(-6),
                EndDate = Now.AddMonths(spec.LeaseTermMonths - 6),
                LeaseTermMonths = (short)spec.LeaseTermMonths,
                BaseRentAmount = spec.Rent,
                Currency = "CAD",
                GracePeriodDays = 5,
                AutoRenew = spec.LeaseTermMonths >= 12,
                ActivatedAt = Now.AddMonths(-6),
                CapturedDate = Now,
                CapturedBy = SystemUser
            });
            i++;
        }

        return list;
    }

    /// <summary>
    /// Ratings referencing leases. Seeded separately because they depend on <see cref="Lease"/>
    /// and <see cref="User"/> rows. Each listing gets 18-22 individual reviews with distinct
    /// reviewer names, dates, stay context and category ratings so highly-rated listings qualify
    /// as "Renter favourite".
    /// </summary>
    public static IReadOnlyList<Rating> Ratings { get; } = BuildRatings();

    private static List<Rating> BuildRatings()
    {
        var list = new List<Rating>();
        int specIndex = 0;
        foreach (var spec in ListingSpecs)
        {
            // 18 reviews per listing, mostly 5-star so the average stays >= 4.8 to earn the
            // "Renter favourite" badge on the detail page. Reviewer name, location and stay
            // context are packed into the review body since the Rating schema has no dedicated
            // columns for them.
            int reviewCount = 18;
            for (int r = 0; r < reviewCount; r++)
            {
                int seed = (specIndex * 31) + r;
                short overall = (short)(r % 6 == 3 ? 4 : 5);
                var name = ReviewerNames[seed % ReviewerNames.Length];
                var location = ReviewerLocations[seed % ReviewerLocations.Length];
                var stay = StayContexts[seed % StayContexts.Length];
                var body = ReviewBodies[seed % ReviewBodies.Length];

                list.Add(new Rating
                {
                    RatingID = NewChildId(),
                    LeaseID = spec.LeaseId,
                    ReviewerUserID = 1,
                    SubjectType = "LISTING",
                    SubjectReferenceID = spec.ListingId.ToString(),
                    OverallRating = overall,
                    CommunicationRating = overall,
                    PropertyCareRating = overall,
                    CleanlinessRating = (short)Math.Min(5, overall + (r % 2)),
                    AccuracyRating = overall,
                    ResponsivenessRating = overall,
                    ReviewBody = Truncate($"{name} · {location} · {stay}. {body}", 256),
                    IsPublic = true,
                    PublishedAt = Now.AddDays(-(5 + (r * 3))),
                    CapturedDate = Now,
                    CapturedBy = SystemUser
                });
            }

            specIndex++;
        }

        return list;
    }
}
