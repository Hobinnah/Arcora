// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.DTOs;
using Arcora.Api.Models;

namespace Arcora.Api.Services.Interfaces
{
    public interface IRefundService
    {
        /// <summary>
        /// Retrieves all refunds with optional paging support.
        /// </summary>
        /// <param name = "paging"></param>
        /// <returns></returns>
        Task<PagedResult<RefundDto>> GetAll(Paging paging);
        /// <summary>
        /// Retrieves a refund by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task<RefundDto?> GetID(Guid ID);
        /// <summary>
        /// Creates a new refund entry.
        /// </summary>
        /// <param name = "refundDto"></param>
        /// <returns></returns>
        Task<RefundDto> CreateRefund(RefundDto refundDto);
        /// <summary>
        /// Updates an existing refund entry by its ID.
        /// </summary>
        /// <param name = "id"></param>
        /// <param name = "refundDto"></param>
        /// <returns></returns>
        Task<RefundDto?> UpdateRefund(Guid id, RefundDto refundDto);
        /// <summary>
        /// Deletes a refund entry by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task DeleteRefund(Guid ID);
        /// <summary>
        /// Updates the status of a refund by its ID.
        /// </summary>
        /// <param name = "id">The unique identifier of the refund to update.</param>
        /// <param name = "status">The new status value to assign to the refund.</param>
        /// <returns>
        /// Returns <see cref = "RefundDto"/> with the updated refund if successful, or null if not found.
        /// </returns>
        Task<RefundDto?> UpdateRefundStatus(Guid id, string status);
    }
}