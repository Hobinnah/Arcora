// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;
using Arcora.Api.Entities;

namespace Arcora.Api.DTOs;
/// <summary>
/// Details individual payment items included in a payout with amounts and descriptions.
/// </summary>
public class PayoutItemDto
{
    /// <summary>
    /// Key
    /// </summary>
    public long PayoutItemID { get; set; }

    /// <summary>
    /// FK to Payout
    /// </summary>
    [Required]
    public long PayoutID { get; set; }

    /// <summary>
    /// FK to Payment
    /// </summary>
    [Required]
    public Guid PaymentID { get; set; }

    /// <summary>
    /// Gross amount of payment item
    /// </summary>
    [Required]
    public decimal GrossAmount { get; set; }

    /// <summary>
    /// Deduction amount from gross
    /// </summary>
    [Required]
    public decimal DeductionAmount { get; set; }

    /// <summary>
    /// Net payout amount after deductions
    /// </summary>
    [Required]
    public decimal NetPayoutAmount { get; set; }

    /// <summary>
    /// Description of payout item
    /// </summary>
    [MaxLength(1000)]
    public string? Description { get; set; }
    /// <summary>
    /// Date record was captured
    /// </summary>
    public DateTime? CapturedDate { get; set; }

    /// <summary>
    /// User ID who captured the record
    /// </summary>
    [MaxLength(256)]
    public string? CapturedBy { get; set; }
    /// <summary>
    /// FK to Payout
    /// </summary>
    public PayoutDto? Payout { get; set; }
    /// <summary>
    /// FK to Payment
    /// </summary>
    public PaymentDto? Payment { get; set; }
}