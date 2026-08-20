// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.DTOs;
using Arcora.Api.Models;

namespace Arcora.Api.Services.Interfaces
{
    public interface ILeaseOccupantsService
    {
        /// <summary>
        /// Retrieves all lease occupants with optional paging support.
        /// </summary>
        /// <param name = "paging"></param>
        /// <returns></returns>
        Task<PagedResult<LeaseOccupantsDto>> GetAll(Paging paging);
        /// <summary>
        /// Retrieves a lease occupants by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task<LeaseOccupantsDto?> GetID(Guid ID);
        /// <summary>
        /// Creates a new lease occupants entry.
        /// </summary>
        /// <param name = "leaseOccupantsDto"></param>
        /// <returns></returns>
        Task<LeaseOccupantsDto> CreateLeaseOccupants(LeaseOccupantsDto leaseOccupantsDto);
        /// <summary>
        /// Updates an existing lease occupants entry by its ID.
        /// </summary>
        /// <param name = "id"></param>
        /// <param name = "leaseoccupantsDto"></param>
        /// <returns></returns>
        Task<LeaseOccupantsDto?> UpdateLeaseOccupants(Guid id, LeaseOccupantsDto leaseoccupantsDto);
        /// <summary>
        /// Deletes a leaseoccupants entry by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task DeleteLeaseOccupants(Guid ID);
        /// <summary>
        /// Updates the status of a leaseoccupants by its ID.
        /// </summary>
        /// <param name = "id">The unique identifier of the leaseoccupants to update.</param>
        /// <param name = "status">The new status value to assign to the leaseoccupants.</param>
        /// <returns>
        /// Returns <see cref = "LeaseOccupantsDto"/> with the updated leaseoccupants if successful, or null if not found.
        /// </returns>
        Task<LeaseOccupantsDto?> UpdateLeaseOccupantsStatus(Guid id, string status);
    }
}