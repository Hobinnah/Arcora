// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.DTOs;
using Arcora.Api.Models;

namespace Arcora.Api.Services.Interfaces
{
    public interface ISecurityDepositService
    {
        /// <summary>
        /// Retrieves all security deposits with optional paging support.
        /// </summary>
        /// <param name = "paging"></param>
        /// <returns></returns>
        Task<PagedResult<SecurityDepositDto>> GetAll(Paging paging);
        /// <summary>
        /// Retrieves a security deposit by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task<SecurityDepositDto?> GetID(Guid ID);
        /// <summary>
        /// Creates a new security deposit entry.
        /// </summary>
        /// <param name = "securityDepositDto"></param>
        /// <returns></returns>
        Task<SecurityDepositDto> CreateSecurityDeposit(SecurityDepositDto securityDepositDto);
        /// <summary>
        /// Updates an existing security deposit entry by its ID.
        /// </summary>
        /// <param name = "id"></param>
        /// <param name = "securitydepositDto"></param>
        /// <returns></returns>
        Task<SecurityDepositDto?> UpdateSecurityDeposit(Guid id, SecurityDepositDto securitydepositDto);
        /// <summary>
        /// Deletes a securitydeposit entry by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task DeleteSecurityDeposit(Guid ID);
        /// <summary>
        /// Updates the status of a securitydeposit by its ID.
        /// </summary>
        /// <param name = "id">The unique identifier of the securitydeposit to update.</param>
        /// <param name = "status">The new status value to assign to the securitydeposit.</param>
        /// <returns>
        /// Returns <see cref = "SecurityDepositDto"/> with the updated securitydeposit if successful, or null if not found.
        /// </returns>
        Task<SecurityDepositDto?> UpdateSecurityDepositStatus(Guid id, string status);
    }
}