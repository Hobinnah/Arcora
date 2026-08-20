// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arcora.Api.Entities;
/// <summary>
/// Records transactions related to security deposits such as payments, refunds, and invoice adjustments.
/// </summary>
[Table("SecurityDepositTransaction")]
public class SecurityDepositTransaction
{
    /// <summary>
    /// Primary key
    /// </summary>
    [Key]
    public Guid SecurityDepositTransactionID { get; set; }

    /// <summary>
    /// FK to SecurityDeposit
    /// </summary>
    [Required]
    public Guid SecurityDepositID { get; set; }
    /// <summary>
    /// FK to Payment
    /// </summary>
    public Guid? PaymentID { get; set; }
    /// <summary>
    /// FK to Refund
    /// </summary>
    public Guid? RefundID { get; set; }
    /// <summary>
    /// FK to InvoiceMaster
    /// </summary>
    public Guid? InvoiceMasterID { get; set; }
    /// <summary>
    /// FK to InvoiceDetail
    /// </summary>
    public Guid? InvoiceDetailID { get; set; }

    /// <summary>
    /// Type of transaction
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string? TransactionType { get; set; }

    /// <summary>
    /// Transaction amount
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
    /// Transaction description
    /// </summary>
    [MaxLength(255)]
    public string? Description { get; set; }

    /// <summary>
    /// Date and time transaction occurred
    /// </summary>
    [Required]
    public DateTime OccurredAt { get; set; }
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
    /// FK to SecurityDeposit
    /// </summary>
    [ForeignKey(nameof(SecurityDepositID))]
    public SecurityDeposit? SecurityDeposit { get; set; }

    /// <summary>
    /// FK to Payment
    /// </summary>
    [ForeignKey(nameof(PaymentID))]
    public Payment? Payment { get; set; }

    /// <summary>
    /// FK to Refund
    /// </summary>
    [ForeignKey(nameof(RefundID))]
    public Refund? Refund { get; set; }

    /// <summary>
    /// FK to InvoiceMaster
    /// </summary>
    [ForeignKey(nameof(InvoiceMasterID))]
    public InvoiceMaster? InvoiceMaster { get; set; }

    /// <summary>
    /// FK to InvoiceDetail
    /// </summary>
    [ForeignKey(nameof(InvoiceDetailID))]
    public InvoiceDetail? InvoiceDetail { get; set; }
}