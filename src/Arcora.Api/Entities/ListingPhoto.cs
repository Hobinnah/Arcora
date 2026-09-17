// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arcora.Api.Entities;
/// <summary>
/// Stores photos related to property listings with details about location and display order.
/// </summary>
[Table("ListingPhoto")]
public class ListingPhoto
{
    /// <summary>
    /// Primary key
    /// </summary>
    [Key]
    public Guid ListingPhotoID { get; set; }

    /// <summary>
    /// FK to Listing
    /// </summary>
    public Guid? ListingID { get; set; }

    /// <summary>
    /// Photo URL
    /// </summary>
    [Required]
    [MaxLength(500)]
    public string? Url { get; set; }

    /// <summary>
    /// Storage provider for the photo (e.g. AZURE_BLOB).
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string? StorageProvider { get; set; } = "AZURE_BLOB";

    /// <summary>
    /// Storage container name
    /// </summary>
    [MaxLength(255)]
    public string? StorageContainer { get; set; }

    /// <summary>
    /// Reference to the stored blob
    /// </summary>
    [Required]
    [MaxLength(500)]
    public string? StorageReference { get; set; }

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
    /// Optional numeric user id (IdentityUser<long>) who captured the photo.
    /// </summary>
    public long? UserID { get; set; }

    /// <summary>
    /// FK to Listing
    /// </summary>
    [ForeignKey(nameof(ListingID))]
    public Listing? Listing { get; set; }
}