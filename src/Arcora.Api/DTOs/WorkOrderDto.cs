// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;
using Arcora.Api.Entities;

namespace Arcora.Api.DTOs;
/// <summary>
/// Work orders created for maintenance requests and assigned to contractors or organization members.
/// </summary>
public class WorkOrderDto
{
    /// <summary>
    /// Key
    /// </summary>
    public Guid? WorkOrderID { get; set; }

    /// <summary>
    /// FK to MaintenanceRequest
    /// </summary>
    [Required]
    public Guid MaintenanceRequestID { get; set; }
    /// <summary>
    /// FK to Contractor
    /// </summary>
    public Guid? ContractorID { get; set; }
    /// <summary>
    /// FK to OrganizationMember
    /// </summary>
    public Guid? AssignedOrganizationMemberID { get; set; }
    /// <summary>
    /// FK to Lease
    /// </summary>
    public Guid? LeaseID { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public Guid? LeaseRenewalID { get; set; }

    /// <summary>
    /// Title of the work order
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string? Title { get; set; }

    /// <summary>
    /// Description of the work order
    /// </summary>
    [MaxLength(1000)]
    public string? Description { get; set; }

    /// <summary>
    /// Work order status
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string? Status { get; set; } = "DRAFT";
    /// <summary>
    /// Estimated cost of the work order
    /// </summary>
    public decimal? EstimatedCost { get; set; }
    /// <summary>
    /// Final cost of the work order
    /// </summary>
    public decimal? FinalCost { get; set; }

    /// <summary>
    /// Currency of the costs
    /// </summary>
    [Required]
    [MaxLength(3)]
    public string? Currency { get; set; } = "CAD";
    /// <summary>
    /// Scheduled date and time for work order
    /// </summary>
    public DateTime? ScheduledAt { get; set; }
    /// <summary>
    /// Date and time work order started
    /// </summary>
    public DateTime? StartedAt { get; set; }
    /// <summary>
    /// Date and time work order completed
    /// </summary>
    public DateTime? CompletedAt { get; set; }
    /// <summary>
    /// Date and time work order cancelled
    /// </summary>
    public DateTime? CancelledAt { get; set; }
    /// <summary>
    /// Date work order was captured
    /// </summary>
    public DateTime? CapturedDate { get; set; }

    /// <summary>
    /// User ID who captured the work order
    /// </summary>
    [MaxLength(100)]
    public string? CapturedBy { get; set; }
    /// <summary>
    /// Date work order was last updated
    /// </summary>
    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// User ID who last updated the work order
    /// </summary>
    [MaxLength(100)]
    public string? UpdatedBy { get; set; }
    /// <summary>
    /// FK to MaintenanceRequest
    /// </summary>
    public MaintenanceRequestDto? MaintenanceRequest { get; set; }
    /// <summary>
    /// FK to Contractor
    /// </summary>
    public ContractorDto? Contractor { get; set; }
    /// <summary>
    /// FK to OrganizationMember
    /// </summary>
    public OrganizationMemberDto? AssignedOrganizationMember { get; set; }
    /// <summary>
    /// FK to Lease
    /// </summary>
    public LeaseDto? Lease { get; set; }
}