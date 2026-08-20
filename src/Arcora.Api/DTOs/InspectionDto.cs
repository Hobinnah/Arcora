// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;
using Arcora.Api.Entities;

namespace Arcora.Api.DTOs;
/// <summary>
/// Represents property or rental unit inspections.
/// </summary>
public class InspectionDto
{
    /// <summary>
    /// Key
    /// </summary>
    public Guid? InspectionID { get; set; }
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
    public PropertyDto? Property { get; set; }
    /// <summary>
    /// FK to RentalUnit
    /// </summary>
    public RentalUnitDto? RentalUnit { get; set; }
    /// <summary>
    /// FK to Lease
    /// </summary>
    public LeaseDto? Lease { get; set; }
}