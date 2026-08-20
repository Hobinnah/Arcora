// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.DTOs;
using Arcora.Api.Models;

namespace Arcora.Api.Services.Interfaces
{
    public interface IConversationMessageService
    {
        /// <summary>
        /// Retrieves all conversation messages with optional paging support.
        /// </summary>
        /// <param name = "paging"></param>
        /// <returns></returns>
        Task<PagedResult<ConversationMessageDto>> GetAll(Paging paging);
        /// <summary>
        /// Retrieves a conversation message by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task<ConversationMessageDto?> GetID(Guid ID);
        /// <summary>
        /// Creates a new conversation message entry.
        /// </summary>
        /// <param name = "conversationMessageDto"></param>
        /// <returns></returns>
        Task<ConversationMessageDto> CreateConversationMessage(ConversationMessageDto conversationMessageDto);
        /// <summary>
        /// Updates an existing conversation message entry by its ID.
        /// </summary>
        /// <param name = "id"></param>
        /// <param name = "conversationmessageDto"></param>
        /// <returns></returns>
        Task<ConversationMessageDto?> UpdateConversationMessage(Guid id, ConversationMessageDto conversationmessageDto);
        /// <summary>
        /// Deletes a conversationmessage entry by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task DeleteConversationMessage(Guid ID);
    }
}