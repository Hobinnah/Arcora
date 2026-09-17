// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;
using Arcora.Api.Entities;

namespace Arcora.Api.DTOs;
/// <summary>
/// Stores documents related to leases and lease renewals including their status and storage details.
/// </summary>
public class LeaseDocumentsDto
{
    /// <summary>
    /// Key
    /// </summary>
    public Guid? LeaseDocumentID { get; set; }

    /// <summary>
    /// FK to Lease
    /// </summary>
    public Guid? LeaseID { get; set; }
    /// <summary>
    /// FK to LeaseRenewals
    /// </summary>
    public Guid? LeaseRenewalID { get; set; }
    /// <summary>
    /// FK to RentalApplication
    /// </summary>
    public Guid? RentalApplicationID { get; set; }

    /// <summary>
    /// FK to Listing
    /// </summary>
    public Guid? ListingID { get; set; }

    /// <summary>
    /// FK to Tenant
    /// </summary>
    public Guid? TenantID { get; set; }

    [Required]
    [MaxLength(50)]
    public string? DocumentType { get; set; }

    /// <summary>
    /// Status of the document
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string? DocumentStatus { get; set; } = "DRAFT";

    /// <summary>
    /// Original filename of the document
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string? OriginalFilename { get; set; }

    /// <summary>
    /// Storage provider for the document
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
    /// Reference to the stored file
    /// </summary>
    [Required]
    [MaxLength(500)]
    public string? StorageReference { get; set; }

    /// <summary>
    /// Hash of the file for integrity check
    /// </summary>
    [MaxLength(255)]
    public string? FileHash { get; set; }

    /// <summary>
    /// Indicates if this is the primary document
    /// </summary>
    [Required]
    public bool IsPrimary { get; set; }
    /// <summary>
    /// Date document was generated
    /// </summary>
    public DateTime? GeneratedAt { get; set; }
    /// <summary>
    /// Date document was sent for signature
    /// </summary>
    public DateTime? SentForSignatureAt { get; set; }
    /// <summary>
    /// Date document was fully signed
    /// </summary>
    public DateTime? FullySignedAt { get; set; }
    /// <summary>
    /// Record creation date
    /// </summary>
    public DateTime? CapturedDate { get; set; }

    /// <summary>
    /// Record created by
    /// </summary>
    [MaxLength(100)]
    public string? CapturedBy { get; set; }
    /// <summary>
    /// Record last update date
    /// </summary>
    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// Record last updated by
    /// </summary>
    [MaxLength(100)]
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// A short-lived, read-only URL (SAS) for accessing the stored document. This is generated on read
    /// and is not persisted; it may be null if the underlying blob no longer exists.
    /// </summary>
    public string? Url { get; set; }

    /// <summary>
    /// FK to Lease
    /// </summary>
    public LeaseDto? Lease { get; set; }
    /// <summary>
    /// FK to LeaseRenewals
    /// </summary>
    public LeaseRenewalsDto? LeaseRenewals { get; set; }
}