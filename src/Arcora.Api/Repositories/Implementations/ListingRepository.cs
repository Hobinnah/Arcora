// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;
using Arcora.Api.Models;
using Arcora.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Arcora.Api.DTOs;
using Arcora.Api.Helpers;

namespace Arcora.Api.Repositories.Implementations
{
    public class ListingRepository : Repository<Listing>, IListingRepository //, IDisposable
    {
        private const int MaxPageSize = 50;

        private readonly ArcoraDbContext context;
        public ListingRepository(ArcoraDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<Listing>> GetListingsAsync()
        {
            // Fetching all listings loads navigation data only, to keep the operation efficient.
            return await ApplyDefaultOrder(this.context.Listings.AsNoTracking()
                .Include(x => x.RentalUnit)!.ThenInclude(u => u!.Property)!.ThenInclude(p => p!.Address)
                .Include(x => x.RentalUnit)!.ThenInclude(u => u!.UnitType)
                .Include(x => x.ListingType)
                .Include(x => x.Organization)
                .Include(x => x.ListingPhotos)
                ).ToListAsync();
        }

        public async Task<Listing?> GetListingAsync(Guid id)
        {
            // Fetching a single listing eagerly loads the child collections in addition
            // to the navigation data.
            return await this.context.Listings.AsNoTracking()
                .Include(x => x.RentalUnit)!.ThenInclude(u => u!.Property)!.ThenInclude(p => p!.Address)
                .Include(x => x.RentalUnit)!.ThenInclude(u => u!.UnitType)
                .Include(x => x.ListingType)
                .Include(x => x.Organization)
                .Include(x => x.ListingTermPrices)
                .Include(x => x.ListingAccessInstructions)
                .Include(x => x.ListingPhotos)
                .Include(x => x.ListingAmenities)!.ThenInclude(a => a!.AmenityCatalog)
                .Include(x => x.ListingRules)
                .Include(x => x.ListingPolicies)
                .Include(x => x.CalendarEvents)
                .Include(x => x.Leases)!.ThenInclude(l => l!.Ratings)!.ThenInclude(r => r!.ReviewerUser)
                .Include(x => x.Leases)!.ThenInclude(l => l!.Tenant)
                .Include(x => x.Leases)!.ThenInclude(l => l!.RentalUnit)!.ThenInclude(u => u!.Property)!.ThenInclude(p => p!.Address)
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.ListingID == id);
        }

        public async Task<bool> HasListingsAsync()
        {
            return await this.context.Set<Listing>().AnyAsync();
        }

        public async Task<List<Listing>> GetListingsByOrganizationAsync(Guid organizationId)
        {
            // Fetching listings for an organization loads navigation data only, to keep the
            // operation efficient and consistent with GetListingsAsync.
            return await ApplyDefaultOrder(this.context.Listings.AsNoTracking()
                .Where(x => x.OrganizationID == organizationId)
                .Include(x => x.RentalUnit)!.ThenInclude(u => u!.Property)!.ThenInclude(p => p!.Address)
                .Include(x => x.RentalUnit)!.ThenInclude(u => u!.UnitType)
                .Include(x => x.ListingType)
                .Include(x => x.Organization)
                .Include(x => x.ListingPhotos)
                ).ToListAsync();
        }

        public async Task<int> CountListingsByOrganizationAsync(Guid organizationId)
        {
            return await this.context.Listings.AsNoTracking()
                .CountAsync(x => x.OrganizationID == organizationId);
        }

        /// <inheritdoc/>
        public async Task<(List<Listing> Items, int TotalCount)> SearchListingsAsync(ListingSearchCriteria criteria)
        {
            criteria ??= new ListingSearchCriteria();
            var query = BuildSearchQuery(criteria);
            var totalCount = await query.CountAsync();
            query = criteria.SortBy?.ToLowerInvariant() switch
            {
                "price_asc" => query.OrderBy(x => x.BaseMonthlyRentAmount).ThenBy(x => x.ListingID),
                "price_desc" => query.OrderByDescending(x => x.BaseMonthlyRentAmount).ThenBy(x => x.ListingID),
                _ => query.OrderByDescending(x => x.PublishedAt).ThenBy(x => x.ListingID)
            };
            var pageNumber = Math.Max(criteria.PageNumber, 1);
            var pageSize = criteria.PageSize < 1 ? 10 : Math.Min(criteria.PageSize, MaxPageSize);
            var items = await query
                .Include(x => x.RentalUnit)!.ThenInclude(u => u!.Property)!.ThenInclude(p => p!.Address)
                .Include(x => x.RentalUnit)!.ThenInclude(u => u!.UnitType)
                .Include(x => x.ListingType)
                .Include(x => x.Organization)
                .Include(x => x.ListingPhotos)
                .Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            return (items, totalCount);
        }

