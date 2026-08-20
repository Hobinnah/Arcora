// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.DTOs;
using Arcora.Api.Models;

namespace Arcora.Api.Services.Interfaces
{
    public interface IAttachmentService
    {
        /// <summary>
        /// Retrieves all attachments with optional paging support.
        /// </summary>
        /// <param name = "paging"></param>
        /// <returns></returns>
        Task<PagedResult<AttachmentDto>> GetAll(Paging paging);
        /// <summary>
        /// Retrieves a attachment by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task<AttachmentDto?> GetID(Guid ID);
        /// <summary>
        /// Creates a new attachment entry.
        /// </summary>
        /// <param name = "attachmentDto"></param>
        /// <returns></returns>
        Task<AttachmentDto> CreateAttachment(AttachmentDto attachmentDto);
        /// <summary>
        /// Updates an existing attachment entry by its ID.
        /// </summary>
        /// <param name = "id"></param>
        /// <param name = "attachmentDto"></param>
        /// <returns></returns>
        Task<AttachmentDto?> UpdateAttachment(Guid id, AttachmentDto attachmentDto);
        /// <summary>
        /// Deletes a attachment entry by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task DeleteAttachment(Guid ID);
    }
}