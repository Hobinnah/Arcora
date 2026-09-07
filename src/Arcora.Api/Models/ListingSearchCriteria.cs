namespace Arcora.Api.Models
{
    /// <summary>
    /// Airbnb-style search criteria for finding available listings. All fields are optional so
    /// callers can search on any combination of location, stay window, lease length and party size.
    /// </summary>
    public class ListingSearchCriteria
    {
        /// <summary>
        /// Free-text location filter ("Where"). Matches city, neighborhood/address line, province
        /// or the listing title.
        /// </summary>
        public string? Where { get; set; }

        /// <summary>
        /// Desired stay length in months ("Stay length"). Combined with <see cref="MoveInDate"/>
        /// to compute the requested occupancy window used for availability filtering.
        /// </summary>
        public int? StayLengthMonths { get; set; }

        /// <summary>
        /// Desired move-in date ("Move-in date"). Listings must be available on or before this date.
        /// </summary>
        public DateTime? MoveInDate { get; set; }

        /// <summary>
        /// Desired lease length in months ("Lease length"). Must fall within the listing's
        /// minimum/maximum lease months.
        /// </summary>
        public int? LeaseLengthMonths { get; set; }

        /// <summary>
        /// Number of renters/guests moving in ("Who's moving in"). Must not exceed the unit capacity.
        /// </summary>
        public int? Renters { get; set; }

        /// <summary>Optional minimum monthly rent filter.</summary>
        public decimal? MinRent { get; set; }

        /// <summary>Optional maximum monthly rent filter.</summary>
        public decimal? MaxRent { get; set; }

        /// <summary>Optional minimum number of bedrooms.</summary>
        public decimal? MinBedrooms { get; set; }

        /// <summary>Optional furnished-only filter.</summary>
        public bool? IsFurnished { get; set; }

        /// <summary>Optional pet-friendly-only filter.</summary>
        public bool? IsPetFriendly { get; set; }

        /// <summary>1-based page number.</summary>
        public int PageNumber { get; set; } = 1;

        /// <summary>Page size (capped by the repository).</summary>
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// Sort order for results. Supported: "price_asc", "price_desc", "newest" (default).
        /// </summary>
        public string? SortBy { get; set; }
    }
}
