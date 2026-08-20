// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arcora.Api.Entities;
/// <summary>
/// Stores fee details including type, calculation, amounts, and validity.
/// </summary>
[Table("Fee")]
public class Fee
{
    /// <summary>
    /// Primary key
    /// </summary>
    [Key]
    public Guid FeeID { get; set; }

    /// <summary>
    /// FK to FeeType
    /// </summary>
    [Required]
    public int FeeTypeID { get; set; }
    /// <summary>
    /// FK to Organization
    /// </summary>
    public Guid? OrganizationID { get; set; }

    /// <summary>
    /// Fee code
    /// </summary>
    [MaxLength(100)]
    public string? Code { get; set; }

    /// <summary>
    /// Fee name
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string? Name { get; set; }

    /// <summary>
    /// Calculation type of fee
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string? CalculationType { get; set; }
    /// <summary>
    /// Fixed amount for fee
    /// </summary>
    public decimal? FixedAmount { get; set; }
    /// <summary>
    /// Percentage rate for fee
    /// </summary>
    public decimal? PercentageRate { get; set; }
    /// <summary>
    /// Minimum fee amount
    /// </summary>
    public decimal? MinimumFeeAmount { get; set; }
    /// <summary>
    /// Maximum fee amount
    /// </summary>
    public decimal? MaximumFeeAmount { get; set; }

    /// <summary>
    /// Currency code
    /// </summary>
    [Required]
    [MaxLength(3)]
    public string? Currency { get; set; } = "CAD";

    /// <summary>
    /// Indicates if fee is taxable
    /// </summary>
    [Required]
    public bool IsTaxable { get; set; }
    /// <summary>
    /// Fee effective start date
    /// </summary>
    public DateTime? EffectiveFrom { get; set; }
    /// <summary>
    /// Fee effective end date
    /// </summary>
    public DateTime? EffectiveTo { get; set; }

    /// <summary>
    /// Indicates if fee is active
    /// </summary>
    [Required]
    public bool IsActive { get; set; }
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
    /// FK to FeeType
    /// </summary>
    [ForeignKey(nameof(FeeTypeID))]
    public FeeType? FeeType { get; set; }

    /// <summary>
    /// FK to Organization
    /// </summary>
    [ForeignKey(nameof(OrganizationID))]
    public Organization? Organization { get; set; }
}