using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Arcora.Api.Realtime
{
    /// <summary>
    /// Real-time messaging hub. Clients join a per-conversation group to receive live
    /// <c>MessageReceived</c>, <c>MessageEdited</c> and <c>MessageDeleted</c> events.
    /// </summary>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class MessagingHub(ArcoraDbContext db) : Hub
    {
        /// <summary>Group name convention for a conversation.</summary>
        public static string GroupName(Guid conversationId) => $"conv:{conversationId}";

        /// <summary>Client joins a conversation group to receive its live message events.</summary>
        public async Task JoinConversation(Guid conversationId)
        {
            await AuthorizeConversationAsync(conversationId);
            await Groups.AddToGroupAsync(Context.ConnectionId, GroupName(conversationId));
        }

        /// <summary>Client leaves a conversation group.</summary>
        public Task LeaveConversation(Guid conversationId)
            => Groups.RemoveFromGroupAsync(Context.ConnectionId, GroupName(conversationId));

        /// <summary>Relays a short-lived "is typing" signal to the other members of a conversation group.</summary>
        public async Task Typing(Guid conversationId, string? name)
        {
            await AuthorizeConversationAsync(conversationId);
            var displayName = string.IsNullOrWhiteSpace(name) ? null : name.Trim();
            if (displayName?.Length > 40)
                displayName = displayName[..40];
            await Clients.OthersInGroup(GroupName(conversationId))
                .SendAsync("UserTyping", new { ConversationID = conversationId, Name = displayName });
        }

        private async Task AuthorizeConversationAsync(Guid conversationId)
        {
            var claim = Context.User?.FindFirst("UserId")?.Value ??
                Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!long.TryParse(claim, out var userId) || !await db.Conversations.AsNoTracking().AnyAsync(
                conversation => conversation.ConversationID == conversationId &&
                    (conversation.Tenant != null && conversation.Tenant.UserID == userId ||
                     db.OrganizationMembers.Any(member => member.OrganizationID == conversation.OrganizationID &&
                         member.UserID == userId && member.Status == "ACTIVE")), Context.ConnectionAborted))
                throw new HubException("Conversation access denied.");
        }
    }
}
