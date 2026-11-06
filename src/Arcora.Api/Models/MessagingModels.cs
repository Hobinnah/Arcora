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
        /// <summary>When provided, resolves the tenant from authentication and the organisation from this listing.</summary>
        public Guid? ListingID { get; set; }
        /// <summary>Listing enquiries only: the renter's opening message, posted before the listing card.</summary>
        public string? Message { get; set; }
        /// <summary>Listing enquiries only: proposed move-in date.</summary>
        public DateOnly? ProposedStartDate { get; set; }
        /// <summary>Listing enquiries only: proposed move-out date (after the start date).</summary>
        public DateOnly? ProposedEndDate { get; set; }
    }

    /// <summary>Saves the host team's private note for a conversation; a blank note clears it.</summary>
    public sealed class SaveConversationNoteRequest
    {
        public string? Note { get; set; }
        public ConversationActor Actor { get; set; } = new();
    }

    /// <summary>A persisted listing snapshot displayed in an enquiry message.</summary>
    public sealed record ListingEnquiryCard(
        Guid ListingID,
        string Title,
        decimal MonthlyRent,
        string Currency,
        Guid? PhotoID,
        DateOnly? ProposedStartDate = null,
        DateOnly? ProposedEndDate = null,
        decimal? SecurityDeposit = null);

    /// <summary>
    /// Request to send a message into a conversation. Exactly one sender side should be populated.
    /// </summary>
    public sealed class SendMessageRequest
    {
        public Guid ConversationID { get; set; }
        public Guid? AttachmentSendKey { get; set; }
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

    /// <summary>Toggles the actor's emoji reaction; reacting with a different emoji replaces the previous one.</summary>
    public sealed class ReactToMessageRequest
    {
        public string? Emoji { get; set; }
        public ConversationActor Actor { get; set; } = new();
    }

    /// <summary>Block, unblock or report a conversation as one of its participants.</summary>
    public sealed class ConversationModerationRequest
    {
        public ConversationActor Actor { get; set; } = new();
        /// <summary>Reports only: what went wrong, shown to the support team.</summary>
        public string? Reason { get; set; }
    }

    /// <summary>Multipart upload of a photo or PDF into a conversation.</summary>
    public sealed class SendAttachmentRequest
    {
        public Microsoft.AspNetCore.Http.IFormFile? File { get; set; }
        public Guid? AttachmentSendKey { get; set; }
        public Guid ConversationID { get; set; }
        public Guid? ReplyToMessageID { get; set; }
        public long? SenderUserID { get; set; }
        public Guid? SenderTenantID { get; set; }
        public Guid? SenderOrganizationMemberID { get; set; }
    }

    /// <summary>The JSON body stored in an ATTACHMENT message.</summary>
    public sealed record MessageAttachment(string BlobName, string Category, string Name, string ContentType, long Size);

    public static class ConversationStatuses
    {
        public const string Open = "OPEN";
        public const string BlockedByTenant = "BLOCKED_BY_TENANT";
        public const string BlockedByHost = "BLOCKED_BY_HOST";

        public static bool IsBlocked(string? status) =>
            string.Equals(status, BlockedByTenant, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(status, BlockedByHost, StringComparison.OrdinalIgnoreCase);
    }

    public static class MessageReactionEmojis
    {
        public static readonly IReadOnlySet<string> Allowed = new HashSet<string>(StringComparer.Ordinal)
        {
            "👍", "❤️", "😂", "😮", "😢", "🙏", "🎉", "👏"
        };
    }
}
