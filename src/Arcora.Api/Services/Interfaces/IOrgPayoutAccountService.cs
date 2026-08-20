// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.DTOs;
using Arcora.Api.Models;

namespace Arcora.Api.Services.Interfaces
{
    public interface IOrgPayoutAccountService
    {
        /// <summary>
        /// Retrieves all org payout accounts with optional paging support.
        /// </summary>
        /// <param name = "paging"></param>
        /// <returns></returns>
        Task<PagedResult<OrgPayoutAccountDto>> GetAll(Paging paging);
        /// <summary>
        /// Retrieves a org payout account by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task<OrgPayoutAccountDto?> GetID(long ID);
        /// <summary>
        /// Creates a new org payout account entry.
        /// </summary>
        /// <param name = "orgPayoutAccountDto"></param>
        /// <returns></returns>
        Task<OrgPayoutAccountDto> CreateOrgPayoutAccount(OrgPayoutAccountDto orgPayoutAccountDto);
        /// <summary>
        /// Updates an existing org payout account entry by its ID.
        /// </summary>
        /// <param name = "id"></param>
        /// <param name = "orgpayoutaccountDto"></param>
        /// <returns></returns>
        Task<OrgPayoutAccountDto?> UpdateOrgPayoutAccount(long id, OrgPayoutAccountDto orgpayoutaccountDto);
        /// <summary>
        /// Deletes a orgpayoutaccount entry by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task DeleteOrgPayoutAccount(long ID);
    }
}