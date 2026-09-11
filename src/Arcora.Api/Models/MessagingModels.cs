namespace Arcora.Api.Models
{
    /// <summary>
    /// Request to open (or reuse) a direct host-to-tenant conversation thread.
    /// </summary>
    public sealed class StartThreadRequest
    {
        public Guid TenantID { get; set; }
        public Guid OrganizationID { get; set; }
        public string? Subject { get; set; }
    }

    /// <summary>
    /// Request to send a message into a conversation. Exactly one sender side should be populated.
    /// </summary>
    public sealed class SendMessageRequest
    {
        public Guid ConversationID { get; set; }
        public string? Message { get; set; }
        public string MessageType { get; set; } = "TEXT";
        public Guid? ReplyToMessageID { get; set; }

        public long? SenderUserID { get; set; }
        public Guid? SenderTenantID { get; set; }
        public Guid? SenderOrganizationMemberID { get; set; }
    }

    /// <summary>
    /// Inbox query: threads for a user acting as a tenant and/or as a host organization member.
    /// </summary>
    public sealed class InboxQuery : Paging
    {
        public Guid? TenantID { get; set; }
        public Guid? OrganizationID { get; set; }
        public bool UnreadOnly { get; set; }
    }

    /// <summary>
    /// Request to edit an existing message. Only the original sender may edit, within 5 minutes of sending.
    /// </summary>
    public sealed class EditMessageRequest
    {
        public string? Message { get; set; }
        public ConversationActor Actor { get; set; } = new();
    }
}
