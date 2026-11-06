namespace Arcora.Api.DTOs;

public class ConversationMessageReactionDto
{
    public string Emoji { get; set; } = string.Empty;
    public Guid? ReactorTenantID { get; set; }
    public Guid? ReactorOrganizationMemberID { get; set; }
}
