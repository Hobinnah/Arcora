// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arcora.Api.Entities;
/// <summary>
/// Represents property or rental unit inspections.
/// </summary>
[Table("Inspection")]
public class Inspection
{
    /// <summary>
    /// Primary key
    /// </summary>
    [Key]
    public Guid InspectionID { get; set; }
    /// <summary>
    /// FK to Property
    /// </summary>
    public Guid? PropertyID { get; set; }
    /// <summary>
    /// FK to RentalUnit
    /// </summary>
    public Guid? RentalUnitID { get; set; }
    /// <summary>
    /// FK to Lease
    /// </summary>
    public Guid? LeaseID { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public Guid? LeaseRenewalID { get; set; }

    /// <summary>
    /// Type of inspection
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string? InspectionType { get; set; }

    /// <summary>
    /// Inspection status
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string? Status { get; set; } = "SCHEDULED";
    /// <summary>
    /// Scheduled date and time for inspection
    /// </summary>
    public DateTime? ScheduledFor { get; set; }
    /// <summary>
    /// Date and time inspection started
    /// </summary>
    public DateTime? StartedAt { get; set; }
    /// <summary>
    /// Date and time inspection completed
    /// </summary>
    public DateTime? CompletedAt { get; set; }

    /// <summary>
    /// Overall condition assessment
    /// </summary>
    [MaxLength(50)]
    public string? OverallCondition { get; set; }

    /// <summary>
    /// Additional notes
    /// </summary>
    [MaxLength(1000)]
    public string? Notes { get; set; }
    /// <summary>
    /// Date inspection was captured
    /// </summary>
    public DateTime? CapturedDate { get; set; }

    /// <summary>
    /// User ID who captured the inspection
    /// </summary>
    [MaxLength(100)]
    public string? CapturedBy { get; set; }
    /// <summary>
    /// Date inspection was last updated
    /// </summary>
    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// User ID who last updated the inspection
    /// </summary>
    [MaxLength(100)]
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// FK to Property
    /// </summary>
    [ForeignKey(nameof(PropertyID))]
    public Property? Property { get; set; }

    /// <summary>
    /// FK to RentalUnit
    /// </summary>
    [ForeignKey(nameof(RentalUnitID))]
    public RentalUnit? RentalUnit { get; set; }

    /// <summary>
    /// FK to Lease
    /// </summary>
    [ForeignKey(nameof(LeaseID))]
    public Lease? Lease { get; set; }
}