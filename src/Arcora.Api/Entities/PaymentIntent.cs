// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arcora.Api.Entities;
/// <summary>
/// Represents an intent to collect payment from a tenant, linked to an invoice and autopay mandate.
/// </summary>
[Table("PaymentIntent")]
public class PaymentIntent
{
    /// <summary>
    /// Primary key
    /// </summary>
    [Key]
    public Guid PaymentIntentID { get; set; }
    /// <summary>
    /// FK to InvoiceMaster
    /// </summary>
    public Guid? InvoiceMasterID { get; set; }
    /// <summary>
    /// FK to AutopayMandate
    /// </summary>
    public Guid? AutopayMandateID { get; set; }

    /// <summary>
    /// FK to Tenant
    /// </summary>
    [Required]
    public Guid TenantID { get; set; }

    /// <summary>
    /// FK to PaymentMethod
    /// </summary>
    [Required]
    public Guid PaymentMethodID { get; set; }

    /// <summary>
    /// Amount to be collected
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
    /// Payment intent status
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string? Status { get; set; } = "CREATED";

    /// <summary>
    /// Method of collection
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string? CollectionMethod { get; set; } = "AUTOPAY";

    /// <summary>
    /// Payment provider name
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string? ProviderName { get; set; }

    /// <summary>
    /// Provider's payment intent identifier
    /// </summary>
    [MaxLength(255)]
    public string? ProviderPaymentIntentID { get; set; }

    /// <summary>
    /// Idempotency key for payment intent
    /// </summary>
    [Required]
    [MaxLength(255)]
    public string? IdempotencyKey { get; set; }

    /// <summary>
    /// Scheduled date and time for charge
    /// </summary>
    [Required]
    public DateTime ScheduledChargeAt { get; set; }
    /// <summary>
    /// Date and time when payment intent started
    /// </summary>
    public DateTime? StartedAt { get; set; }
    /// <summary>
    /// Date and time when payment intent completed
    /// </summary>
    public DateTime? CompletedAt { get; set; }
    /// <summary>
    /// Date and time when payment intent was cancelled
    /// </summary>
    public DateTime? CancelledAt { get; set; }

    /// <summary>
    /// Reason for failure if any
    /// </summary>
    [MaxLength(256)]
    public string? FailureReason { get; set; }
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
    /// Date and time record was last updated
    /// </summary>
    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// User who last updated the record
    /// </summary>
    [MaxLength(100)]
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// FK to InvoiceMaster
    /// </summary>
    [ForeignKey(nameof(InvoiceMasterID))]
    public InvoiceMaster? InvoiceMaster { get; set; }

    /// <summary>
    /// FK to AutopayMandate
    /// </summary>
    [ForeignKey(nameof(AutopayMandateID))]
    public AutopayMandate? AutopayMandate { get; set; }

    /// <summary>
    /// FK to Tenant
    /// </summary>
    [ForeignKey(nameof(TenantID))]
    public Tenant? Tenant { get; set; }

    /// <summary>
    /// FK to PaymentMethod
    /// </summary>
    [ForeignKey(nameof(PaymentMethodID))]
    public PaymentMethod? PaymentMethod { get; set; }
}