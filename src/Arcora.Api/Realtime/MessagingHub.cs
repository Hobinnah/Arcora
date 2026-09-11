using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Arcora.Api.Realtime
{
    /// <summary>
    /// Real-time messaging hub. Clients join a per-conversation group to receive live
    /// <c>MessageReceived</c>, <c>MessageEdited</c> and <c>MessageDeleted</c> events.
    /// </summary>
    [Authorize]
    public class MessagingHub : Hub
    {
        /// <summary>Group name convention for a conversation.</summary>
        public static string GroupName(Guid conversationId) => $"conv:{conversationId}";

        /// <summary>Client joins a conversation group to receive its live message events.</summary>
        public Task JoinConversation(Guid conversationId)
            => Groups.AddToGroupAsync(Context.ConnectionId, GroupName(conversationId));

        /// <summary>Client leaves a conversation group.</summary>
        public Task LeaveConversation(Guid conversationId)
            => Groups.RemoveFromGroupAsync(Context.ConnectionId, GroupName(conversationId));
    }
}
