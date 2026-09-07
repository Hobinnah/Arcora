// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Arcora.Api.Entities;

namespace Arcora.Api.DTOs;
/// <summary>
/// Associates amenities with listings including optional notes.
/// </summary>
public class ListingAmenityDto
{
    /// <summary>
    /// Key
    /// </summary>
    public Guid? ListingAmenityID { get; set; }

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
    [JsonIgnore]
    public ListingDto? Listing { get; set; }
    /// <summary>
    /// FK to AmenityCatalog
    /// </summary>
    public AmenityCatalogDto? AmenityCatalog { get; set; }
}