using Arcora.Api.DTOs;
using Arcora.Api.Models;

namespace Arcora.Api.Services.Interfaces
{
    /// <summary>
    /// Orchestrates the direct host-to-tenant messaging experience: opening/reusing a thread keyed by
    /// (tenant, organization), sending/editing/deleting messages (edits and deletes are limited to a
    /// 5-minute window after creation), inbox and thread queries, read tracking, real-time delivery and
    /// email notifications to the tenant/user and every active organization member.
    /// </summary>
    public interface IMessagingService
    {
        /// <summary>Returns the existing DIRECT thread for a (tenant, organization) pair, creating one if none exists.</summary>
        Task<ConversationDto> GetOrCreateDirectThreadAsync(StartThreadRequest request, long? userID = null, CancellationToken cancellationToken = default);

        /// <summary>Persists a new message, pushes it live and queues notification emails to the other parties.</summary>
        Task<ConversationMessageDto> SendMessageAsync(SendMessageRequest request);

        /// <summary>
        /// A participant's text message from the chat UI: the sender must belong to the thread, the thread must not be
        /// blocked, and each sender is rate limited. Replies must quote a message from the same thread.
        /// </summary>
        Task<ConversationMessageDto> SendUserMessageAsync(SendMessageRequest request);

        /// <summary>
        /// Validates and stores a photo or PDF with the same rules as <see cref="SendUserMessageAsync"/>.
        /// AttachmentSendKey is required and scoped to the validated sender and conversation. Replays return the original
        /// message without another upload or notification; reuse with different file bytes, name or reply returns 409.
        /// </summary>
        Task<ConversationMessageDto> SendAttachmentAsync(SendMessageRequest request, Stream content, string fileName, long length, CancellationToken cancellationToken = default);

        /// <summary>Stops new messages in the thread. Only the blocking side can unblock it.</summary>
        Task<string> SetBlockedAsync(Guid conversationId, ConversationActor actor, bool blocked);

        /// <summary>Flags the thread to the support team with the participant's reason.</summary>
        Task ReportConversationAsync(Guid conversationId, ConversationActor actor, string reason);

        /// <summary>Edits a message. Only the original sender may edit, and only within 5 minutes of creation.</summary>
        Task<ConversationMessageDto?> EditMessageAsync(Guid messageId, string message, ConversationActor actor);

        /// <summary>Soft-deletes a message. Only the original sender may delete, and only within 5 minutes of creation.</summary>
        Task<bool> DeleteMessageAsync(Guid messageId, ConversationActor actor);

        /// <summary>
        /// Adds, replaces, or removes (same emoji again) the actor's reaction. The actor must be the thread's tenant or an
        /// active member of its organization. Returns null when the message does not exist.
        /// </summary>
        Task<List<ConversationMessageReactionDto>?> ReactToMessageAsync(Guid messageId, string emoji, ConversationActor actor);

        /// <summary>Returns paged inbox rows for the acting tenant and/or organization.</summary>
        Task<PagedResult<ConversationSummaryDto>> GetInboxAsync(InboxQuery query);

        /// <summary>Number of the inbox's conversations with unread messages from the other side (drives the nav "Messages" dot).</summary>
        Task<int> CountUnreadConversationsAsync(Guid? tenantId, Guid? organizationId);

        /// <summary>
        /// Returns the newest page of a thread (or the page before <paramref name="before"/>), oldest to newest within the page.
        /// </summary>
        Task<PagedResult<ConversationMessageDto>> GetThreadMessagesAsync(Guid conversationId, int pageNumber, int pageSize, DateTime? before = null);

        /// <summary>Marks a thread as read up to now for the acting participant and pushes the receipt live.</summary>
        Task MarkReadAsync(Guid conversationId, ConversationActor actor);

        /// <summary>Returns the latest read position of every participant who has opened the thread.</summary>
        Task<List<ConversationReadReceiptDto>> GetReadReceiptsAsync(Guid conversationId);
    }
}
