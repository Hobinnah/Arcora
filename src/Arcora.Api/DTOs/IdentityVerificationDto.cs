// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;
using Arcora.Api.Entities;

namespace Arcora.Api.DTOs;
/// <summary>
/// Tracks identity verification status and details for users.
/// </summary>
public class IdentityVerificationDto
{
    /// <summary>
    /// Key
    /// </summary>
    public Guid? IdentityVerificationID { get; set; }

    /// <summary>
    /// FK to User
    /// </summary>
    [Required]
    public long UserID { get; set; }

    /// <summary>
    /// Type of verification
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string? VerificationType { get; set; } = "IDENTITY";

    /// <summary>
    /// Verification provider name
    /// </summary>
    [MaxLength(100)]
    public string? ProviderName { get; set; }

    /// <summary>
    /// Provider reference ID
    /// </summary>
    [MaxLength(255)]
    public string? ProviderReferenceID { get; set; }

    /// <summary>
    /// Verification status
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string? Status { get; set; } = "NOT_STARTED";
    /// <summary>
    /// Verification requested timestamp
    /// </summary>
    public DateTime? RequestedAt { get; set; }
    /// <summary>
    /// Verification completed timestamp
    /// </summary>
    public DateTime? VerifiedAt { get; set; }
    /// <summary>
    /// Verification expiration timestamp
    /// </summary>
    public DateTime? ExpiresAt { get; set; }

    /// <summary>
    /// Reason for verification failure
    /// </summary>
    [MaxLength(256)]
    public string? FailureReason { get; set; }
    /// <summary>
    /// Record captured date
    /// </summary>
    public DateTime? CapturedDate { get; set; }

    /// <summary>
    /// User who captured the record
    /// </summary>
    [MaxLength(100)]
    public string? CapturedBy { get; set; }
    /// <summary>
    /// Record last updated date
    /// </summary>
    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// User who last updated the record
    /// </summary>
    [MaxLength(100)]
    public string? UpdatedBy { get; set; }
    /// <summary>
    /// FK to User
    /// </summary>
    public User? User { get; set; }
}