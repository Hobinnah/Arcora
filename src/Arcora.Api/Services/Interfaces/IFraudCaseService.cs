// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.DTOs;
using Arcora.Api.Models;

namespace Arcora.Api.Services.Interfaces
{
    public interface IFraudCaseService
    {
        /// <summary>
        /// Retrieves all fraud cases with optional paging support.
        /// </summary>
        /// <param name = "paging"></param>
        /// <returns></returns>
        Task<PagedResult<FraudCaseDto>> GetAll(Paging paging);
        /// <summary>
        /// Retrieves a fraud case by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task<FraudCaseDto?> GetID(Guid ID);
        /// <summary>
        /// Creates a new fraud case entry.
        /// </summary>
        /// <param name = "fraudCaseDto"></param>
        /// <returns></returns>
        Task<FraudCaseDto> CreateFraudCase(FraudCaseDto fraudCaseDto);
        /// <summary>
        /// Updates an existing fraud case entry by its ID.
        /// </summary>
        /// <param name = "id"></param>
        /// <param name = "fraudcaseDto"></param>
        /// <returns></returns>
        Task<FraudCaseDto?> UpdateFraudCase(Guid id, FraudCaseDto fraudcaseDto);
        /// <summary>
        /// Deletes a fraudcase entry by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task DeleteFraudCase(Guid ID);
        /// <summary>
        /// Updates the status of a fraudcase by its ID.
        /// </summary>
        /// <param name = "id">The unique identifier of the fraudcase to update.</param>
        /// <param name = "status">The new status value to assign to the fraudcase.</param>
        /// <returns>
        /// Returns <see cref = "FraudCaseDto"/> with the updated fraudcase if successful, or null if not found.
        /// </returns>
        Task<FraudCaseDto?> UpdateFraudCaseStatus(Guid id, string status);
    }
}