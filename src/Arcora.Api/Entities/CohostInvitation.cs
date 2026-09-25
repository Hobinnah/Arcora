using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arcora.Api.Entities;

/// <summary>
/// Persists cohost invitations for organization listing access.
/// </summary>
[Table("CohostInvitation")]
public class CohostInvitation
{
    [Key]
    public Guid CohostInvitationID { get; set; }

    [Required]
    public Guid OrganizationID { get; set; }

    [MaxLength(255)]
    [Required]
    public string? Email { get; set; }

    [MaxLength(150)]
    public string? CohostName { get; set; }

    [MaxLength(50)]
    [Required]
    public string? PhoneNumber { get; set; }

    [MaxLength(100)]
    [Required]
    public string? CohostAccess { get; set; }

    [MaxLength(500)]
    [Required]
    public string? TokenHash { get; set; }

    [MaxLength(50)]
    [Required]
    public string? Status { get; set; } = "PENDING";

    [Required]
    public DateTime ExpiresAt { get; set; }

    public DateTime? AcceptedAt { get; set; }

    public DateTime? DeclinedAt { get; set; }

    public DateTime? RevokedAt { get; set; }

    public DateTime? CapturedDate { get; set; }

    [MaxLength(100)]
    public string? CapturedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    [MaxLength(100)]
    public string? UpdatedBy { get; set; }

    [ForeignKey(nameof(OrganizationID))]
    public Organization? Organization { get; set; }
}
