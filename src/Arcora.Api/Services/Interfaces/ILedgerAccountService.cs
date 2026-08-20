// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.DTOs;
using Arcora.Api.Models;

namespace Arcora.Api.Services.Interfaces
{
    public interface ILedgerAccountService
    {
        /// <summary>
        /// Retrieves all ledger accounts with optional paging support.
        /// </summary>
        /// <param name = "paging"></param>
        /// <returns></returns>
        Task<PagedResult<LedgerAccountDto>> GetAll(Paging paging);
        /// <summary>
        /// Retrieves a ledger account by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task<LedgerAccountDto?> GetID(Guid ID);
        /// <summary>
        /// Creates a new ledger account entry.
        /// </summary>
        /// <param name = "ledgerAccountDto"></param>
        /// <returns></returns>
        Task<LedgerAccountDto> CreateLedgerAccount(LedgerAccountDto ledgerAccountDto);
        /// <summary>
        /// Updates an existing ledger account entry by its ID.
        /// </summary>
        /// <param name = "id"></param>
        /// <param name = "ledgeraccountDto"></param>
        /// <returns></returns>
        Task<LedgerAccountDto?> UpdateLedgerAccount(Guid id, LedgerAccountDto ledgeraccountDto);
        /// <summary>
        /// Deletes a ledgeraccount entry by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task DeleteLedgerAccount(Guid ID);
    }
}