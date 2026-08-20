// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.DTOs;
using Arcora.Api.Models;

namespace Arcora.Api.Services.Interfaces
{
    public interface ISecurityDepositTransactionService
    {
        /// <summary>
        /// Retrieves all security deposit transactions with optional paging support.
        /// </summary>
        /// <param name = "paging"></param>
        /// <returns></returns>
        Task<PagedResult<SecurityDepositTransactionDto>> GetAll(Paging paging);
        /// <summary>
        /// Retrieves a security deposit transaction by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task<SecurityDepositTransactionDto?> GetID(Guid ID);
        /// <summary>
        /// Creates a new security deposit transaction entry.
        /// </summary>
        /// <param name = "securityDepositTransactionDto"></param>
        /// <returns></returns>
        Task<SecurityDepositTransactionDto> CreateSecurityDepositTransaction(SecurityDepositTransactionDto securityDepositTransactionDto);
        /// <summary>
        /// Updates an existing security deposit transaction entry by its ID.
        /// </summary>
        /// <param name = "id"></param>
        /// <param name = "securitydeposittransactionDto"></param>
        /// <returns></returns>
        Task<SecurityDepositTransactionDto?> UpdateSecurityDepositTransaction(Guid id, SecurityDepositTransactionDto securitydeposittransactionDto);
        /// <summary>
        /// Deletes a securitydeposittransaction entry by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task DeleteSecurityDepositTransaction(Guid ID);
    }
}