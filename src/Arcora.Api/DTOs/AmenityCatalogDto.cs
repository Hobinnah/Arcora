// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;

namespace Arcora.Api.DTOs;
/// <summary>
/// Catalog of amenities available for listings with category and active status.
/// </summary>
public class AmenityCatalogDto
{
    /// <summary>
    /// Key
    /// </summary>
    public Guid? AmenityID { get; set; }

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