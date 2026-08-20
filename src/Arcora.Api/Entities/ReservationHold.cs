// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arcora.Api.Entities;
/// <summary>
/// Represents holds placed on listings for potential tenants during application or reservation process.
/// </summary>
[Table("ReservationHold")]
public class ReservationHold
{
    /// <summary>
    /// Primary key
    /// </summary>
    [Key]
    public Guid ReservationHoldID { get; set; }

    /// <summary>
    /// FK to Listing
    /// </summary>
    [Required]
    public Guid ListingID { get; set; }
    /// <summary>
    /// FK to RentalApplication
    /// </summary>
    public Guid? RentalApplicationID { get; set; }
    /// <summary>
    /// FK to Tenant
    /// </summary>
    public Guid? TenantID { get; set; }

    /// <summary>
    /// Hold start date
    /// </summary>
    [Required]
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Hold end date
    /// </summary>
    [Required]
    public DateTime EndDate { get; set; }

    /// <summary>
    /// Reason for hold
    /// </summary>
    [MaxLength(100)]
    public string? HoldReason { get; set; }

    /// <summary>
    /// Hold status
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string? Status { get; set; } = "ACTIVE";

    /// <summary>
    /// Expiration date of hold
    /// </summary>
    [Required]
    public DateTime ExpiresAt { get; set; }
    /// <summary>
    /// Date and time hold was released
    /// </summary>
    public DateTime? ReleasedAt { get; set; }
    /// <summary>
    /// Date and time hold was converted to lease
    /// </summary>
    public DateTime? ConvertedToLeaseAt { get; set; }
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
    /// FK to Listing
    /// </summary>
    [ForeignKey(nameof(ListingID))]
    public Listing? Listing { get; set; }

    /// <summary>
    /// FK to RentalApplication
    /// </summary>
    [ForeignKey(nameof(RentalApplicationID))]
    public RentalApplication? RentalApplication { get; set; }

    /// <summary>
    /// FK to Tenant
    /// </summary>
    [ForeignKey(nameof(TenantID))]
    public Tenant? Tenant { get; set; }
}