        public async Task<ListingMapResultDto> SearchMapAsync(ListingSearchCriteria criteria, CancellationToken cancellationToken)
        {
            var query = BuildSearchQuery(criteria);
            var rows = await query.OrderBy(x => x.ListingID)
                .Select(x => new
                {
                    x.ListingID, x.Title, x.BaseMonthlyRentAmount, x.Currency,
                    Latitude = x.RentalUnit == null || x.RentalUnit.Property == null || x.RentalUnit.Property.Address == null ? null : x.RentalUnit.Property.Address.Latitude,
                    Longitude = x.RentalUnit == null || x.RentalUnit.Property == null || x.RentalUnit.Property.Address == null ? null : x.RentalUnit.Property.Address.Longitude
                }).ToListAsync(cancellationToken);
            var pins = new List<ListingMapPinDto>();
            foreach (var row in rows)
            {
                var location = PublicMapLocation.From(row.Latitude, row.Longitude);
                if (location != null)
                    pins.Add(new(row.ListingID, row.Title, location.Latitude, location.Longitude, row.BaseMonthlyRentAmount, row.Currency));
            }
            return new(pins, rows.Count, rows.Count - pins.Count);
        }

        private static IQueryable<Listing> WithValidCoordinates(IQueryable<Listing> query) =>
            query.Where(x => x.RentalUnit != null && x.RentalUnit.Property != null && x.RentalUnit.Property.Address != null &&
                x.RentalUnit.Property.Address.Latitude >= -90 && x.RentalUnit.Property.Address.Latitude <= 90 &&
                x.RentalUnit.Property.Address.Longitude >= -180 && x.RentalUnit.Property.Address.Longitude <= 180);

