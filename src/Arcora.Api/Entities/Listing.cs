// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arcora.Api.Entities;
/// <summary>
/// Represents rental property listings managed by organizations.
/// </summary>
[Table("Listing")]
public class Listing
{
    /// <summary>
    /// Primary key
    /// </summary>
    [Key]
    public Guid ListingID { get; set; }

    /// <summary>
    /// FK to RentalUnit
    /// </summary>
    [Required]
    public Guid RentalUnitID { get; set; }

    /// <summary>
    /// FK to ListingType
    /// </summary>
    [Required]
    public Guid ListingTypeID { get; set; }

    /// <summary>
    /// FK to Organization
    /// </summary>
    [Required]
    public Guid OrganizationID { get; set; }

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

    /// <summary>
    /// Number of bedrooms
    /// </summary>
    [MaxLength(3)]
    public decimal? Bedrooms { get; set; }

    /// <summary>
    /// Number of bathrooms
    /// </summary>
    [MaxLength(3)]
    public decimal? Bathrooms { get; set; }

    /// <summary>
    /// Size in square feet
    /// </summary>
    [MaxLength(3)]
    public decimal? SquareFeet { get; set; }

    /// <summary>
    /// Base monthly rent amount
    /// </summary>
    [Required]
    public decimal BaseMonthlyRentAmount { get; set; }

    /// <summary>
    /// Security deposit amount
    /// </summary>
    [Required]
    public decimal SecurityDepositAmount { get; set; } = 0;

    /// <summary>
    /// Year the property was built
    /// </summary>
    [Required]
    public int YearBuilt { get; set; }

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
    public short MinimumLeaseMonths { get; set; }

    /// <summary>
    /// Maximum lease duration in months
    /// </summary>
    [Required]
    public short MaximumLeaseMonths { get; set; }
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
    public bool AcceptingApplications { get; set; }

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
    [ForeignKey(nameof(RentalUnitID))]
    public RentalUnit? RentalUnit { get; set; }

    /// <summary>
    /// FK to ListingType
    /// </summary>
    [ForeignKey(nameof(ListingTypeID))]
    public ListingType? ListingType { get; set; }

    /// <summary>
    /// FK to Organization
    /// </summary>
    [ForeignKey(nameof(OrganizationID))]
    public Organization? Organization { get; set; }
}