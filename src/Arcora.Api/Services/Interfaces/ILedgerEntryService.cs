// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.DTOs;
using Arcora.Api.Models;

namespace Arcora.Api.Services.Interfaces
{
    public interface ILedgerEntryService
    {
        /// <summary>
        /// Retrieves all ledger entries with optional paging support.
        /// </summary>
        /// <param name = "paging"></param>
        /// <returns></returns>
        Task<PagedResult<LedgerEntryDto>> GetAll(Paging paging);
        /// <summary>
        /// Retrieves a ledger entry by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task<LedgerEntryDto?> GetID(Guid ID);
        /// <summary>
        /// Creates a new ledger entry entry.
        /// </summary>
        /// <param name = "ledgerEntryDto"></param>
        /// <returns></returns>
        Task<LedgerEntryDto> CreateLedgerEntry(LedgerEntryDto ledgerEntryDto);
        /// <summary>
        /// Updates an existing ledger entry entry by its ID.
        /// </summary>
        /// <param name = "id"></param>
        /// <param name = "ledgerentryDto"></param>
        /// <returns></returns>
        Task<LedgerEntryDto?> UpdateLedgerEntry(Guid id, LedgerEntryDto ledgerentryDto);
        /// <summary>
        /// Deletes a ledgerentry entry by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task DeleteLedgerEntry(Guid ID);
    }
}