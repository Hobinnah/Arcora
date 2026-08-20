// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;
using Arcora.Api.Entities;

namespace Arcora.Api.DTOs;
/// <summary>
/// Represents payouts made to organizations with status and processing details.
/// </summary>
public class PayoutDto
{
    /// <summary>
    /// Key
    /// </summary>
    public long PayoutID { get; set; }

    /// <summary>
    /// FK to Organization
    /// </summary>
    [Required]
    public Guid OrganizationID { get; set; }

    /// <summary>
    /// FK to OrgPayoutAccount
    /// </summary>
    [Required]
    public long OrgPayoutAccountID { get; set; }

    /// <summary>
    /// Payout amount
    /// </summary>
    [Required]
    public decimal Amount { get; set; }

    /// <summary>
    /// Currency code
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string? Currency { get; set; }

    /// <summary>
    /// Payout status
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string? Status { get; set; }
    /// <summary>
    /// Scheduled payout date
    /// </summary>
    public DateTime? ScheduledAt { get; set; }

    /// <summary>
    /// Date payout was requested
    /// </summary>
    [Required]
    public DateTime RequestedAt { get; set; }
    /// <summary>
    /// Date payout was processed
    /// </summary>
    public DateTime? ProcessedAt { get; set; }
    /// <summary>
    /// Date payout was paid
    /// </summary>
    public DateTime? PaidAt { get; set; }

    /// <summary>
    /// Name of payout provider
    /// </summary>
    [MaxLength(100)]
    public string? ProviderName { get; set; }

    /// <summary>
    /// Provider payout identifier
    /// </summary>
    [MaxLength(256)]
    public string? ProviderPayoutID { get; set; }

    /// <summary>
    /// Reason for payout failure
    /// </summary>
    [MaxLength(256)]
    public string? FailureReason { get; set; }

    /// <summary>
    /// User ID who captured the record
    /// </summary>
    [MaxLength(256)]
    public string? CapturedBy { get; set; }
    /// <summary>
    /// Date record was captured
    /// </summary>
    public DateTime? CapturedDate { get; set; }
    /// <summary>
    /// FK to Organization
    /// </summary>
    public OrganizationDto? Organization { get; set; }
    /// <summary>
    /// FK to OrgPayoutAccount
    /// </summary>
    public OrgPayoutAccountDto? OrgPayoutAccount { get; set; }
}