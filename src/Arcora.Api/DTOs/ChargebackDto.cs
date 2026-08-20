// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;
using Arcora.Api.Entities;

namespace Arcora.Api.DTOs;
/// <summary>
/// Represents a dispute or chargeback related to a payment.
/// </summary>
public class ChargebackDto
{
    /// <summary>
    /// Key
    /// </summary>
    public Guid? ChargebackID { get; set; }

    /// <summary>
    /// FK to Payment
    /// </summary>
    [Required]
    public Guid PaymentID { get; set; }
    /// <summary>
    /// FK to Tenant
    /// </summary>
    public Guid? TenantID { get; set; }
    /// <summary>
    /// FK to Organization
    /// </summary>
    public Guid? OrganizationID { get; set; }

    /// <summary>
    /// Provider's dispute identifier
    /// </summary>
    [Required]
    [MaxLength(255)]
    public string? ProviderDisputeID { get; set; }

    /// <summary>
    /// Chargeback amount
    /// </summary>
    [Required]
    public decimal Amount { get; set; }

    /// <summary>
    /// Currency code
    /// </summary>
    [Required]
    [MaxLength(3)]
    public string? Currency { get; set; } = "CAD";

    /// <summary>
    /// Reason code for chargeback
    /// </summary>
    [MaxLength(100)]
    public string? ReasonCode { get; set; }

    /// <summary>
    /// Chargeback status
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string? Status { get; set; } = "OPEN";

    /// <summary>
    /// Date and time chargeback was opened
    /// </summary>
    [Required]
    public DateTime OpenedAt { get; set; }
    /// <summary>
    /// Date evidence is due
    /// </summary>
    public DateTime? EvidenceDueAt { get; set; }
    /// <summary>
    /// Date evidence was submitted
    /// </summary>
    public DateTime? EvidenceSubmittedAt { get; set; }
    /// <summary>
    /// Date chargeback was resolved
    /// </summary>
    public DateTime? ResolvedAt { get; set; }

    /// <summary>
    /// Outcome of the chargeback
    /// </summary>
    [MaxLength(50)]
    public string? Outcome { get; set; }

    /// <summary>
    /// JSON response from provider
    /// </summary>
    [MaxLength(256)]
    public string? ProviderResponse { get; set; }
    /// <summary>
    /// Date and time record was captured
    /// </summary>
    public DateTime? CapturedDate { get; set; }

    /// <summary>
    /// User who captured the record
    /// </summary>
    [MaxLength(100)]
    public string? CapturedBy { get; set; }
    /// <summary>
    /// FK to Payment
    /// </summary>
    public PaymentDto? Payment { get; set; }
    /// <summary>
    /// FK to Tenant
    /// </summary>
    public TenantDto? Tenant { get; set; }
    /// <summary>
    /// FK to Organization
    /// </summary>
    public OrganizationDto? Organization { get; set; }
}