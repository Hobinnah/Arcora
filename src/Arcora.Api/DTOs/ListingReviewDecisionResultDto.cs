namespace Arcora.Api.DTOs;

public sealed class ListingReviewDecisionResultDto
{
    public ListingDto Listing { get; set; } = new();
    public Guid ConversationID { get; set; }
    public bool EmailSent { get; set; }
    public string? Warning { get; set; }
}