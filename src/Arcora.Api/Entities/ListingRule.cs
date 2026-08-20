// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arcora.Api.Entities;
/// <summary>
/// Rules associated with a listing including type, title, description, allowance, and effective dates.
/// </summary>
[Table("ListingRule")]
public class ListingRule
{
    /// <summary>
    /// Primary key
    /// </summary>
    [Key]
    public Guid ListingRuleID { get; set; }

    /// <summary>
    /// FK to Listing
    /// </summary>
    [Required]
    public Guid ListingID { get; set; }

    /// <summary>
    /// Type of the rule
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string? RuleType { get; set; }

    /// <summary>
    /// Title of the rule
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string? RuleTitle { get; set; }

    /// <summary>
    /// Description of the rule
    /// </summary>
    [MaxLength(1000)]
    public string? RuleDescription { get; set; }
    /// <summary>
    /// Indicates if the rule is allowed
    /// </summary>
    public bool? IsAllowed { get; set; }
    /// <summary>
    /// Rule effective start date
    /// </summary>
    public DateTime? EffectiveFrom { get; set; }
    /// <summary>
    /// Rule effective end date
    /// </summary>
    public DateTime? EffectiveTo { get; set; }
    /// <summary>
    /// Record capture date
    /// </summary>
    public DateTime? CapturedDate { get; set; }

    /// <summary>
    /// User who captured the record
    /// </summary>
    [MaxLength(100)]
    public string? CapturedBy { get; set; }
    /// <summary>
    /// Record last update date
    /// </summary>
    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// User who last updated the record
    /// </summary>
    [MaxLength(100)]
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// FK to Listing
    /// </summary>
    [ForeignKey(nameof(ListingID))]
    public Listing? Listing { get; set; }
}