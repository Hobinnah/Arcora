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
        public Guid? ListingID { get; set; }
        public string? Subject { get; set; }
        public string? Status { get; set; }

        public string? CounterpartyName { get; set; }
        public string? CounterpartyPhotoUrl { get; set; }
        /// <summary>First names of the conversation participants, counterparty side first (organization members, then tenant for renters; tenant, then members for hosts).</summary>
        public List<string> ParticipantNames { get; set; } = new();
        /// <summary>Renter's first name; with <see cref="OrganizationName"/> it forms the inbox row label ("Elijah, Grinding Breeze").</summary>
        public string? TenantFirstName { get; set; }
        /// <summary>Display name (or legal name) of the organization that owns the conversation.</summary>
        public string? OrganizationName { get; set; }

        public string? LastMessagePreview { get; set; }
        public DateTime? LastMessageAt { get; set; }
        public int UnreadCount { get; set; }
    }
}
