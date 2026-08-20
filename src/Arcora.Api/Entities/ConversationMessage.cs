// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arcora.Api.Entities;
/// <summary>
/// Stores messages exchanged in conversations between users, tenants, or organization members.
/// </summary>
[Table("ConversationMessage")]
public class ConversationMessage
{
    /// <summary>
    /// Primary key
    /// </summary>
    [Key]
    public Guid ConversationMessageID { get; set; }

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
    [ForeignKey(nameof(ConversationID))]
    public Conversation? Conversation { get; set; }

    /// <summary>
    /// FK to User
    /// </summary>
    [ForeignKey(nameof(SenderUserID))]
    public User? SenderUser { get; set; }

    /// <summary>
    /// FK to Tenant
    /// </summary>
    [ForeignKey(nameof(SenderTenantID))]
    public Tenant? SenderTenant { get; set; }

    /// <summary>
    /// FK to OrganizationMember
    /// </summary>
    [ForeignKey(nameof(SenderOrganizationMemberID))]
    public OrganizationMember? SenderOrganizationMember { get; set; }

    /// <summary>
    /// FK to ConversationMessage (reply)
    /// </summary>
    [ForeignKey(nameof(ReplyToMessageID))]
    public ConversationMessage? ReplyToMessageConversationMessage { get; set; }
}