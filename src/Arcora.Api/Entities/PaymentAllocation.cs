// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arcora.Api.Entities;
/// <summary>
/// Represents allocation of payments to invoices and invoice details.
/// </summary>
[Table("PaymentAllocation")]
public class PaymentAllocation
{
    /// <summary>
    /// Primary key
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long PaymentAllocationID { get; set; }

    /// <summary>
    /// FK to Payment
    /// </summary>
    [Required]
    public Guid PaymentID { get; set; }

    /// <summary>
    /// FK to InvoiceMaster
    /// </summary>
    [Required]
    public Guid InvoiceMasterID { get; set; }
    /// <summary>
    /// FK to InvoiceDetail
    /// </summary>
    public Guid? InvoiceDetailID { get; set; }

    /// <summary>
    /// Amount allocated
    /// </summary>
    [Required]
    public decimal AllocatedAmount { get; set; }

    /// <summary>
    /// Type of allocation
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string? AllocationType { get; set; }

    /// <summary>
    /// Date and time allocated
    /// </summary>
    [Required]
    public DateTime AllocatedAt { get; set; }
    /// <summary>
    /// Date captured
    /// </summary>
    public DateTime? CapturedDate { get; set; }

    /// <summary>
    /// User ID who captured the record
    /// </summary>
    [MaxLength(256)]
    public string? CapturedBy { get; set; }

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
    /// FK to InvoiceDetail
    /// </summary>
    [ForeignKey(nameof(InvoiceDetailID))]
    public InvoiceDetail? InvoiceDetail { get; set; }
}