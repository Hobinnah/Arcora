// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;
using Arcora.Api.Entities;

namespace Arcora.Api.DTOs;
/// <summary>
/// Stores photos related to property listings with details about location and display order.
/// </summary>
public class ListingPhotoDto
{
    /// <summary>
    /// Key
    /// </summary>
    public Guid? ListingPhotoID { get; set; }

    /// <summary>
    /// FK to Listing
    /// </summary>
    [Required]
    public Guid ListingID { get; set; }

    /// <summary>
    /// Photo URL
    /// </summary>
    [Required]
    [MaxLength(500)]
    public string? Url { get; set; }

    /// <summary>
    /// Photo location (LivingRoom, Bedroom, Bathroom, Laundry, Exterior, Additional)
    /// </summary>
    [MaxLength(50)]
    public string? Location { get; set; }

    /// <summary>
    /// Photo caption
    /// </summary>
    [MaxLength(255)]
    public string? Caption { get; set; }

    /// <summary>
    /// Alternative text for photo
    /// </summary>
    [MaxLength(255)]
    public string? AltText { get; set; }

    /// <summary>
    /// Order of display
    /// </summary>
    [Required]
    public int DisplayOrder { get; set; } = 0;

    /// <summary>
    /// Indicates if photo is cover photo
    /// </summary>
    [Required]
    public bool IsCoverPhoto { get; set; }
    /// <summary>
    /// Date photo was captured
    /// </summary>
    public DateTime? CapturedDate { get; set; }

    /// <summary>
    /// User who captured the photo
    /// </summary>
    [MaxLength(100)]
    public string? CapturedBy { get; set; }
    /// <summary>
    /// FK to Listing
    /// </summary>
    public ListingDto? Listing { get; set; }
}