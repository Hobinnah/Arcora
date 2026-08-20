// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;
using Arcora.Api.Entities;

namespace Arcora.Api.DTOs;
/// <summary>
/// Represents allocation of payments to invoices and invoice details.
/// </summary>
public class PaymentAllocationDto
{
    /// <summary>
    /// Key
    /// </summary>
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
    public PaymentDto? Payment { get; set; }
    /// <summary>
    /// FK to InvoiceMaster
    /// </summary>
    public InvoiceMasterDto? InvoiceMaster { get; set; }
    /// <summary>
    /// FK to InvoiceDetail
    /// </summary>
    public InvoiceDetailDto? InvoiceDetail { get; set; }
}