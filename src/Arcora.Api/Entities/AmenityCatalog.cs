// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arcora.Api.Entities;
/// <summary>
/// Catalog of amenities available for listings with category and active status.
/// </summary>
[Table("AmenityCatalog")]
public class AmenityCatalog
{
    /// <summary>
    /// Primary key
    /// </summary>
    [Key]
    public Guid AmenityID { get; set; }

    /// <summary>
    /// Amenity name
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string? Name { get; set; }

    /// <summary>
    /// Amenity category
    /// </summary>
    [MaxLength(100)]
    public string? Category { get; set; }

    /// <summary>
    /// Active status
    /// </summary>
    [Required]
    public bool IsActive { get; set; }
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
}