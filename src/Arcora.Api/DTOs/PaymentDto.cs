// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;
using Arcora.Api.Entities;

namespace Arcora.Api.DTOs;
/// <summary>
/// Represents a completed payment transaction for a tenant.
/// </summary>
public class PaymentDto
{
    /// <summary>
    /// Key
    /// </summary>
    public Guid? PaymentID { get; set; }

    /// <summary>
    /// FK to PaymentIntent
    /// </summary>
    [Required]
    public Guid PaymentIntentID { get; set; }

    /// <summary>
    /// FK to Tenant
    /// </summary>
    [Required]
    public Guid TenantID { get; set; }

    /// <summary>
    /// Provider's charge identifier
    /// </summary>
    [Required]
    [MaxLength(255)]
    public string? ProviderChargeID { get; set; }

    /// <summary>
    /// Payment status
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string? Status { get; set; } = "PENDING";

    /// <summary>
    /// Gross amount of the payment
    /// </summary>
    [Required]
    public decimal GrossAmount { get; set; }

    /// <summary>
    /// Platform fee amount
    /// </summary>
    [Required]
    public decimal PlatformFeeAmount { get; set; } = 0;

    /// <summary>
    /// Processor fee amount
    /// </summary>
    [Required]
    public decimal ProcessorFeeAmount { get; set; } = 0;

    /// <summary>
    /// Amount refunded
    /// </summary>
    [Required]
    public decimal RefundedAmount { get; set; } = 0;

    /// <summary>
    /// Net amount after fees and refunds
    /// </summary>
    [Required]
    public decimal NetAmount { get; set; }

    /// <summary>
    /// Currency code
    /// </summary>
    [Required]
    [MaxLength(3)]
    public string? Currency { get; set; } = "CAD";
    /// <summary>
    /// Date and time payment was made
    /// </summary>
    public DateTime? PaidAt { get; set; }
    /// <summary>
    /// Date and time payment was settled
    /// </summary>
    public DateTime? SettledAt { get; set; }

    /// <summary>
    /// User who captured the record
    /// </summary>
    [MaxLength(100)]
    public string? CapturedBy { get; set; }
    /// <summary>
    /// Date and time record was captured
    /// </summary>
    public DateTime? CapturedDate { get; set; }
    /// <summary>
    /// Date and time record was last updated
    /// </summary>
    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// User who last updated the record
    /// </summary>
    [MaxLength(100)]
    public string? UpdatedBy { get; set; }
    /// <summary>
    /// FK to PaymentIntent
    /// </summary>
    public PaymentIntentDto? PaymentIntent { get; set; }
    /// <summary>
    /// FK to Tenant
    /// </summary>
    public TenantDto? Tenant { get; set; }
}