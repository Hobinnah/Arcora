// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;
using Arcora.Api.Entities;

namespace Arcora.Api.DTOs;
/// <summary>
/// Stores detailed line items for invoices including charges, fees, and service periods.
/// </summary>
public class InvoiceDetailDto
{
    /// <summary>
    /// Key
    /// </summary>
    public Guid? InvoiceDetailID { get; set; }

    /// <summary>
    /// FK to InvoiceMaster
    /// </summary>
    [Required]
    public Guid InvoiceMasterID { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public Guid? LeaseRecurringChargeID { get; set; }
    /// <summary>
    /// FK to Fee
    /// </summary>
    public Guid? FeeID { get; set; }

    /// <summary>
    /// Type of line item
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string? LineType { get; set; }

    /// <summary>
    /// Description of the line item
    /// </summary>
    [Required]
    [MaxLength(255)]
    public string? Description { get; set; }
    /// <summary>
    /// Start date of service period
    /// </summary>
    public DateTime? ServicePeriodStart { get; set; }
    /// <summary>
    /// End date of service period
    /// </summary>
    public DateTime? ServicePeriodEnd { get; set; }

    /// <summary>
    /// Quantity of items
    /// </summary>
    [Required]
    public decimal Quantity { get; set; } = 1;

    /// <summary>
    /// Unit price amount
    /// </summary>
    [Required]
    public decimal UnitAmount { get; set; }

    /// <summary>
    /// Amount for the line before tax
    /// </summary>
    [Required]
    public decimal LineAmount { get; set; }
    /// <summary>
    /// Tax rate applied
    /// </summary>
    public decimal? TaxRate { get; set; }

    /// <summary>
    /// Tax amount for the line
    /// </summary>
    [Required]
    public decimal TaxAmount { get; set; } = 0;

    /// <summary>
    /// Total amount for the line including tax
    /// </summary>
    [Required]
    public decimal TotalLineAmount { get; set; }

    /// <summary>
    /// User who captured the record
    /// </summary>
    [MaxLength(100)]
    public string? CapturedBy { get; set; }
    /// <summary>
    /// Date when record was captured
    /// </summary>
    public DateTime? CapturedDate { get; set; }
    /// <summary>
    /// FK to InvoiceMaster
    /// </summary>
    public InvoiceMasterDto? InvoiceMaster { get; set; }
    /// <summary>
    /// FK to Fee
    /// </summary>
    public FeeDto? Fee { get; set; }
}