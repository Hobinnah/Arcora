// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arcora.Api.Entities;
/// <summary>
/// Stores refund details related to tenant payments including status and processing information.
/// </summary>
[Table("Refund")]
public class Refund
{
    /// <summary>
    /// Primary key
    /// </summary>
    [Key]
    public Guid RefundID { get; set; }

    /// <summary>
    /// FK to Payment
    /// </summary>
    [Required]
    public Guid PaymentID { get; set; }

    /// <summary>
    /// FK to Tenant
    /// </summary>
    [Required]
    public Guid TenantID { get; set; }

    /// <summary>
    /// Refund amount
    /// </summary>
    [Required]
    public decimal Amount { get; set; }

    /// <summary>
    /// Currency code
    /// </summary>
    [Required]
    [MaxLength(10)]
    public string? Currency { get; set; }

    /// <summary>
    /// Reason for refund
    /// </summary>
    [MaxLength(256)]
    public string? Reason { get; set; }

    /// <summary>
    /// Refund status
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string? Status { get; set; } = "REQUESTED";

    /// <summary>
    /// Refund provider name
    /// </summary>
    [MaxLength(100)]
    public string? ProviderName { get; set; }

    /// <summary>
    /// Provider refund identifier
    /// </summary>
    [MaxLength(100)]
    public string? ProviderRefundID { get; set; }

    /// <summary>
    /// Refund requested date and time
    /// </summary>
    [Required]
    public DateTime RequestedAt { get; set; }
    /// <summary>
    /// Refund processed date and time
    /// </summary>
    public DateTime? ProcessedAt { get; set; }
    /// <summary>
    /// Refund failure date and time
    /// </summary>
    public DateTime? FailedAt { get; set; }

    /// <summary>
    /// Reason for refund failure
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
    [MaxLength(256)]
    public string? CapturedBy { get; set; }

    /// <summary>
    /// FK to Payment
    /// </summary>
    [ForeignKey(nameof(PaymentID))]
    public Payment? Payment { get; set; }

    /// <summary>
    /// FK to Tenant
    /// </summary>
    [ForeignKey(nameof(TenantID))]
    public Tenant? Tenant { get; set; }
}