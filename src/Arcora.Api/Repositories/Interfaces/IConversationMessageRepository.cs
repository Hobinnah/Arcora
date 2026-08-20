// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;

namespace Arcora.Api.Repositories.Interfaces
{
    public interface IConversationMessageRepository : IRepository<ConversationMessage>
    {
        Task<List<ConversationMessage>> GetConversationMessageAsync();
        Task<bool> HasConversationMessagesAsync();
    }
}