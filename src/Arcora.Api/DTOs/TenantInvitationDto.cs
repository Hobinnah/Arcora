// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;
using Arcora.Api.Entities;

namespace Arcora.Api.DTOs;
/// <summary>
/// Manages invitations sent to tenants for lease or rental application purposes with status tracking.
/// </summary>
public class TenantInvitationDto
{
    /// <summary>
    /// Key
    /// </summary>
    public Guid? TenantInvitationID { get; set; }
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
    public LeaseDto? Lease { get; set; }
    /// <summary>
    /// FK to RentalApplication
    /// </summary>
    public RentalApplicationDto? RentalApplication { get; set; }
}