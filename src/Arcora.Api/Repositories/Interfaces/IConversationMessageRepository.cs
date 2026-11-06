// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;

namespace Arcora.Api.Repositories.Interfaces
{
    public interface IConversationMessageRepository : IRepository<ConversationMessage>
    {
        Task<Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction> BeginAttachmentSendAsync(
            Guid conversationID, CancellationToken cancellationToken = default);
        Task<ConversationMessage?> GetAttachmentSendAsync(Guid conversationID, string senderScope, Guid sendKey);
        Task<List<ConversationMessage>> GetConversationMessageAsync();
        Task<bool> HasConversationMessagesAsync();

        /// <summary>
        /// A page of a thread's non-deleted messages counted back from the newest (page 1 = latest), returned oldest to newest.
        /// With <paramref name="before"/> only messages sent before it are considered, so the client can page back with a
        /// stable cursor even while new messages arrive. TotalCount is the number of messages in that considered range.
        /// </summary>
        Task<(List<ConversationMessage> Items, int TotalCount)> GetThreadMessagesAsync(
            Guid conversationId, int pageNumber, int pageSize, DateTime? before = null);

        /// <summary>Latest non-deleted message in a thread (for the sidebar preview).</summary>
        Task<ConversationMessage?> GetLatestAsync(Guid conversationId);

        /// <summary>
        /// Count of non-deleted messages from the other side sent after <paramref name="lastReadAt"/> (unread badge).
        /// <paramref name="fromOrganization"/> = true counts host-team messages (the renter's view); false counts the renter's (the host's view).
        /// Your own side's messages never count as unread.
        /// </summary>
        Task<int> CountUnreadAsync(Guid conversationId, DateTime? lastReadAt, bool fromOrganization);
    }
}