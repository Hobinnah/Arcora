// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;

namespace Arcora.Api.Repositories.Interfaces
{
    public interface IConversationParticipantRepository : IRepository<ConversationParticipant>
    {
        Task<List<ConversationParticipant>> GetConversationParticipantAsync();
        Task<bool> HasConversationParticipantsAsync();
    }
}