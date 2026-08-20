// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;

namespace Arcora.Api.Repositories.Interfaces
{
    public interface IConversationRepository : IRepository<Conversation>
    {
        Task<List<Conversation>> GetConversationAsync();
        Task<bool> HasConversationsAsync();
    }
}