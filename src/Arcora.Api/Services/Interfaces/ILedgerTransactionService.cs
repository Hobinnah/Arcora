// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.DTOs;
using Arcora.Api.Models;

namespace Arcora.Api.Services.Interfaces
{
    public interface ILedgerTransactionService
    {
        /// <summary>
        /// Retrieves all ledger transactions with optional paging support.
        /// </summary>
        /// <param name = "paging"></param>
        /// <returns></returns>
        Task<PagedResult<LedgerTransactionDto>> GetAll(Paging paging);
        /// <summary>
        /// Retrieves a ledger transaction by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task<LedgerTransactionDto?> GetID(Guid ID);
        /// <summary>
        /// Creates a new ledger transaction entry.
        /// </summary>
        /// <param name = "ledgerTransactionDto"></param>
        /// <returns></returns>
        Task<LedgerTransactionDto> CreateLedgerTransaction(LedgerTransactionDto ledgerTransactionDto);
        /// <summary>
        /// Updates an existing ledger transaction entry by its ID.
        /// </summary>
        /// <param name = "id"></param>
        /// <param name = "ledgertransactionDto"></param>
        /// <returns></returns>
        Task<LedgerTransactionDto?> UpdateLedgerTransaction(Guid id, LedgerTransactionDto ledgertransactionDto);
        /// <summary>
        /// Deletes a ledgertransaction entry by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task DeleteLedgerTransaction(Guid ID);
        /// <summary>
        /// Updates the status of a ledgertransaction by its ID.
        /// </summary>
        /// <param name = "id">The unique identifier of the ledgertransaction to update.</param>
        /// <param name = "status">The new status value to assign to the ledgertransaction.</param>
        /// <returns>
        /// Returns <see cref = "LedgerTransactionDto"/> with the updated ledgertransaction if successful, or null if not found.
        /// </returns>
        Task<LedgerTransactionDto?> UpdateLedgerTransactionStatus(Guid id, string status);
    }
}