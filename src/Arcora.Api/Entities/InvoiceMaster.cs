// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arcora.Api.Entities;
/// <summary>
/// Represents invoices issued to tenants for leases including billing and payment details.
/// </summary>
[Table("InvoiceMaster")]
public class InvoiceMaster
{
    /// <summary>
    /// Primary key
    /// </summary>
    [Key]
    public Guid InvoiceMasterID { get; set; }

    /// <summary>
    /// FK to Lease
    /// </summary>
    [Required]
    public Guid LeaseID { get; set; }
    /// <summary>
    /// FK to Lease Renewal
    /// </summary>
    public Guid? LeaseRenewalID { get; set; }

    /// <summary>
    /// FK to Organization
    /// </summary>
    [Required]
    public Guid OrganizationID { get; set; }

    /// <summary>
    /// FK to Tenant
    /// </summary>
    [Required]
    public Guid TenantID { get; set; }

    /// <summary>
    /// Invoice number
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string? InvoiceNumber { get; set; }

    /// <summary>
    /// Billing period start date
    /// </summary>
    [Required]
    public DateTime BillingPeriodStart { get; set; }

    /// <summary>
    /// Billing period end date
    /// </summary>
    [Required]
    public DateTime BillingPeriodEnd { get; set; }

    /// <summary>
    /// Invoice due date
    /// </summary>
    [Required]
    public DateTime DueDate { get; set; }

    /// <summary>
    /// Subtotal amount
    /// </summary>
    [Required]
    public decimal SubtotalAmount { get; set; } = 0;

    /// <summary>
    /// Tax amount
    /// </summary>
    [Required]
    public decimal TaxAmount { get; set; } = 0;

    /// <summary>
    /// Discount amount
    /// </summary>
    [Required]
    public decimal DiscountAmount { get; set; } = 0;

    /// <summary>
    /// Late fee amount
    /// </summary>
    [Required]
    public decimal LateFeeAmount { get; set; } = 0;

    /// <summary>
    /// Adjustment amount
    /// </summary>
    [Required]
    public decimal AdjustmentAmount { get; set; } = 0;

    /// <summary>
    /// Total invoice amount
    /// </summary>
    [Required]
    public decimal TotalAmount { get; set; } = 0;

    /// <summary>
    /// Amount paid
    /// </summary>
    [Required]
    public decimal AmountPaid { get; set; } = 0;

    /// <summary>
    /// Balance due
    /// </summary>
    [Required]
    public decimal BalanceDue { get; set; } = 0;

    /// <summary>
    /// Currency code
    /// </summary>
    [Required]
    [MaxLength(3)]
    public string? Currency { get; set; } = "CAD";

    /// <summary>
    /// Invoice status
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string? Status { get; set; } = "DRAFT";
    /// <summary>
    /// Invoice issued date
    /// </summary>
    public DateTime? IssuedAt { get; set; }
    /// <summary>
    /// Invoice paid date
    /// </summary>
    public DateTime? PaidAt { get; set; }
    /// <summary>
    /// Invoice voided date
    /// </summary>
    public DateTime? VoidedAt { get; set; }

    /// <summary>
    /// Reason for voiding invoice
    /// </summary>
    [MaxLength(1000)]
    public string? VoidReason { get; set; }
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
    /// Record last update date
    /// </summary>
    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// Record last updated by
    /// </summary>
    [MaxLength(100)]
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// FK to Lease
    /// </summary>
    [ForeignKey(nameof(LeaseID))]
    public Lease? Lease { get; set; }

    /// <summary>
    /// FK to Lease Renewal
    /// </summary>
    [ForeignKey(nameof(LeaseRenewalID))]
    public Lease? LeaseRenewalLease { get; set; }

    /// <summary>
    /// FK to Organization
    /// </summary>
    [ForeignKey(nameof(OrganizationID))]
    public Organization? Organization { get; set; }

    /// <summary>
    /// FK to Tenant
    /// </summary>
    [ForeignKey(nameof(TenantID))]
    public Tenant? Tenant { get; set; }
}