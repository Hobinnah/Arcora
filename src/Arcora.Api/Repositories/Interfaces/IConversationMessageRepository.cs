// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;

namespace Arcora.Api.Repositories.Interfaces
{
    public interface IConversationMessageRepository : IRepository<ConversationMessage>
    {
        Task<List<ConversationMessage>> GetConversationMessageAsync();
        Task<bool> HasConversationMessagesAsync();

        /// <summary>Paged messages for a thread, ascending by SentAt (oldest to newest), excluding deleted.</summary>
        Task<(List<ConversationMessage> Items, int TotalCount)> GetThreadMessagesAsync(
            Guid conversationId, int pageNumber, int pageSize);

        /// <summary>Latest non-deleted message in a thread (for the sidebar preview).</summary>
        Task<ConversationMessage?> GetLatestAsync(Guid conversationId);

        /// <summary>Count of non-deleted messages after a timestamp, optionally excluding a sender (unread badge).</summary>
        Task<int> CountUnreadAsync(Guid conversationId, DateTime? lastReadAt, long? excludeSenderUserId);
    }
}