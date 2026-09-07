// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;
using Arcora.Api.Entities;

namespace Arcora.Api.DTOs;
/// <summary>
/// Represents rental property listings managed by organizations.
/// </summary>
public class ListingDto
{
    /// <summary>
    /// Key
    /// </summary>
    public Guid? ListingID { get; set; }

    /// <summary>
    /// FK to RentalUnit
    /// </summary>
    [Required]
    public Guid? RentalUnitID { get; set; }

    /// <summary>
    /// FK to ListingType
    /// </summary>
    [Required]
    public Guid? ListingTypeID { get; set; }

    /// <summary>
    /// FK to Organization
    /// </summary>
    [Required]
    public Guid? OrganizationID { get; set; }

    /// <summary>
    /// Listing title
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string? Title { get; set; }

    /// <summary>
    /// Listing description
    /// </summary>
    [MaxLength(1000)]
    public string? Description { get; set; }

    /// <summary>
    /// Door code for check-in
    /// </summary>
    [MaxLength(10)]
    public string? CheckInDoorCode { get; set; }

    public string? Location
    {
        get
        {
            if (RentalUnit != null && RentalUnit.Property != null && RentalUnit.Property.Address != null)
            {
                return $"{RentalUnit.Property.Address.City}, {Helpers.RegionNames.GetName(RentalUnit.Property.Address.ProvinceCode)}";
            }
            return null;
        }
        set { /* intentionally left blank to allow model binding */ }
    }

    /// <summary>
    /// Additional details about the listing . "1 bed · 1 bath · Furnished"
    /// </summary>
    public string? Details
    {
        get
        {
            if (Bedrooms > 0 && Bathrooms > 0)
            {
                return $"{Bedrooms.ToString()} bed(s) · {Bathrooms.ToString()} bath(s) . {(IsFurnished ? "Furnished" : "Unfurnished")} . {(IsPetFriendly ? "Pet Friendly" : "No Pets")}";
            }

            return "1 bed · 1 bath · Unfurnished · No Pets";
        }
        set { /* intentionally left blank to allow model binding */ }
    }

    /// <summary>
    /// Price of the listing. Derived from the active term price when available,
    /// otherwise falls back to the base monthly rent amount.
    /// </summary>
    public decimal? Price
    {
        get => BaseMonthlyRentAmount > 0 ? BaseMonthlyRentAmount : _price;
        set => _price = value ?? 0;
    }
    private decimal? _price = 0;

