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
        Task<ConversationDto> GetOrCreateDirectThreadAsync(StartThreadRequest request);

        /// <summary>Persists a new message, pushes it live and queues notification emails to the other parties.</summary>
        Task<ConversationMessageDto> SendMessageAsync(SendMessageRequest request);

        /// <summary>Edits a message. Only the original sender may edit, and only within 5 minutes of creation.</summary>
        Task<ConversationMessageDto?> EditMessageAsync(Guid messageId, string message, ConversationActor actor);

        /// <summary>Soft-deletes a message. Only the original sender may delete, and only within 5 minutes of creation.</summary>
        Task<bool> DeleteMessageAsync(Guid messageId, ConversationActor actor);

        /// <summary>Returns paged inbox rows for the acting tenant and/or organization.</summary>
        Task<PagedResult<ConversationSummaryDto>> GetInboxAsync(InboxQuery query);

        /// <summary>Returns paged messages for a thread, oldest to newest.</summary>
        Task<PagedResult<ConversationMessageDto>> GetThreadMessagesAsync(Guid conversationId, int pageNumber, int pageSize);

        /// <summary>Marks a thread as read up to now for the acting participant.</summary>
        Task MarkReadAsync(Guid conversationId, ConversationActor actor);
    }
}
