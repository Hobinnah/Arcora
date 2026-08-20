// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;
using Arcora.Api.Entities;

namespace Arcora.Api.DTOs;
/// <summary>
/// Stores messages exchanged in conversations between users, tenants, or organization members.
/// </summary>
public class ConversationMessageDto
{
    /// <summary>
    /// Key
    /// </summary>
    public Guid? ConversationMessageID { get; set; }

    /// <summary>
    /// FK to Conversation
    /// </summary>
    [Required]
    public Guid ConversationID { get; set; }
    /// <summary>
    /// FK to User
    /// </summary>
    public long? SenderUserID { get; set; }
    /// <summary>
    /// FK to Tenant
    /// </summary>
    public Guid? SenderTenantID { get; set; }
    /// <summary>
    /// FK to OrganizationMember
    /// </summary>
    public Guid? SenderOrganizationMemberID { get; set; }

    /// <summary>
    /// Message content
    /// </summary>
    [Required]
    [MaxLength(256)]
    public string? Message { get; set; }

    /// <summary>
    /// Type of message
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string? MessageType { get; set; } = "TEXT";
    /// <summary>
    /// FK to ConversationMessage (reply)
    /// </summary>
    public Guid? ReplyToMessageID { get; set; }

    /// <summary>
    /// Message sent timestamp
    /// </summary>
    [Required]
    public DateTime SentAt { get; set; }
    /// <summary>
    /// Message edited timestamp
    /// </summary>
    public DateTime? EditedAt { get; set; }
    /// <summary>
    /// Message deleted timestamp
    /// </summary>
    public DateTime? DeletedAt { get; set; }
    /// <summary>
    /// Record captured date
    /// </summary>
    public DateTime? CapturedDate { get; set; }

    /// <summary>
    /// User who captured the record
    /// </summary>
    [MaxLength(256)]
    public string? CapturedBy { get; set; }
    /// <summary>
    /// FK to Conversation
    /// </summary>
    public ConversationDto? Conversation { get; set; }
    /// <summary>
    /// FK to User
    /// </summary>
    public User? SenderUser { get; set; }
    /// <summary>
    /// FK to Tenant
    /// </summary>
    public TenantDto? SenderTenant { get; set; }
    /// <summary>
    /// FK to OrganizationMember
    /// </summary>
    public OrganizationMemberDto? SenderOrganizationMember { get; set; }
    /// <summary>
    /// FK to ConversationMessage (reply)
    /// </summary>
    public ConversationMessageDto? ReplyToMessageConversationMessage { get; set; }
}