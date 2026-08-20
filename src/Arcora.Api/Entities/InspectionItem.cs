// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arcora.Api.Entities;
/// <summary>
/// Items inspected during an inspection with condition and repair details.
/// </summary>
[Table("InspectionItem")]
public class InspectionItem
{
    /// <summary>
    /// Primary key
    /// </summary>
    [Key]
    public Guid InspectionItemID { get; set; }

    /// <summary>
    /// FK to Inspection
    /// </summary>
    [Required]
    public Guid InspectionID { get; set; }

    /// <summary>
    /// Area of the inspection item
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string? Area { get; set; }

    /// <summary>
    /// Name of the inspection item
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string? ItemName { get; set; }

    /// <summary>
    /// Condition of the inspection item
    /// </summary>
    [MaxLength(50)]
    public string? Condition { get; set; }

    /// <summary>
    /// Additional notes about the item
    /// </summary>
    [MaxLength(1000)]
    public string? Notes { get; set; }

    /// <summary>
    /// Indicates if repair is required
    /// </summary>
    [Required]
    public bool RequiresRepair { get; set; }
    /// <summary>
    /// Estimated cost for repair
    /// </summary>
    public decimal? EstimatedRepairCost { get; set; }
    /// <summary>
    /// Date inspection item was captured
    /// </summary>
    public DateTime? CapturedDate { get; set; }

    /// <summary>
    /// User ID who captured the inspection item
    /// </summary>
    [MaxLength(100)]
    public string? CapturedBy { get; set; }

    /// <summary>
    /// FK to Inspection
    /// </summary>
    [ForeignKey(nameof(InspectionID))]
    public Inspection? Inspection { get; set; }
}