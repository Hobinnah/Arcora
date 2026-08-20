// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.DTOs;
using Arcora.Api.Models;

namespace Arcora.Api.Services.Interfaces
{
    public interface ILeaseDocExtractedTermService
    {
        /// <summary>
        /// Retrieves all lease doc extracted terms with optional paging support.
        /// </summary>
        /// <param name = "paging"></param>
        /// <returns></returns>
        Task<PagedResult<LeaseDocExtractedTermDto>> GetAll(Paging paging);
        /// <summary>
        /// Retrieves a lease doc extracted term by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task<LeaseDocExtractedTermDto?> GetID(Guid ID);
        /// <summary>
        /// Creates a new lease doc extracted term entry.
        /// </summary>
        /// <param name = "leaseDocExtractedTermDto"></param>
        /// <returns></returns>
        Task<LeaseDocExtractedTermDto> CreateLeaseDocExtractedTerm(LeaseDocExtractedTermDto leaseDocExtractedTermDto);
        /// <summary>
        /// Updates an existing lease doc extracted term entry by its ID.
        /// </summary>
        /// <param name = "id"></param>
        /// <param name = "leasedocextractedtermDto"></param>
        /// <returns></returns>
        Task<LeaseDocExtractedTermDto?> UpdateLeaseDocExtractedTerm(Guid id, LeaseDocExtractedTermDto leasedocextractedtermDto);
        /// <summary>
        /// Deletes a leasedocextractedterm entry by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task DeleteLeaseDocExtractedTerm(Guid ID);
    }
}