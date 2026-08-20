// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.DTOs;
using Arcora.Api.Models;

namespace Arcora.Api.Services.Interfaces
{
    public interface ILeaseSignatoriesService
    {
        /// <summary>
        /// Retrieves all lease signatories with optional paging support.
        /// </summary>
        /// <param name = "paging"></param>
        /// <returns></returns>
        Task<PagedResult<LeaseSignatoriesDto>> GetAll(Paging paging);
        /// <summary>
        /// Retrieves a lease signatories by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task<LeaseSignatoriesDto?> GetID(Guid ID);
        /// <summary>
        /// Creates a new lease signatories entry.
        /// </summary>
        /// <param name = "leaseSignatoriesDto"></param>
        /// <returns></returns>
        Task<LeaseSignatoriesDto> CreateLeaseSignatories(LeaseSignatoriesDto leaseSignatoriesDto);
        /// <summary>
        /// Updates an existing lease signatories entry by its ID.
        /// </summary>
        /// <param name = "id"></param>
        /// <param name = "leasesignatoriesDto"></param>
        /// <returns></returns>
        Task<LeaseSignatoriesDto?> UpdateLeaseSignatories(Guid id, LeaseSignatoriesDto leasesignatoriesDto);
        /// <summary>
        /// Deletes a leasesignatories entry by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task DeleteLeaseSignatories(Guid ID);
        /// <summary>
        /// Updates the status of a leasesignatories by its ID.
        /// </summary>
        /// <param name = "id">The unique identifier of the leasesignatories to update.</param>
        /// <param name = "status">The new status value to assign to the leasesignatories.</param>
        /// <returns>
        /// Returns <see cref = "LeaseSignatoriesDto"/> with the updated leasesignatories if successful, or null if not found.
        /// </returns>
        Task<LeaseSignatoriesDto?> UpdateLeaseSignatoriesStatus(Guid id, string status);
    }
}