// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arcora.Api.Entities;
/// <summary>
/// Details individual payment items included in a payout with amounts and descriptions.
/// </summary>
[Table("PayoutItem")]
public class PayoutItem
{
    /// <summary>
    /// Primary key
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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
    [ForeignKey(nameof(PayoutID))]
    public Payout? Payout { get; set; }

    /// <summary>
    /// FK to Payment
    /// </summary>
    [ForeignKey(nameof(PaymentID))]
    public Payment? Payment { get; set; }
}