    /// <summary>
    /// Type of the listing (e.g., "Apartment", "House", "Condo").
    /// Derived from the related ListingType, falling back to the property type.
    /// </summary>
    public string? Type
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(ListingType?.Name))
            {
                return ListingType!.Name;
            }

            return RentalUnit?.Property?.PropertyType;
        }
        set { /* intentionally left blank to allow model binding */ }
    }

    /// <summary>
    /// Average overall rating of the listing based on user reviews.
    /// Aggregated from the Rating table and populated by the service layer.
    /// </summary>
    public decimal Rating { get; set; } = 0;

    /// <summary>
    /// Number of reviews for the listing.
    /// Aggregated from the Rating table and populated by the service layer.
    /// </summary>
    public int Reviews { get; set; } = 0;

    /// <summary>
    /// Optional manual/editorial tag set on the listing (e.g. "Great location", "Pet friendly").
    /// Takes precedence over the derived <see cref="DisplayTag"/>.
    /// </summary>
    [MaxLength(50)]
    public string? Tag { get; set; }

    /// <summary>
    /// The tag to display for the listing. Returns the manual <see cref="Tag"/> when set,
    /// otherwise derives one from the listing data using a priority order.
    /// </summary>
    public string? DisplayTag
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(Tag))
            {
                return Tag;
            }

            var today = DateTime.UtcNow.Date;
            bool isPublished = string.Equals(Status, "PUBLISHED", StringComparison.OrdinalIgnoreCase);

            // New this week: published within the last 7 days.
            if (PublishedAt.HasValue && PublishedAt.Value.Date >= today.AddDays(-7))
            {
                return "New this week";
            }

            // New this month: published within the last 30 days.
            if (PublishedAt.HasValue && PublishedAt.Value.Date >= today.AddDays(-30))
            {
                return "New this month";
            }

            // Move-in ready: published and available now or in the past.
            if (isPublished && AvailableFrom.HasValue && AvailableFrom.Value.Date <= today)
            {
                return "Move-in ready";
            }

            // Popular stay: highly rated with a meaningful number of reviews.
            if (Reviews >= 20 && Rating >= 4.8m)
            {
                return "Popular stay";
            }

            // IsPetFriendly.
            if (IsPetFriendly)
            {
                return "Pet friendly";
            }

            // Is Furnished.
            if (IsFurnished)
            {
                return "Furnished";
            }

            return null;
        }
        set { /* intentionally left blank to allow model binding */ }
    }

    /// <summary>
    /// Indicates whether the listing is furnished 
    /// </summary>
    public bool IsFurnished { get; set; } = false;

    /// <summary>
    /// Indicates whether the listing is pet friendly 
    /// </summary>
    public bool IsPetFriendly { get; set; } = false;

    /// <summary>
    /// Number of bedrooms
    /// </summary>
    public decimal? Bedrooms { get; set; }

    /// <summary>
    /// Number of bathrooms
    /// </summary>
    public decimal? Bathrooms { get; set; }

    /// <summary>
    /// Size in square feet
    /// </summary>
    public decimal? SquareFeet { get; set; }

    /// <summary>
    /// Base monthly rent amount
    /// </summary>
    [Required]
    public decimal BaseMonthlyRentAmount { get; set; } = 0;

    /// <summary>
    /// Security deposit amount
    /// </summary>
    [Required]
    public decimal SecurityDepositAmount { get; set; } = 0;

    /// <summary>
    /// Year the property was built
    /// </summary>
    [Required]
    public int? YearBuilt { get; set; }

    /// <summary>
    /// Listing status
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string? Status { get; set; } = "DRAFT";
    /// <summary>
    /// Date when listing was published
    /// </summary>
    public DateTime? PublishedAt { get; set; }
    /// <summary>
    /// Date when listing was unpublished
    /// </summary>
    public DateTime? UnpublishedAt { get; set; }

    /// <summary>
    /// Currency code
    /// </summary>
    [Required]
    [MaxLength(3)]
    public string? Currency { get; set; } = "CAD";
    /// <summary>
    /// Date from which listing is available
    /// </summary>
    public DateTime? AvailableFrom { get; set; }
    /// <summary>
    /// Date until which listing is available
    /// </summary>
    public DateTime? AvailableTo { get; set; }

    /// <summary>
    /// Minimum lease duration in months
    /// </summary>
    [Required]
    public short? MinimumLeaseMonths { get; set; }

    /// <summary>
    /// Maximum lease duration in months
    /// </summary>
    [Required]
    public short? MaximumLeaseMonths { get; set; }
    /// <summary>
    /// Deadline for applications
    /// </summary>
    public DateTime? ApplicationDeadline { get; set; }

    /// <summary>
    /// Additional notes
    /// </summary>
    [Required]
    [MaxLength(1000)]
    public string? Notes { get; set; }

    /// <summary>
    /// WiFi network name
    /// </summary>
    [MaxLength(50)]
    public string? WIFINetwork { get; set; }

    /// <summary>
    /// WiFi password
    /// </summary>
    [MaxLength(50)]
    public string? WIFIPassword { get; set; }

    /// <summary>
    /// Indicates if listing is accepting applications
    /// </summary>
    [Required]
    public bool AcceptingApplications { get; set; } = true;

    /// <summary>
    /// User who captured the record
    /// </summary>
    [MaxLength(50)]
    public string? CapturedBy { get; set; }
    /// <summary>
    /// Date when record was captured
    /// </summary>
    public DateTime? CapturedDate { get; set; }
    /// <summary>
    /// Date when record was last updated
    /// </summary>
    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// User who last updated the record
    /// </summary>
    [MaxLength(100)]
    public string? UpdatedBy { get; set; }
    /// <summary>
    /// FK to RentalUnit
    /// </summary>
    public RentalUnitDto? RentalUnit { get; set; }
    /// <summary>
    /// FK to ListingType
    /// </summary>
    public ListingTypeDto? ListingType { get; set; }
    /// <summary>
    /// FK to Organization
    /// </summary>
    public OrganizationDto? Organization { get; set; }

    /// <summary>
    /// Collection of ListingTermPrice entities associated with this listing. 
    /// </summary>
    public List<ListingTermPriceDto>? ListingTermPrices { get; set; }

    /// <summary>
    /// Collection of ListingAccessInstruction entities associated with this listing. 
    /// </summary>
    public List<ListingAccessInstructionDto>? ListingAccessInstructions { get; set; }

    /// <summary>
    /// Collection of ListingPhoto entities associated with this listing. 
    /// </summary>
    public List<ListingPhotoDto>? ListingPhotos { get; set; }

    /// <summary>
    /// Collection of ListingAmenity entities associated with this listing. 
    /// </summary>
    public List<ListingAmenityDto>? ListingAmenities { get; set; }

    /// <summary>
    /// Collection of ListingRule entities associated with this listing. 
    /// </summary>
    public List<ListingRuleDto>? ListingRules { get; set; }

    /// <summary>
    /// Collection of ListingPolicy entities associated with this listing. 
    /// </summary>
    public List<ListingPolicyDto>? ListingPolicies { get; set; }

    /// <summary>
    /// Collection of CalendarEvent entities associated with this listing. 
    /// </summary>
    public List<CalendarEventDto>? CalendarEvents { get; set; }

    /// <summary>
    /// Reviews for this listing, flattened from the listing's leases. 
    /// </summary>
    public List<RatingDto>? ReviewList { get; set; }
}