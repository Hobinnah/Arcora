// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.DTOs;
using Arcora.Api.Models;

namespace Arcora.Api.Services.Interfaces
{
    public interface ILeaseDocumentsService
    {
        /// <summary>
        /// Retrieves all lease documents with optional paging support.
        /// </summary>
        /// <param name = "paging"></param>
        /// <returns></returns>
        Task<PagedResult<LeaseDocumentsDto>> GetAll(Paging paging);
        /// <summary>
        /// Retrieves a lease documents by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task<LeaseDocumentsDto?> GetID(Guid ID);
        /// <summary>
        /// Creates a new lease documents entry.
        /// </summary>
        /// <param name = "leaseDocumentsDto"></param>
        /// <returns></returns>
        Task<LeaseDocumentsDto> CreateLeaseDocuments(LeaseDocumentsDto leaseDocumentsDto);
        /// <summary>
        /// Updates an existing lease documents entry by its ID.
        /// </summary>
        /// <param name = "id"></param>
        /// <param name = "leasedocumentsDto"></param>
        /// <returns></returns>
        Task<LeaseDocumentsDto?> UpdateLeaseDocuments(Guid id, LeaseDocumentsDto leasedocumentsDto);
        /// <summary>
        /// Deletes a leasedocuments entry by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task DeleteLeaseDocuments(Guid ID);
    }
}