using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arcora.Api.Entities;

/// <summary>
/// An emoji reaction to a conversation message. Each participant (tenant or organization member) has at most one per message.
/// </summary>
[Table("ConversationMessageReaction")]
public class ConversationMessageReaction
{
    [Key]
    public Guid ConversationMessageReactionID { get; set; }

    [Required]
    public Guid ConversationMessageID { get; set; }

    public Guid? ReactorTenantID { get; set; }

    public Guid? ReactorOrganizationMemberID { get; set; }

    [Required]
    [MaxLength(32)]
    public string Emoji { get; set; } = string.Empty;

    [Required]
    public DateTime CreatedAt { get; set; }

    [ForeignKey(nameof(ConversationMessageID))]
    public ConversationMessage? ConversationMessage { get; set; }

    [ForeignKey(nameof(ReactorTenantID))]
    public Tenant? ReactorTenant { get; set; }

    [ForeignKey(nameof(ReactorOrganizationMemberID))]
    public OrganizationMember? ReactorOrganizationMember { get; set; }
}
