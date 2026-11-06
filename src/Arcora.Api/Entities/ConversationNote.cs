using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arcora.Api.Entities;

/// <summary>
/// A private note the host organization keeps about a conversation. One note per conversation,
/// shared by every member of the host organization and never visible to the tenant.
/// </summary>
[Table("ConversationNote")]
public class ConversationNote
{
    public const int MaxLength = 2000;

    [Key]
    public Guid ConversationNoteID { get; set; }

    [Required]
    public Guid ConversationID { get; set; }

    [Required]
    [MaxLength(MaxLength)]
    public string Note { get; set; } = string.Empty;

    public Guid? UpdatedByOrganizationMemberID { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; }

    [Required]
    public DateTime UpdatedAt { get; set; }

    [ForeignKey(nameof(ConversationID))]
    public Conversation? Conversation { get; set; }

    [ForeignKey(nameof(UpdatedByOrganizationMemberID))]
    public OrganizationMember? UpdatedByOrganizationMember { get; set; }
}
