// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.DTOs;
using Arcora.Api.Models;

namespace Arcora.Api.Services.Interfaces
{
    public interface IContractorService
    {
        /// <summary>
        /// Retrieves all contractors with optional paging support.
        /// </summary>
        /// <param name = "paging"></param>
        /// <returns></returns>
        Task<PagedResult<ContractorDto>> GetAll(Paging paging);
        /// <summary>
        /// Retrieves a contractor by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task<ContractorDto?> GetID(Guid ID);
        /// <summary>
        /// Creates a new contractor entry.
        /// </summary>
        /// <param name = "contractorDto"></param>
        /// <returns></returns>
        Task<ContractorDto> CreateContractor(ContractorDto contractorDto);
        /// <summary>
        /// Updates an existing contractor entry by its ID.
        /// </summary>
        /// <param name = "id"></param>
        /// <param name = "contractorDto"></param>
        /// <returns></returns>
        Task<ContractorDto?> UpdateContractor(Guid id, ContractorDto contractorDto);
        /// <summary>
        /// Deletes a contractor entry by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task DeleteContractor(Guid ID);
        /// <summary>
        /// Updates the status of a contractor by its ID.
        /// </summary>
        /// <param name = "id">The unique identifier of the contractor to update.</param>
        /// <param name = "status">The new status value to assign to the contractor.</param>
        /// <returns>
        /// Returns <see cref = "ContractorDto"/> with the updated contractor if successful, or null if not found.
        /// </returns>
        Task<ContractorDto?> UpdateContractorStatus(Guid id, string status);
    }
}