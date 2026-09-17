// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;
using Arcora.Api.DTOs;

namespace Arcora.Api.DTOs;
/// <summary>
/// Payload expected from the frontend when creating a property with units and listings.
/// Uses existing DTOs where appropriate and adds helper fields for catalog name lookups.
/// </summary>
public class ListingImportDto
{
    /// <summary>
    /// Property information. Include Address via the nested Address property.
    /// PropertyDto.Address should be populated by the caller.
    /// </summary>
    [Required]
    public PropertyDto Property { get; set; } = new PropertyDto();

    /// <summary>
    /// Units to create. Each unit contains the rental unit data and the listing to create for it.
    /// </summary>
    [Required]
    public List<UnitImportDto> Units { get; set; } = new List<UnitImportDto>();

    /// <summary>
    /// Optional safety details payload. Not mapped by the backend at the moment but kept for compatibility.
    /// </summary>
    public object? SafetyDetails { get; set; }
}

public class UnitImportDto
{
    /// <summary>
    /// Rental unit details. The caller may provide UnitNumber, Bedrooms, Bathrooms, etc.
    /// </summary>
    [Required]
    public RentalUnitDto RentalUnit { get; set; } = new RentalUnitDto();

    /// <summary>
    /// Friendly name of the unit type (e.g. "Apartment"). Will be resolved against UnitType catalog.
    /// </summary>
    public string? UnitTypeName { get; set; }

    /// <summary>
    /// Listing data to create for this unit. Uses a relaxed import DTO so fields that are
    /// resolved server-side (ListingTypeID, RentalUnitID) may be omitted.
    /// </summary>
    [Required]
    public ListingImportInnerDto Listing { get; set; } = new ListingImportInnerDto();

    /// <summary>
    /// Friendly name of the listing type (e.g. "ShortTerm"). Will be resolved against ListingType catalog.
    /// </summary>
    public string? ListingTypeName { get; set; }

    /// <summary>
    /// Selected amenity names (e.g. "WiFi"). Will be resolved against AmenityCatalog.
    /// </summary>
    public List<string>? SelectedAmenities { get; set; }

    /// <summary>
    /// Photos already uploaded. These records often have ListingID = null and CapturedBy values
    /// such as "2_Unit_1_"; they should be reconciled against the new listing ID.
    /// </summary>
    public List<ListingPhotoDto>? ListingPhotos { get; set; }
}

/// <summary>
/// Relaxed listing DTO used only for import so the frontend can omit IDs that
/// are created server-side. No [Required] attributes here.
/// </summary>
public class ListingImportInnerDto
{
    public Guid? ListingID { get; set; }
    public Guid? RentalUnitID { get; set; }
    public Guid? ListingTypeID { get; set; }
    public Guid? OrganizationID { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? CheckInDoorCode { get; set; }
    public string? Details { get; set; }
    public decimal? Price { get; set; }
    public string? Type { get; set; }
    public decimal Rating { get; set; }
    public int Reviews { get; set; }
    public string? Tag { get; set; }
    public string? DisplayTag { get; set; }
    public bool IsFurnished { get; set; }
    public bool IsPetFriendly { get; set; }
    public decimal? Bedrooms { get; set; }
    public decimal? Bathrooms { get; set; }
    public decimal? SquareFeet { get; set; }
    public decimal BaseMonthlyRentAmount { get; set; }
    public decimal SecurityDepositAmount { get; set; }
    public int? YearBuilt { get; set; }
    public string? Status { get; set; } = "DRAFT";
    public DateTime? PublishedAt { get; set; }
    public DateTime? UnpublishedAt { get; set; }
    public string? Currency { get; set; } = "CAD";
    public DateTime? AvailableFrom { get; set; }
    public DateTime? AvailableTo { get; set; }
    public short? MinimumLeaseMonths { get; set; }
    public short? MaximumLeaseMonths { get; set; }
    public DateTime? ApplicationDeadline { get; set; }
    public string? Notes { get; set; }
    public string? WIFINetwork { get; set; }
    public string? WIFIPassword { get; set; }
    public bool AcceptingApplications { get; set; } = true;
    public string? CapturedBy { get; set; }
    public DateTime? CapturedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public string? UpdatedBy { get; set; }
    public RentalUnitDto? RentalUnit { get; set; }
    public ListingTypeDto? ListingType { get; set; }
    public OrganizationDto? Organization { get; set; }
    public List<ListingTermPriceDto>? ListingTermPrices { get; set; }
    public List<ListingAccessInstructionDto>? ListingAccessInstructions { get; set; }
    public List<ListingPhotoDto>? ListingPhotos { get; set; }
    public List<ListingAmenityDto>? ListingAmenities { get; set; }
    public List<ListingRuleDto>? ListingRules { get; set; }
    public List<ListingPolicyDto>? ListingPolicies { get; set; }
    public List<CalendarEventDto>? CalendarEvents { get; set; }
}
