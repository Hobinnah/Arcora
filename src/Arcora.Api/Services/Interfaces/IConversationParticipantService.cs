// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.DTOs;
using Arcora.Api.Models;

namespace Arcora.Api.Services.Interfaces
{
    public interface IConversationParticipantService
    {
        /// <summary>
        /// Retrieves all conversation participants with optional paging support.
        /// </summary>
        /// <param name = "paging"></param>
        /// <returns></returns>
        Task<PagedResult<ConversationParticipantDto>> GetAll(Paging paging);
        /// <summary>
        /// Retrieves a conversation participant by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task<ConversationParticipantDto?> GetID(Guid ID);
        /// <summary>
        /// Creates a new conversation participant entry.
        /// </summary>
        /// <param name = "conversationParticipantDto"></param>
        /// <returns></returns>
        Task<ConversationParticipantDto> CreateConversationParticipant(ConversationParticipantDto conversationParticipantDto);
        /// <summary>
        /// Updates an existing conversation participant entry by its ID.
        /// </summary>
        /// <param name = "id"></param>
        /// <param name = "conversationparticipantDto"></param>
        /// <returns></returns>
        Task<ConversationParticipantDto?> UpdateConversationParticipant(Guid id, ConversationParticipantDto conversationparticipantDto);
        /// <summary>
        /// Deletes a conversationparticipant entry by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task DeleteConversationParticipant(Guid ID);
    }
}