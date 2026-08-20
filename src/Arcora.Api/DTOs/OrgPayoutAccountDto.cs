// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;
using Arcora.Api.Entities;

namespace Arcora.Api.DTOs;
/// <summary>
/// Stores payout account details for organizations including verification and status.
/// </summary>
public class OrgPayoutAccountDto
{
    /// <summary>
    /// Key
    /// </summary>
    public long OrgPayoutAccountID { get; set; }

    /// <summary>
    /// FK to Organization
    /// </summary>
    [Required]
    public Guid OrganizationID { get; set; }

    /// <summary>
    /// Payout provider name
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string? ProviderName { get; set; }

    /// <summary>
    /// Provider account identifier
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string? ProviderAccountID { get; set; }

    /// <summary>
    /// Type of account
    /// </summary>
    [MaxLength(100)]
    public string? AccountType { get; set; }

    /// <summary>
    /// Bank name
    /// </summary>
    [MaxLength(100)]
    public string? BankName { get; set; }

    /// <summary>
    /// Last 4 digits of account
    /// </summary>
    [MaxLength(4)]
    public string? AccountLast4 { get; set; }

    /// <summary>
    /// Currency code
    /// </summary>
    [Required]
    [MaxLength(10)]
    public string? Currency { get; set; }

    /// <summary>
    /// Verification status
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string? VerificationStatus { get; set; }
    /// <summary>
    /// Verification date and time
    /// </summary>
    public DateTime? VerifiedAt { get; set; }

    /// <summary>
    /// Is default payout account
    /// </summary>
    [Required]
    public bool IsDefault { get; set; }

    /// <summary>
    /// Is active status
    /// </summary>
    [Required]
    public bool IsActive { get; set; }
    /// <summary>
    /// Record captured date
    /// </summary>
    public DateTime? CapturedDate { get; set; }

    /// <summary>
    /// User who captured the record
    /// </summary>
    [MaxLength(256)]
    public string? CapturedBy { get; set; }
    /// <summary>
    /// Record updated date
    /// </summary>
    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// User who updated the record
    /// </summary>
    [MaxLength(256)]
    public string? UpdatedBy { get; set; }
    /// <summary>
    /// FK to Organization
    /// </summary>
    public OrganizationDto? Organization { get; set; }
}