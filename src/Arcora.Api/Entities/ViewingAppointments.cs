// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arcora.Api.Entities;
/// <summary>
/// Manages appointments for viewing listings including scheduling, status, and type of viewing.
/// </summary>
[Table("ViewingAppointments")]
public class ViewingAppointments
{
    /// <summary>
    /// Primary key
    /// </summary>
    [Key]
    public Guid ViewingAppointmentID { get; set; }

    /// <summary>
    /// FK to Listing
    /// </summary>
    [Required]
    public Guid ListingID { get; set; }

    /// <summary>
    /// FK to User who requested the appointment
    /// </summary>
    [Required]
    public long RequestedByUserID { get; set; }
    /// <summary>
    /// FK to Tenant
    /// </summary>
    public Guid? TenantID { get; set; }
    /// <summary>
    /// FK to OrganizationMember assigned to the appointment
    /// </summary>
    public Guid? AssignedOrganizationMemberID { get; set; }

    /// <summary>
    /// Date and time scheduled for the appointment
    /// </summary>
    [Required]
    public DateTime ScheduledFor { get; set; }

    /// <summary>
    /// Duration of the appointment in minutes
    /// </summary>
    [Required]
    public int DurationMinutes { get; set; } = 30;

    /// <summary>
    /// Time zone of the appointment
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string? TimeZone { get; set; }

    /// <summary>
    /// Appointment status
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string? Status { get; set; } = "REQUESTED";

    /// <summary>
    /// Type of viewing
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string? ViewingType { get; set; } = "IN_PERSON";

    /// <summary>
    /// URL for virtual meeting
    /// </summary>
    [MaxLength(500)]
    public string? MeetingUrl { get; set; }

    /// <summary>
    /// Additional notes for the appointment
    /// </summary>
    [MaxLength(1000)]
    public string? Notes { get; set; }
    /// <summary>
    /// Date and time when appointment was cancelled
    /// </summary>
    public DateTime? CancelledAt { get; set; }

    /// <summary>
    /// Reason for cancellation
    /// </summary>
    [MaxLength(256)]
    public string? CancellationReason { get; set; }
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
    /// FK to Listing
    /// </summary>
    [ForeignKey(nameof(ListingID))]
    public Listing? Listing { get; set; }

    /// <summary>
    /// FK to User who requested the appointment
    /// </summary>
    [ForeignKey(nameof(RequestedByUserID))]
    public User? RequestedByUser { get; set; }

    /// <summary>
    /// FK to Tenant
    /// </summary>
    [ForeignKey(nameof(TenantID))]
    public Tenant? Tenant { get; set; }

    /// <summary>
    /// FK to OrganizationMember assigned to the appointment
    /// </summary>
    [ForeignKey(nameof(AssignedOrganizationMemberID))]
    public OrganizationMember? AssignedOrganizationMember { get; set; }
}