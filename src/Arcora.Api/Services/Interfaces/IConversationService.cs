// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.DTOs;
using Arcora.Api.Models;

namespace Arcora.Api.Services.Interfaces
{
    public interface IConversationService
    {
        /// <summary>
        /// Retrieves all conversations with optional paging support.
        /// </summary>
        /// <param name = "paging"></param>
        /// <returns></returns>
        Task<PagedResult<ConversationDto>> GetAll(Paging paging);
        /// <summary>
        /// Retrieves a conversation by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task<ConversationDto?> GetID(Guid ID);
        /// <summary>
        /// Creates a new conversation entry.
        /// </summary>
        /// <param name = "conversationDto"></param>
        /// <returns></returns>
        Task<ConversationDto> CreateConversation(ConversationDto conversationDto);
        /// <summary>
        /// Updates an existing conversation entry by its ID.
        /// </summary>
        /// <param name = "id"></param>
        /// <param name = "conversationDto"></param>
        /// <returns></returns>
        Task<ConversationDto?> UpdateConversation(Guid id, ConversationDto conversationDto);
        /// <summary>
        /// Deletes a conversation entry by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task DeleteConversation(Guid ID);
        /// <summary>
        /// Updates the status of a conversation by its ID.
        /// </summary>
        /// <param name = "id">The unique identifier of the conversation to update.</param>
        /// <param name = "status">The new status value to assign to the conversation.</param>
        /// <returns>
        /// Returns <see cref = "ConversationDto"/> with the updated conversation if successful, or null if not found.
        /// </returns>
        Task<ConversationDto?> UpdateConversationStatus(Guid id, string status);
    }
}