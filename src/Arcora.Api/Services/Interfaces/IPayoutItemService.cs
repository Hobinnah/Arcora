// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.DTOs;
using Arcora.Api.Models;

namespace Arcora.Api.Services.Interfaces
{
    public interface IPayoutItemService
    {
        /// <summary>
        /// Retrieves all payout items with optional paging support.
        /// </summary>
        /// <param name = "paging"></param>
        /// <returns></returns>
        Task<PagedResult<PayoutItemDto>> GetAll(Paging paging);
        /// <summary>
        /// Retrieves a payout item by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task<PayoutItemDto?> GetID(long ID);
        /// <summary>
        /// Creates a new payout item entry.
        /// </summary>
        /// <param name = "payoutItemDto"></param>
        /// <returns></returns>
        Task<PayoutItemDto> CreatePayoutItem(PayoutItemDto payoutItemDto);
        /// <summary>
        /// Updates an existing payout item entry by its ID.
        /// </summary>
        /// <param name = "id"></param>
        /// <param name = "payoutitemDto"></param>
        /// <returns></returns>
        Task<PayoutItemDto?> UpdatePayoutItem(long id, PayoutItemDto payoutitemDto);
        /// <summary>
        /// Deletes a payoutitem entry by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task DeletePayoutItem(long ID);
    }
}