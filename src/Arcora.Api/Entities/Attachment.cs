// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arcora.Api.Entities;
/// <summary>
/// Stores file attachments linked to various entities.
/// </summary>
[Table("Attachment")]
public class Attachment
{
    /// <summary>
    /// Primary key
    /// </summary>
    [Key]
    public Guid AttachmentID { get; set; }

    /// <summary>
    /// Type of the linked entity
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string? EntityType { get; set; }

    /// <summary>
    /// ID of the linked entity
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string? EntityID { get; set; }

    /// <summary>
    /// Name of the file
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string? FileName { get; set; }

    /// <summary>
    /// MIME type of the file
    /// </summary>
    [MaxLength(100)]
    public string? MimeType { get; set; }
    /// <summary>
    /// Size of the file in bytes
    /// </summary>
    public long? FileSizeBytes { get; set; }

    /// <summary>
    /// Storage provider for the file
    /// </summary>
    [MaxLength(100)]
    public string? StorageProvider { get; set; }

    /// <summary>
    /// Reference or path in storage
    /// </summary>
    [Required]
    [MaxLength(500)]
    public string? StorageReference { get; set; }

    /// <summary>
    /// Type of attachment
    /// </summary>
    [MaxLength(50)]
    public string? AttachmentType { get; set; }

    /// <summary>
    /// Description of the attachment
    /// </summary>
    [MaxLength(1000)]
    public string? Description { get; set; }

    /// <summary>
    /// Client ID for the upload
    /// </summary>
    [MaxLength(100)]
    public string? ClientUploadID { get; set; }

    /// <summary>
    /// SHA256 checksum for deduplication
    /// </summary>
    [MaxLength(64)]
    public string? ChecksumSha256 { get; set; }

    /// <summary>
    /// Status of the upload
    /// </summary>
    [MaxLength(20)]
    public string? UploadStatus { get; set; }

    /// <summary>
    /// Expiration time for the upload
    /// </summary>
    public DateTime? UploadExpiresAt { get; set; }

    /// <summary>
    /// Reason for upload failure
    /// </summary>
    [MaxLength(256)]
    public string? UploadFailureReason { get; set; }
    /// <summary>
    /// Date attachment was captured
    /// </summary>
    public DateTime? CapturedDate { get; set; }

    /// <summary>
    /// User ID who captured the attachment
    /// </summary>
    [MaxLength(100)]
    public string? CapturedBy { get; set; }
}