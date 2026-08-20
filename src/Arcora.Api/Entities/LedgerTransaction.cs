// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arcora.Api.Entities;
/// <summary>
/// Records financial transactions in the ledger with references to payments, invoices, refunds, and payouts.
/// </summary>
[Table("LedgerTransaction")]
public class LedgerTransaction
{
    /// <summary>
    /// Primary key
    /// </summary>
    [Key]
    public Guid LedgerTransactionID { get; set; }

    /// <summary>
    /// FK to Organization
    /// </summary>
    [Required]
    public Guid OrganizationID { get; set; }

    /// <summary>
    /// Type of transaction
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string? TransactionType { get; set; }

    /// <summary>
    /// Date of transaction
    /// </summary>
    [Required]
    public DateTime TransactionDate { get; set; }

    /// <summary>
    /// Transaction description
    /// </summary>
    [MaxLength(1000)]
    public string? Description { get; set; }
    /// <summary>
    /// FK to Payment
    /// </summary>
    public Guid? PaymentID { get; set; }
    /// <summary>
    /// FK to InvoiceMaster
    /// </summary>
    public Guid? InvoiceMasterID { get; set; }
    /// <summary>
    /// FK to Refund
    /// </summary>
    public Guid? RefundID { get; set; }
    /// <summary>
    /// FK to Payout
    /// </summary>
    public long? PayoutID { get; set; }

    /// <summary>
    /// Reference number for transaction
    /// </summary>
    [MaxLength(256)]
    public string? ReferenceNumber { get; set; }

    /// <summary>
    /// Transaction status
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string? Status { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public Guid? ReversedTransactionID { get; set; }
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
    /// FK to Organization
    /// </summary>
    [ForeignKey(nameof(OrganizationID))]
    public Organization? Organization { get; set; }

    /// <summary>
    /// FK to Payment
    /// </summary>
    [ForeignKey(nameof(PaymentID))]
    public Payment? Payment { get; set; }

    /// <summary>
    /// FK to InvoiceMaster
    /// </summary>
    [ForeignKey(nameof(InvoiceMasterID))]
    public InvoiceMaster? InvoiceMaster { get; set; }

    /// <summary>
    /// FK to Refund
    /// </summary>
    [ForeignKey(nameof(RefundID))]
    public Refund? Refund { get; set; }

    /// <summary>
    /// FK to Payout
    /// </summary>
    [ForeignKey(nameof(PayoutID))]
    public Payout? Payout { get; set; }
}