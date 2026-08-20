// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.DTOs;
using Arcora.Api.Models;

namespace Arcora.Api.Services.Interfaces
{
    public interface ILeaseRenewalsService
    {
        /// <summary>
        /// Retrieves all lease renewals with optional paging support.
        /// </summary>
        /// <param name = "paging"></param>
        /// <returns></returns>
        Task<PagedResult<LeaseRenewalsDto>> GetAll(Paging paging);
        /// <summary>
        /// Retrieves a lease renewals by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task<LeaseRenewalsDto?> GetID(Guid ID);
        /// <summary>
        /// Creates a new lease renewals entry.
        /// </summary>
        /// <param name = "leaseRenewalsDto"></param>
        /// <returns></returns>
        Task<LeaseRenewalsDto> CreateLeaseRenewals(LeaseRenewalsDto leaseRenewalsDto);
        /// <summary>
        /// Updates an existing lease renewals entry by its ID.
        /// </summary>
        /// <param name = "id"></param>
        /// <param name = "leaserenewalsDto"></param>
        /// <returns></returns>
        Task<LeaseRenewalsDto?> UpdateLeaseRenewals(Guid id, LeaseRenewalsDto leaserenewalsDto);
        /// <summary>
        /// Deletes a leaserenewals entry by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task DeleteLeaseRenewals(Guid ID);
        /// <summary>
        /// Updates the status of a leaserenewals by its ID.
        /// </summary>
        /// <param name = "id">The unique identifier of the leaserenewals to update.</param>
        /// <param name = "status">The new status value to assign to the leaserenewals.</param>
        /// <returns>
        /// Returns <see cref = "LeaseRenewalsDto"/> with the updated leaserenewals if successful, or null if not found.
        /// </returns>
        Task<LeaseRenewalsDto?> UpdateLeaseRenewalsStatus(Guid id, string status);
    }
}