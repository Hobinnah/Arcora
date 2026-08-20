// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;
using Arcora.Api.Entities;

namespace Arcora.Api.DTOs;
/// <summary>
/// Events related to listings such as leases, rental applications, reservations, maintenance with status, timing, occupancy, and notes.
/// </summary>
public class CalendarEventDto
{
    /// <summary>
    /// Key
    /// </summary>
    public Guid? CalendarEventID { get; set; }

    /// <summary>
    /// FK to Listing
    /// </summary>
    [Required]
    public Guid ListingID { get; set; }
    /// <summary>
    /// FK to Lease
    /// </summary>
    public Guid? LeaseID { get; set; }
    /// <summary>
    /// FK to RentalApplication
    /// </summary>
    public Guid? RentalApplicationID { get; set; }
    /// <summary>
    /// FK to ReservationHold
    /// </summary>
    public Guid? ReservationHoldID { get; set; }
    /// <summary>
    /// FK to MaintenanceRequest
    /// </summary>
    public Guid? MaintenanceRequestID { get; set; }

    /// <summary>
    /// Type of event
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string? EventType { get; set; }

    /// <summary>
    /// Event status
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string? Status { get; set; } = "CONFIRMED";

    /// <summary>
    /// Event start date and time
    /// </summary>
    [Required]
    public DateTime StartAt { get; set; }

    /// <summary>
    /// Event end date and time
    /// </summary>
    [Required]
    public DateTime EndAt { get; set; }

    /// <summary>
    /// Indicates if event lasts all day
    /// </summary>
    [Required]
    public bool IsAllDay { get; set; }

    /// <summary>
    /// Event title
    /// </summary>
    [MaxLength(255)]
    public string? Title { get; set; }

    /// <summary>
    /// Name of occupant
    /// </summary>
    [MaxLength(100)]
    public string? OccupantName { get; set; }
    /// <summary>
    /// Number of occupants
    /// </summary>
    public int? OccupantCount { get; set; }

    /// <summary>
    /// Source system of the event
    /// </summary>
    [MaxLength(100)]
    public string? SourceSystem { get; set; }

    /// <summary>
    /// Reference ID in source system
    /// </summary>
    [MaxLength(255)]
    public string? SourceReferenceID { get; set; }

    /// <summary>
    /// External calendar identifier
    /// </summary>
    [MaxLength(255)]
    public string? ExternalCalendarID { get; set; }

    /// <summary>
    /// Indicates if event blocks availability
    /// </summary>
    [Required]
    public bool BlocksAvailability { get; set; }

    /// <summary>
    /// Additional notes about the event
    /// </summary>
    [MaxLength(1000)]
    public string? Notes { get; set; }
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
    public ListingDto? Listing { get; set; }
    /// <summary>
    /// FK to Lease
    /// </summary>
    public LeaseDto? Lease { get; set; }
    /// <summary>
    /// FK to RentalApplication
    /// </summary>
    public RentalApplicationDto? RentalApplication { get; set; }
    /// <summary>
    /// FK to ReservationHold
    /// </summary>
    public ReservationHoldDto? ReservationHold { get; set; }
    /// <summary>
    /// FK to MaintenanceRequest
    /// </summary>
    public MaintenanceRequestDto? MaintenanceRequest { get; set; }
}