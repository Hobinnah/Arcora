// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.DTOs;
using Arcora.Api.Models;

namespace Arcora.Api.Services.Interfaces
{
    public interface ILeaseService
    {
        /// <summary>
        /// Retrieves all leases with optional paging support.
        /// </summary>
        /// <param name = "paging"></param>
        /// <returns></returns>
        Task<PagedResult<LeaseDto>> GetAll(Paging paging);
        /// <summary>
        /// Retrieves a lease by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task<LeaseDto?> GetID(Guid ID);
        /// <summary>
        /// Creates a new lease entry.
        /// </summary>
        /// <param name = "leaseDto"></param>
        /// <returns></returns>
        Task<LeaseDto> CreateLease(LeaseDto leaseDto);
        /// <summary>
        /// Updates an existing lease entry by its ID.
        /// </summary>
        /// <param name = "id"></param>
        /// <param name = "leaseDto"></param>
        /// <returns></returns>
        Task<LeaseDto?> UpdateLease(Guid id, LeaseDto leaseDto);
        /// <summary>
        /// Deletes a lease entry by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task DeleteLease(Guid ID);
        /// <summary>
        /// Updates the status of a lease by its ID.
        /// </summary>
        /// <param name = "id">The unique identifier of the lease to update.</param>
        /// <param name = "status">The new status value to assign to the lease.</param>
        /// <returns>
        /// Returns <see cref = "LeaseDto"/> with the updated lease if successful, or null if not found.
        /// </returns>
        Task<LeaseDto?> UpdateLeaseStatus(Guid id, string status);
    }
}