        private IQueryable<Listing> BuildSearchQuery(ListingSearchCriteria criteria)
        {

            // Start from a composable, no-tracking query. Every filter below is applied on the
            // IQueryable so the whole search (including availability and paging) is translated to
            // a single SQL statement and executed server-side.
            IQueryable<Listing> query = this.context.Listings.AsNoTracking();

            // Only approved listings are visible through the public marketplace search.
            query = query.Where(x => x.Status != null &&
                (x.Status.ToUpper() == "APPROVED" || x.Status.ToUpper() == "PUBLISHED"));

            // "Where": match city, address lines, postal code, province (code or full name) or title.
            if (!string.IsNullOrWhiteSpace(criteria.Where))
            {
                var term = criteria.Where.Trim();

                // Resolve any province/state whose full name matches the term so a search for
                // e.g. "Washington" or "Ontario" hits the stored ProvinceCode ("WA"/"ON").
                var provinceCodes = ResolveProvinceCodes(term);

                query = query.Where(x =>
                    (x.Title != null && x.Title.Contains(term)) ||
                    (x.RentalUnit != null && x.RentalUnit.Property != null && x.RentalUnit.Property.Address != null &&
                        ((x.RentalUnit.Property.Address.City != null && x.RentalUnit.Property.Address.City.Contains(term)) ||
                         (x.RentalUnit.Property.Address.ProvinceCode != null &&
                            (x.RentalUnit.Property.Address.ProvinceCode.Contains(term) ||
                             provinceCodes.Contains(x.RentalUnit.Property.Address.ProvinceCode))))));
            }

            // "Move-in date": the listing must be available on or before the requested move-in.
            if (criteria.MoveInDate.HasValue)
            {
                var moveIn = criteria.MoveInDate.Value.Date;
                query = query.Where(x => x.AvailableFrom == null || x.AvailableFrom <= moveIn);

                // The occupancy window is [moveIn, moveIn + stay). If the listing has an
                // AvailableTo, the requested window must fit inside it.
                var stayMonths = criteria.StayLengthMonths ?? criteria.LeaseLengthMonths ?? 1;
                if (stayMonths > 0)
                {
                    var moveOut = moveIn.AddMonths(stayMonths);
                    query = query.Where(x => x.AvailableTo == null || x.AvailableTo >= moveOut);

                    // Exclude listings whose calendar has an availability-blocking event that
                    // overlaps the requested window. Uses the CalendarEvents navigation so it
                    // translates to a correlated EXISTS within the same SQL command (avoiding a
                    // second open DataReader on the connection).
                    query = query.Where(x => !x.CalendarEvents!.Any(ce =>
                        ce.BlocksAvailability &&
                        !ArcoraDbContext.NonBlockingCalendarEventStatuses.Contains((ce.Status ?? "").ToUpper()) &&
                        ce.StartAt < moveOut &&
                        ce.EndAt > moveIn));
                    query = query.Where(x => !context.Leases.Any(lease => (lease.ListingID == x.ListingID || lease.RentalUnitID == x.RentalUnitID) &&
                        !ArcoraDbContext.NonBlockingLeaseStatuses.Contains((lease.Status ?? "").ToUpper()) &&
                        lease.StartDate < moveOut && (lease.EndDate == null || lease.EndDate > moveIn)));
                    var now = DateTime.UtcNow;
                    query = query.Where(x => !context.ReservationHolds.Any(hold => hold.ListingID == x.ListingID &&
                        !ArcoraDbContext.NonBlockingHoldStatuses.Contains((hold.Status ?? "").ToUpper()) &&
                        hold.ReleasedAt == null && hold.ExpiresAt > now && hold.StartDate < moveOut && hold.EndDate > moveIn));
                }
            }

            // "Lease length": must fall within the listing's allowed min/max lease months.
            if (criteria.StayLengthMonths.HasValue && criteria.StayLengthMonths.Value > 0)
            {
                var lease = (short)criteria.StayLengthMonths.Value;
                query = query.Where(x => x.MinimumLeaseMonths <= lease && x.MaximumLeaseMonths >= lease);
            }

            // "Who's moving in": party size must not exceed the unit's maximum occupancy.
            if (criteria.Renters.HasValue && criteria.Renters.Value > 0)
            {
                var renters = criteria.Renters.Value;
                query = query.Where(x => x.RentalUnit == null || x.RentalUnit.MaximumOccupants == null || x.RentalUnit.MaximumOccupants >= renters);
            }

            // Optional refinement filters.
            if (criteria.MinRent.HasValue)
                query = query.Where(x => x.BaseMonthlyRentAmount >= criteria.MinRent.Value);
            if (criteria.MaxRent.HasValue)
                query = query.Where(x => x.BaseMonthlyRentAmount <= criteria.MaxRent.Value);
            if (criteria.MinBedrooms.HasValue)
                query = query.Where(x => x.Bedrooms != null && x.Bedrooms >= criteria.MinBedrooms.Value);
            if (criteria.IsFurnished.HasValue)
                query = query.Where(x => x.IsFurnished == criteria.IsFurnished.Value);
            if (criteria.IsPetFriendly.HasValue)
                query = query.Where(x => x.IsPetFriendly == criteria.IsPetFriendly.Value);

            if (criteria.UnitTypeID.HasValue)
                query = query.Where(x => x.RentalUnit != null && x.RentalUnit.UnitTypeID == criteria.UnitTypeID);
            if (criteria.South.HasValue && criteria.North.HasValue && criteria.West.HasValue && criteria.East.HasValue)
            {
                // Filter on the same public cell centers, never on the private coordinates.
                var projected = WithValidCoordinates(query).Select(x => new
                {
                    Listing = x,
                    Lat = Math.Floor((x.RentalUnit!.Property!.Address!.Latitude == 90 ? 89.999 : (double)x.RentalUnit.Property.Address.Latitude!.Value) * 100) / 100 + 0.005,
                    Lon = Math.Floor((x.RentalUnit.Property.Address.Longitude == 180 ? 179.999 : (double)x.RentalUnit.Property.Address.Longitude!.Value) * 100) / 100 + 0.005
                });
                projected = projected.Where(x => x.Lat >= criteria.South && x.Lat <= criteria.North &&
                    (criteria.West < criteria.East
                        ? x.Lon >= criteria.West && x.Lon <= criteria.East
                        : x.Lon >= criteria.West || x.Lon <= criteria.East));
                query = projected.Select(x => x.Listing);
            }
            return query;
        }

