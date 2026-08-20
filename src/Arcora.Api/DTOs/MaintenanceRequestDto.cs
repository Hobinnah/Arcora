// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;
using Arcora.Api.Entities;

namespace Arcora.Api.DTOs;
/// <summary>
/// Represents maintenance requests submitted for properties or rental units.
/// </summary>
public class MaintenanceRequestDto
{
    /// <summary>
    /// Key
    /// </summary>
    public Guid? MaintenanceRequestID { get; set; }
    /// <summary>
    /// FK to Property
    /// </summary>
    public Guid? PropertyID { get; set; }
    /// <summary>
    /// FK to RentalUnit
    /// </summary>
    public Guid? RentalUnitID { get; set; }
    /// <summary>
    /// FK to Listing
    /// </summary>
    public Guid? ListingID { get; set; }
    /// <summary>
    /// FK to Lease
    /// </summary>
    public Guid? LeaseID { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public Guid? LeaseRenewalID { get; set; }
    /// <summary>
    /// FK to Tenant
    /// </summary>
    public Guid? SubmittedByTenantID { get; set; }

    /// <summary>
    /// FK to Category
    /// </summary>
    [Required]
    public int CategoryID { get; set; }

    /// <summary>
    /// Priority level
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string? Priority { get; set; } = "NORMAL";

    /// <summary>
    /// Status of the request
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string? Status { get; set; } = "OPEN";

    /// <summary>
    /// Title of the request
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string? Title { get; set; }

    /// <summary>
    /// Detailed description
    /// </summary>
    [Required]
    [MaxLength(1000)]
    public string? Description { get; set; }
    /// <summary>
    /// Permission to enter property
    /// </summary>
    public bool? PermissionToEnter { get; set; }

    /// <summary>
    /// Date and time submitted
    /// </summary>
    [Required]
    public DateTime SubmittedAt { get; set; }
    /// <summary>
    /// Date and time acknowledged
    /// </summary>
    public DateTime? AcknowledgedAt { get; set; }
    /// <summary>
    /// Date and time scheduled
    /// </summary>
    public DateTime? ScheduledAt { get; set; }
    /// <summary>
    /// Date and time completed
    /// </summary>
    public DateTime? CompletedAt { get; set; }
    /// <summary>
    /// Date and time cancelled
    /// </summary>
    public DateTime? CancelledAt { get; set; }
    /// <summary>
    /// Date request was captured
    /// </summary>
    public DateTime? CapturedDate { get; set; }

    /// <summary>
    /// User ID who captured the request
    /// </summary>
    [MaxLength(100)]
    public string? CapturedBy { get; set; }
    /// <summary>
    /// Date request was last updated
    /// </summary>
    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// User ID who last updated the request
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
    /// FK to Listing
    /// </summary>
    public ListingDto? Listing { get; set; }
    /// <summary>
    /// FK to Lease
    /// </summary>
    public LeaseDto? Lease { get; set; }
    /// <summary>
    /// FK to Tenant
    /// </summary>
    public TenantDto? SubmittedByTenant { get; set; }
    /// <summary>
    /// FK to Category
    /// </summary>
    public CategoryDto? Category { get; set; }
}