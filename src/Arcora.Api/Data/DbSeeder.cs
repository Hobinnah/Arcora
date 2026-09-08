using Arcora.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Data;

/// <summary>
/// Development-time seeder that pushes the in-memory <see cref="SeedData"/> graph into the
/// database when the core listing tables are empty. Entities are inserted parent-first so that
/// foreign keys line up. This is intended for local/development use only.
/// </summary>
public static class DbSeeder
{
    /// <summary>
    /// Seeds the listing domain into the database when no listings exist yet.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <param name="logger">Logger for seed diagnostics.</param>
    /// <param name="forceReseed">
    /// When <c>true</c>, existing seeded rows are cleared first so the current
    /// <see cref="SeedData"/> graph is re-inserted. Intended for local/development use only.
    /// </param>
    public static async Task SeedAsync(ArcoraDbContext context, ILogger logger, bool forceReseed = false)
    {
        try
        {
            if (forceReseed)
            {
                await ClearSeededDataAsync(context, logger);
            }

            if (await context.Listings.AnyAsync())
            {
                logger.LogInformation("Seed skipped: listings already exist.");
            }
            else
            {
                // Parents first so foreign keys resolve.
                await AddIfEmptyAsync(context, context.Organizations, SeedData.Organizations);
                await AddIfEmptyAsync(context, context.Addresses, SeedData.Addresses);
                await AddIfEmptyAsync(context, context.Properties, SeedData.Properties);
                await AddIfEmptyAsync(context, context.UnitTypes, SeedData.UnitTypes);
                await AddIfEmptyAsync(context, context.ListingTypes, SeedData.ListingTypes);
                await AddIfEmptyAsync(context, context.TenancyTypes, SeedData.TenancyTypes);
                await AddIfEmptyAsync(context, context.RentalUnits, SeedData.RentalUnits);

                // Independent lookup tables.
                await AddIfEmptyAsync(context, context.FeeTypes, SeedData.FeeTypes);
                await AddIfEmptyAsync(context, context.TaxRates, SeedData.TaxRates);
                await AddIfEmptyAsync(context, context.AmenityCatalogs, SeedData.AmenityCatalogs);

                // Listings and their child collections.
                context.Listings.AddRange(SeedData.Listings);
                context.ListingPhotos.AddRange(SeedData.ListingPhotos);
                context.ListingTermPrices.AddRange(SeedData.ListingTermPrices);
                context.ListingAccessInstructions.AddRange(SeedData.ListingAccessInstructions);
                context.ListingAmenities.AddRange(SeedData.ListingAmenities);
                context.ListingRules.AddRange(SeedData.ListingRules);
                context.ListingPolicies.AddRange(SeedData.ListingPolicies);

                await context.SaveChangesAsync();
                logger.LogInformation("Seed complete: inserted {Count} listings.", SeedData.Listings.Count);
            }

            // Organization members, tenants, leases, ratings and lease-linked calendar events
            // reference existing application users; seed them separately so a missing user
            // cannot roll back the core listing graph. These run independently of the core
            // listing seed so they can still populate once the referenced users exist.
            await SeedOrganizationMembersAsync(context, logger);
            await SeedTenancyGraphAsync(context, logger);
            await SeedPlatformFeesAsync(context, logger);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database.");
        }
    }

