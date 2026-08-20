// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;
using Arcora.Api.Entities;

namespace Arcora.Api.DTOs;
/// <summary>
/// Records financial transactions in the ledger with references to payments, invoices, refunds, and payouts.
/// </summary>
public class LedgerTransactionDto
{
    /// <summary>
    /// Key
    /// </summary>
    public Guid? LedgerTransactionID { get; set; }

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
    public OrganizationDto? Organization { get; set; }
    /// <summary>
    /// FK to Payment
    /// </summary>
    public PaymentDto? Payment { get; set; }
    /// <summary>
    /// FK to InvoiceMaster
    /// </summary>
    public InvoiceMasterDto? InvoiceMaster { get; set; }
    /// <summary>
    /// FK to Refund
    /// </summary>
    public RefundDto? Refund { get; set; }
    /// <summary>
    /// FK to Payout
    /// </summary>
    public PayoutDto? Payout { get; set; }
}