// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.DTOs;
using Arcora.Api.Models;

namespace Arcora.Api.Services.Interfaces
{
    public interface IIdentityVerificationService
    {
        /// <summary>
        /// Retrieves all identity verifications with optional paging support.
        /// </summary>
        /// <param name = "paging"></param>
        /// <returns></returns>
        Task<PagedResult<IdentityVerificationDto>> GetAll(Paging paging);
        /// <summary>
        /// Retrieves a identity verification by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task<IdentityVerificationDto?> GetID(Guid ID);
        /// <summary>
        /// Creates a new identity verification entry.
        /// </summary>
        /// <param name = "identityVerificationDto"></param>
        /// <returns></returns>
        Task<IdentityVerificationDto> CreateIdentityVerification(IdentityVerificationDto identityVerificationDto);
        /// <summary>
        /// Updates an existing identity verification entry by its ID.
        /// </summary>
        /// <param name = "id"></param>
        /// <param name = "identityverificationDto"></param>
        /// <returns></returns>
        Task<IdentityVerificationDto?> UpdateIdentityVerification(Guid id, IdentityVerificationDto identityverificationDto);
        /// <summary>
        /// Deletes a identityverification entry by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task DeleteIdentityVerification(Guid ID);
        /// <summary>
        /// Updates the status of a identityverification by its ID.
        /// </summary>
        /// <param name = "id">The unique identifier of the identityverification to update.</param>
        /// <param name = "status">The new status value to assign to the identityverification.</param>
        /// <returns>
        /// Returns <see cref = "IdentityVerificationDto"/> with the updated identityverification if successful, or null if not found.
        /// </returns>
        Task<IdentityVerificationDto?> UpdateIdentityVerificationStatus(Guid id, string status);
    }
}