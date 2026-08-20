// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;

namespace Arcora.Api.DTOs;
/// <summary>
/// Stores file attachments linked to various entities.
/// </summary>
public class AttachmentDto
{
    /// <summary>
    /// Key
    /// </summary>
    public Guid? AttachmentID { get; set; }

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
    [MaxLength(255)]
    public string? Description { get; set; }
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