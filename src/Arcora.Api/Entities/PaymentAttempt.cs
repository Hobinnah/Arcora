// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arcora.Api.Entities;
/// <summary>
/// Represents an attempt to process a payment for a payment intent.
/// </summary>
[Table("PaymentAttempt")]
public class PaymentAttempt
{
    /// <summary>
    /// Primary key
    /// </summary>
    [Key]
    public Guid PaymentAttemptID { get; set; }

    /// <summary>
    /// FK to PaymentIntent
    /// </summary>
    [Required]
    public Guid PaymentIntentID { get; set; }

    /// <summary>
    /// Attempt sequence number
    /// </summary>
    [Required]
    public int AttemptNumber { get; set; }

    /// <summary>
    /// FK to the PaymentMethod used for this attempt. Distinguishes a PAD attempt from a
    /// card-fallback attempt on the same intent.
    /// </summary>
    public Guid? PaymentMethodID { get; set; }

    /// <summary>
    /// Payment rail/kind used for this attempt (e.g. "PAD" or "CARD").
    /// </summary>
    [MaxLength(50)]
    public string? MethodKind { get; set; }

    /// <summary>
    /// Classification of the failure that drives the retry/fallback state machine
    /// (NONE, PENDING, TEMPORARY, PERMANENT).
    /// </summary>
    [MaxLength(50)]
    public string? FailureCategory { get; set; }

    /// <summary>
    /// Amount attempted
    /// </summary>
    [Required]
    public decimal Amount { get; set; }

    /// <summary>
    /// Attempt status
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string? Status { get; set; } = "PENDING";

    /// <summary>
    /// Provider's attempt identifier
    /// </summary>
    [MaxLength(255)]
    public string? ProviderAttemptID { get; set; }

    /// <summary>
    /// Failure code if any
    /// </summary>
    [MaxLength(100)]
    public string? FailureCode { get; set; }

    /// <summary>
    /// Failure message if any
    /// </summary>
    [MaxLength(256)]
    public string? FailureMessage { get; set; }

    /// <summary>
    /// Date and time attempt was made
    /// </summary>
    [Required]
    public DateTime AttemptedAt { get; set; }
    /// <summary>
    /// Date and time attempt completed
    /// </summary>
    public DateTime? CompletedAt { get; set; }
    /// <summary>
    /// Date and time for next retry
    /// </summary>
    public DateTime? NextRetryAt { get; set; }

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
    /// FK to PaymentIntent
    /// </summary>
    [ForeignKey(nameof(PaymentIntentID))]
    public PaymentIntent? PaymentIntent { get; set; }
}