// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arcora.Api.Entities;
/// <summary>
/// Associates amenities with listings including optional notes.
/// </summary>
[Table("ListingAmenity")]
public class ListingAmenity
{
    /// <summary>
    /// Primary key
    /// </summary>
    [Key]
    public Guid ListingAmenityID { get; set; }

    /// <summary>
    /// FK to Listing
    /// </summary>
    [Required]
    public Guid ListingID { get; set; }

    /// <summary>
    /// FK to AmenityCatalog
    /// </summary>
    [Required]
    public Guid AmenityID { get; set; }

    /// <summary>
    /// Optional notes about the amenity for the listing
    /// </summary>
    [MaxLength(1000)]
    public string? Notes { get; set; }
    /// <summary>
    /// Record capture date
    /// </summary>
    public DateTime? CapturedDate { get; set; }

    /// <summary>
    /// User who captured the record
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string? CapturedBy { get; set; }

    /// <summary>
    /// FK to Listing
    /// </summary>
    [ForeignKey(nameof(ListingID))]
    public Listing? Listing { get; set; }

    /// <summary>
    /// FK to AmenityCatalog
    /// </summary>
    [ForeignKey(nameof(AmenityID))]
    public AmenityCatalog? AmenityCatalog { get; set; }
}