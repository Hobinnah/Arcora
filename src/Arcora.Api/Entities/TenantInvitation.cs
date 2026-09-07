// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arcora.Api.Entities;
/// <summary>
/// Manages invitations sent to tenants for lease or rental application purposes with status tracking.
/// </summary>
[Table("TenantInvitation")]
public class TenantInvitation
{
    /// <summary>
    /// Primary key
    /// </summary>
    [Key]
    public Guid TenantInvitationID { get; set; }
    /// <summary>
    /// FK to Listing
    /// </summary>
    public Guid? ListingID { get; set; }
    /// <summary>
    /// FK to Lease
    /// </summary>
    public Guid? LeaseID { get; set; }
    /// <summary>
    /// FK to RentalApplication
    /// </summary>
    public Guid? RentalApplicationID { get; set; }

    /// <summary>
    /// Purpose of the invitation
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string? InvitationPurpose { get; set; }

    /// <summary>
    /// Email address of invitee
    /// </summary>
    [MaxLength(255)]
    public string? Email { get; set; }

    /// <summary>
    /// Phone number of invitee
    /// </summary>
    [MaxLength(50)]
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Hashed token for invitation security
    /// </summary>
    [Required]
    [MaxLength(500)]
    public string? TokenHash { get; set; }

    /// <summary>
    /// Invitation status
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string? Status { get; set; } = "PENDING";

    /// <summary>
    /// Expiration date of invitation
    /// </summary>
    [Required]
    public DateTime ExpiresAt { get; set; }
    /// <summary>
    /// Date invitation was accepted
    /// </summary>
    public DateTime? AcceptedAt { get; set; }
    /// <summary>
    /// Date invitation was revoked
    /// </summary>
    public DateTime? RevokedAt { get; set; }

    /// <summary>
    /// User who captured the invitation
    /// </summary>
    [MaxLength(100)]
    public string? CapturedBy { get; set; }
    /// <summary>
    /// Date invitation was captured
    /// </summary>
    public DateTime? CapturedDate { get; set; }

    /// <summary>
    /// FK to Lease
    /// </summary>
    [ForeignKey(nameof(LeaseID))]
    public Lease? Lease { get; set; }

    /// <summary>
    /// FK to RentalApplication
    /// </summary>
    [ForeignKey(nameof(RentalApplicationID))]
    public RentalApplication? RentalApplication { get; set; }
}