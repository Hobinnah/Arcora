namespace Arcora.Api.DTOs
{
    /// <summary>
    /// Lightweight left-sidebar row for the messaging inbox. Intentionally does not hydrate the full
    /// message graph - only the counterparty label, last-message preview and unread count are returned.
    /// </summary>
    public sealed class ConversationSummaryDto
    {
        public Guid ConversationID { get; set; }
        public Guid? TenantID { get; set; }
        public Guid? OrganizationID { get; set; }
        public string? Subject { get; set; }
        public string? Status { get; set; }

        public string? CounterpartyName { get; set; }
        public string? CounterpartyPhotoUrl { get; set; }

        public string? LastMessagePreview { get; set; }
        public DateTime? LastMessageAt { get; set; }
        public int UnreadCount { get; set; }
    }
}
