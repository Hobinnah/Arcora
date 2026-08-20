// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arcora.Api.Entities;
/// <summary>
/// Represents individual rental units within a property with details and status.
/// </summary>
[Table("RentalUnit")]
public class RentalUnit
{
    /// <summary>
    /// Primary key
    /// </summary>
    [Key]
    public Guid RentalUnitID { get; set; }

    /// <summary>
    /// FK to Property
    /// </summary>
    [Required]
    public Guid PropertyID { get; set; }
    /// <summary>
    /// FK to UnitType
    /// </summary>
    public Guid? UnitTypeID { get; set; }

    /// <summary>
    /// Unit number or identifier
    /// </summary>
    [MaxLength(50)]
    public string? UnitNumber { get; set; }

    /// <summary>
    /// Floor number
    /// </summary>
    [MaxLength(20)]
    public string? FloorNumber { get; set; }

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
    /// Square footage
    /// </summary>
    public int? SquareFeet { get; set; }
    /// <summary>
    /// Maximum number of occupants
    /// </summary>
    public int? MaximumOccupants { get; set; }

    /// <summary>
    /// Additional notes
    /// </summary>
    [MaxLength(1000)]
    public string? Notes { get; set; }

    /// <summary>
    /// Status of the rental unit
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string? Status { get; set; } = "ACTIVE";
    /// <summary>
    /// Record creation date
    /// </summary>
    public DateTime? CapturedDate { get; set; }

    /// <summary>
    /// Record created by user
    /// </summary>
    [MaxLength(100)]
    public string? CapturedBy { get; set; }
    /// <summary>
    /// Record last update date
    /// </summary>
    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// Record last updated by user
    /// </summary>
    [MaxLength(100)]
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// FK to Property
    /// </summary>
    [ForeignKey(nameof(PropertyID))]
    public Property? Property { get; set; }

    /// <summary>
    /// FK to UnitType
    /// </summary>
    [ForeignKey(nameof(UnitTypeID))]
    public UnitType? UnitType { get; set; }
}