// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.DTOs;
using Arcora.Api.Models;

namespace Arcora.Api.Services.Interfaces
{
    public interface IPayoutService
    {
        /// <summary>
        /// Retrieves all payouts with optional paging support.
        /// </summary>
        /// <param name = "paging"></param>
        /// <returns></returns>
        Task<PagedResult<PayoutDto>> GetAll(Paging paging);
        /// <summary>
        /// Retrieves a payout by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task<PayoutDto?> GetID(long ID);
        /// <summary>
        /// Creates a new payout entry.
        /// </summary>
        /// <param name = "payoutDto"></param>
        /// <returns></returns>
        Task<PayoutDto> CreatePayout(PayoutDto payoutDto);
        /// <summary>
        /// Updates an existing payout entry by its ID.
        /// </summary>
        /// <param name = "id"></param>
        /// <param name = "payoutDto"></param>
        /// <returns></returns>
        Task<PayoutDto?> UpdatePayout(long id, PayoutDto payoutDto);
        /// <summary>
        /// Deletes a payout entry by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task DeletePayout(long ID);
        /// <summary>
        /// Updates the status of a payout by its ID.
        /// </summary>
        /// <param name = "id">The unique identifier of the payout to update.</param>
        /// <param name = "status">The new status value to assign to the payout.</param>
        /// <returns>
        /// Returns <see cref = "PayoutDto"/> with the updated payout if successful, or null if not found.
        /// </returns>
        Task<PayoutDto?> UpdatePayoutStatus(long id, string status);
    }
}