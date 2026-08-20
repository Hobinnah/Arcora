// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;
using Arcora.Api.Entities;

namespace Arcora.Api.DTOs;
/// <summary>
/// Participants in conversations including users, tenants, and organization members.
/// </summary>
public class ConversationParticipantDto
{
    /// <summary>
    /// Key
    /// </summary>
    public Guid? ConversationParticipantID { get; set; }

    /// <summary>
    /// FK to Conversation
    /// </summary>
    [Required]
    public Guid ConversationID { get; set; }
    /// <summary>
    /// FK to User
    /// </summary>
    public long? UserID { get; set; }
    /// <summary>
    /// FK to Tenant
    /// </summary>
    public Guid? TenantID { get; set; }
    /// <summary>
    /// FK to OrganizationMember
    /// </summary>
    public Guid? OrganizationMemberID { get; set; }

    /// <summary>
    /// Role of the participant in the conversation
    /// </summary>
    [MaxLength(50)]
    public string? ParticipantRole { get; set; }

    /// <summary>
    /// Date and time participant joined
    /// </summary>
    [Required]
    public DateTime JoinedAt { get; set; }
    /// <summary>
    /// Date and time participant left
    /// </summary>
    public DateTime? LeftAt { get; set; }
    /// <summary>
    /// Date and time participant last read messages
    /// </summary>
    public DateTime? LastReadAt { get; set; }

    /// <summary>
    /// Indicates if participant is muted
    /// </summary>
    [Required]
    public bool IsMuted { get; set; }
    /// <summary>
    /// Date participant was captured
    /// </summary>
    public DateTime? CapturedDate { get; set; }
    /// <summary>
    /// FK to Conversation
    /// </summary>
    public ConversationDto? Conversation { get; set; }
    /// <summary>
    /// FK to User
    /// </summary>
    public User? User { get; set; }
    /// <summary>
    /// FK to Tenant
    /// </summary>
    public TenantDto? Tenant { get; set; }
    /// <summary>
    /// FK to OrganizationMember
    /// </summary>
    public OrganizationMemberDto? OrganizationMember { get; set; }
}