        /// <summary>
        /// Maps a free-text province/state <b>name</b> to the matching stored province code(s)
        /// so a search for e.g. "Washington" resolves to "WA" and "Ontario" resolves to "ON".
        /// The match is case-insensitive and also accepts a partial name (e.g. "British" -> "BC").
        /// Returns an empty list when nothing matches.
        /// </summary>
        private static List<string> ResolveProvinceCodes(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return new List<string>();

            return ProvinceNameToCode
                .Where(kv => kv.Key.Contains(term, StringComparison.OrdinalIgnoreCase))
                .Select(kv => kv.Value)
                .Distinct()
                .ToList();
        }

        /// <summary>
        /// Full province/state name to ISO code map covering Canadian provinces/territories and
        /// US states, used to translate a typed location name into the stored province code.
        /// </summary>
        private static readonly IReadOnlyDictionary<string, string> ProvinceNameToCode =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                // Canadian provinces and territories.
                ["Alberta"] = "AB",
                ["British Columbia"] = "BC",
                ["Manitoba"] = "MB",
                ["New Brunswick"] = "NB",
                ["Newfoundland and Labrador"] = "NL",
                ["Northwest Territories"] = "NT",
                ["Nova Scotia"] = "NS",
                ["Nunavut"] = "NU",
                ["Ontario"] = "ON",
                ["Prince Edward Island"] = "PE",
                ["Quebec"] = "QC",
                ["Saskatchewan"] = "SK",
                ["Yukon"] = "YT",

                // US states and DC.
                ["Alabama"] = "AL",
                ["Alaska"] = "AK",
                ["Arizona"] = "AZ",
                ["Arkansas"] = "AR",
                ["California"] = "CA",
                ["Colorado"] = "CO",
                ["Connecticut"] = "CT",
                ["Delaware"] = "DE",
                ["District of Columbia"] = "DC",
                ["Florida"] = "FL",
                ["Georgia"] = "GA",
                ["Hawaii"] = "HI",
                ["Idaho"] = "ID",
                ["Illinois"] = "IL",
                ["Indiana"] = "IN",
                ["Iowa"] = "IA",
                ["Kansas"] = "KS",
                ["Kentucky"] = "KY",
                ["Louisiana"] = "LA",
                ["Maine"] = "ME",
                ["Maryland"] = "MD",
                ["Massachusetts"] = "MA",
                ["Michigan"] = "MI",
                ["Minnesota"] = "MN",
                ["Mississippi"] = "MS",
                ["Missouri"] = "MO",
                ["Montana"] = "MT",
                ["Nebraska"] = "NE",
                ["Nevada"] = "NV",
                ["New Hampshire"] = "NH",
                ["New Jersey"] = "NJ",
                ["New Mexico"] = "NM",
                ["New York"] = "NY",
                ["North Carolina"] = "NC",
                ["North Dakota"] = "ND",
                ["Ohio"] = "OH",
                ["Oklahoma"] = "OK",
                ["Oregon"] = "OR",
                ["Pennsylvania"] = "PA",
                ["Rhode Island"] = "RI",
                ["South Carolina"] = "SC",
                ["South Dakota"] = "SD",
                ["Tennessee"] = "TN",
                ["Texas"] = "TX",
                ["Utah"] = "UT",
                ["Vermont"] = "VT",
                ["Virginia"] = "VA",
                ["Washington"] = "WA",
                ["West Virginia"] = "WV",
                ["Wisconsin"] = "WI",
                ["Wyoming"] = "WY"
            };
    }
}