// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.DTOs;
using Arcora.Api.Models;

namespace Arcora.Api.Services.Interfaces
{
    public interface IDisputeService
    {
        /// <summary>
        /// Retrieves all disputes with optional paging support.
        /// </summary>
        /// <param name = "paging"></param>
        /// <returns></returns>
        Task<PagedResult<DisputeDto>> GetAll(Paging paging);
        /// <summary>
        /// Retrieves a dispute by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task<DisputeDto?> GetID(Guid ID);
        /// <summary>
        /// Creates a new dispute entry.
        /// </summary>
        /// <param name = "disputeDto"></param>
        /// <returns></returns>
        Task<DisputeDto> CreateDispute(DisputeDto disputeDto);
        /// <summary>
        /// Updates an existing dispute entry by its ID.
        /// </summary>
        /// <param name = "id"></param>
        /// <param name = "disputeDto"></param>
        /// <returns></returns>
        Task<DisputeDto?> UpdateDispute(Guid id, DisputeDto disputeDto);
        /// <summary>
        /// Deletes a dispute entry by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task DeleteDispute(Guid ID);
        /// <summary>
        /// Updates the status of a dispute by its ID.
        /// </summary>
        /// <param name = "id">The unique identifier of the dispute to update.</param>
        /// <param name = "status">The new status value to assign to the dispute.</param>
        /// <returns>
        /// Returns <see cref = "DisputeDto"/> with the updated dispute if successful, or null if not found.
        /// </returns>
        Task<DisputeDto?> UpdateDisputeStatus(Guid id, string status);
    }
}