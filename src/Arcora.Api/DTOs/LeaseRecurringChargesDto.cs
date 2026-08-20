// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;
using Arcora.Api.Entities;

namespace Arcora.Api.DTOs;
/// <summary>
/// Represents recurring charges associated with leases including fees, amounts, and billing frequency.
/// </summary>
public class LeaseRecurringChargesDto
{
    /// <summary>
    /// Key
    /// </summary>
    public Guid? LeaseRecurringChargeID { get; set; }

    /// <summary>
    /// FK to Lease
    /// </summary>
    [Required]
    public Guid LeaseID { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public Guid? LeaseRenewalID { get; set; }
    /// <summary>
    /// FK to Fee
    /// </summary>
    public Guid? FeeID { get; set; }

    /// <summary>
    /// Code for the charge
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string? ChargeCode { get; set; }

    /// <summary>
    /// Description of the charge
    /// </summary>
    [Required]
    [MaxLength(255)]
    public string? Description { get; set; }

    /// <summary>
    /// Charge amount
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
    /// Billing frequency
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string? Frequency { get; set; } = "MONTHLY";
    /// <summary>
    /// Day of month for billing
    /// </summary>
    public short? BillingDayOfMonth { get; set; }

    /// <summary>
    /// First due date for charge
    /// </summary>
    [Required]
    public DateTime FirstDueDate { get; set; }
    /// <summary>
    /// Last due date for charge
    /// </summary>
    public DateTime? LastDueDate { get; set; }

    /// <summary>
    /// Proration rule for charge
    /// </summary>
    [MaxLength(50)]
    public string? ProrationRule { get; set; }

    /// <summary>
    /// Flag to auto generate invoice
    /// </summary>
    [Required]
    public bool AutoGenerateInvoice { get; set; }

    /// <summary>
    /// Active status of the charge
    /// </summary>
    [Required]
    public bool IsActive { get; set; }
    /// <summary>
    /// Record creation date/time
    /// </summary>
    public DateTime? CapturedDate { get; set; }

    /// <summary>
    /// Record created by
    /// </summary>
    [MaxLength(100)]
    public string? CapturedBy { get; set; }
    /// <summary>
    /// Record last update date/time
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
    public LeaseDto? Lease { get; set; }
    /// <summary>
    /// FK to Fee
    /// </summary>
    public FeeDto? Fee { get; set; }
}