    /// <summary>
    /// Seeds a global platform booking fee (5% of first month's rent) charged when a booking is made
    /// through this platform. The fee references the identity-generated "BOOKING FEE" platform fee type
    /// by name, so it is seeded after the fee types are persisted. Idempotent.
    /// </summary>
    private static async Task SeedPlatformFeesAsync(ArcoraDbContext context, ILogger logger)
    {
        try
        {
            var bookingFeeType = await context.FeeTypes
                .FirstOrDefaultAsync(ft => ft.Name == "BOOKING FEE" && ft.IsPlatformFee == true);

            if (bookingFeeType == null)
            {
                // The fee type may not exist yet on an already-seeded database (the core seed block is
                // skipped once listings exist). Create it here so the platform fee can reference it.
                bookingFeeType = new FeeType
                {
                    Name = "BOOKING FEE",
                    IsPlatformFee = true,
                    CapturedDate = DateTime.UtcNow,
                    CapturedBy = "SYSTEM_SEED"
                };
                context.FeeTypes.Add(bookingFeeType);
                await context.SaveChangesAsync();
            }

            if (await context.Fees.AnyAsync(f => f.FeeTypeID == bookingFeeType.FeeTypeID))
            {
                return;
            }

            context.Fees.Add(new Fee
            {
                FeeID = Guid.NewGuid(),
                FeeTypeID = bookingFeeType.FeeTypeID,
                OrganizationID = null, // Global platform fee applies to all organizations.
                Code = "PLATFORM-BOOKING-5PCT",
                Name = "Platform Booking Fee",
                CalculationType = "PERCENTAGE",
                PercentageRate = 5m,
                Currency = "CAD",
                IsTaxable = false,
                EffectiveFrom = DateTime.UtcNow.AddYears(-1),
                IsActive = true,
                CapturedDate = DateTime.UtcNow,
                CapturedBy = "SYSTEM_SEED"
            });

            await context.SaveChangesAsync();
            logger.LogInformation("Seed complete: inserted platform booking fee (5%).");
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Skipped seeding platform booking fee.");
        }
    }

    private static async Task SeedOrganizationMembersAsync(ArcoraDbContext context, ILogger logger)
    {
        try
        {
            if (await context.OrganizationMembers.AnyAsync())
                return;
            context.OrganizationMembers.AddRange(SeedData.OrganizationMembers);
            await context.SaveChangesAsync();
            logger.LogInformation("Seed complete: inserted organization members.");
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Skipped seeding organization members (referenced users may not exist).");
        }
    }

    /// <summary>
    /// Seeds tenants, leases, ratings and calendar events. These depend on application users,
    /// so failures here are logged as warnings and do not affect the core listing graph.
    /// </summary>
    private static async Task SeedTenancyGraphAsync(ArcoraDbContext context, ILogger logger)
    {
        try
        {
            if (await context.Tenants.AnyAsync() || await context.Leases.AnyAsync())
                return;

            context.Tenants.AddRange(SeedData.Tenants);
            context.Leases.AddRange(SeedData.Leases);
            context.Ratings.AddRange(SeedData.Ratings);
            context.CalendarEvents.AddRange(SeedData.CalendarEvents);
            await context.SaveChangesAsync();
            logger.LogInformation("Seed complete: inserted tenants, leases, ratings and calendar events.");
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Skipped seeding tenancy graph (referenced users may not exist).");
        }
    }

    private static async Task AddIfEmptyAsync<TEntity>(
        ArcoraDbContext context,
        DbSet<TEntity> set,
        IReadOnlyList<TEntity> items) where TEntity : class
    {
        if (await set.AnyAsync())
            return;

        set.AddRange(items);
    }

    /// <summary>
    /// Clears the seeded listing domain so it can be re-inserted from the current
    /// <see cref="SeedData"/> graph. Children are removed before parents to respect foreign
    /// keys. Lookup tables (organizations, rental units, properties, catalogs) are left intact
    /// because the seeder skips them when they already exist.
    /// </summary>
    private static async Task ClearSeededDataAsync(ArcoraDbContext context, ILogger logger)
    {
        try
        {
            // Child records first, then parents, to satisfy foreign-key constraints.
            context.CalendarEvents.RemoveRange(context.CalendarEvents);
            context.Ratings.RemoveRange(context.Ratings);
            context.Leases.RemoveRange(context.Leases);
            context.Tenants.RemoveRange(context.Tenants);
            context.ListingPolicies.RemoveRange(context.ListingPolicies);
            context.ListingRules.RemoveRange(context.ListingRules);
            context.ListingAmenities.RemoveRange(context.ListingAmenities);
            context.ListingPhotos.RemoveRange(context.ListingPhotos);
            context.ListingAccessInstructions.RemoveRange(context.ListingAccessInstructions);
            context.ListingTermPrices.RemoveRange(context.ListingTermPrices);
            context.Listings.RemoveRange(context.Listings);
            context.OrganizationMembers.RemoveRange(context.OrganizationMembers);

            await context.SaveChangesAsync();
            logger.LogInformation("Force reseed: cleared existing seeded listing data.");
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Force reseed: failed to clear existing seeded data.");
        }
    }
}
