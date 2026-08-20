// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arcora.Api.Entities;
/// <summary>
/// Represents members of an organization with roles and status.
/// </summary>
[Table("OrganizationMember")]
public class OrganizationMember
{
    /// <summary>
    /// Primary key
    /// </summary>
    [Key]
    public Guid OrganizationMemberID { get; set; }

    /// <summary>
    /// FK to Organization
    /// </summary>
    [Required]
    public Guid OrganizationID { get; set; }

    /// <summary>
    /// FK to User
    /// </summary>
    [Required]
    public long UserID { get; set; }

    /// <summary>
    /// Role name of the member
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string? RoleName { get; set; }

    /// <summary>
    /// Status of the organization member
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string? Status { get; set; } = "INVITED";

    /// <summary>
    /// Indicates if the member is the primary owner
    /// </summary>
    [Required]
    public bool IsPrimaryOwner { get; set; }
    /// <summary>
    /// Date when the member was invited
    /// </summary>
    public DateTime? InvitedAt { get; set; }
    /// <summary>
    /// Date when the member accepted the invitation
    /// </summary>
    public DateTime? AcceptedAt { get; set; }
    /// <summary>
    /// Date when the member was deactivated
    /// </summary>
    public DateTime? DeactivatedAt { get; set; }
    /// <summary>
    /// Date when the record was captured
    /// </summary>
    public DateTime? CapturedDate { get; set; }

    /// <summary>
    /// User who captured the record
    /// </summary>
    [MaxLength(100)]
    public string? CapturedBy { get; set; }
    /// <summary>
    /// Date when the record was last updated
    /// </summary>
    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// User who last updated the record
    /// </summary>
    [MaxLength(100)]
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// FK to Organization
    /// </summary>
    [ForeignKey(nameof(OrganizationID))]
    public Organization? Organization { get; set; }

    /// <summary>
    /// FK to User
    /// </summary>
    [ForeignKey(nameof(UserID))]
    public User? User { get; set; }
}