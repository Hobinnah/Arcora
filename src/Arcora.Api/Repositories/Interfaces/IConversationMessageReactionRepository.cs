using Arcora.Api.Entities;

namespace Arcora.Api.Repositories.Interfaces
{
    public interface IConversationMessageReactionRepository : IRepository<ConversationMessageReaction>
    {
        /// <summary>Reactions for the given messages, oldest first.</summary>
        Task<List<ConversationMessageReaction>> GetForMessagesAsync(IReadOnlyCollection<Guid> messageIds);

        /// <summary>
        /// Adds the reactor's emoji, replaces a different one, or removes it when it is the same emoji.
        /// Returns the message's reactions after the change.
        /// </summary>
        Task<List<ConversationMessageReaction>> ToggleAsync(Guid messageId, string emoji, Guid? tenantId, Guid? organizationMemberId);
    }
}
