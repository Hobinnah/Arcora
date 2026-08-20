// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.DTOs;
using Arcora.Api.Models;

namespace Arcora.Api.Services.Interfaces
{
    public interface ILeaseRecurringChargesService
    {
        /// <summary>
        /// Retrieves all lease recurring charges with optional paging support.
        /// </summary>
        /// <param name = "paging"></param>
        /// <returns></returns>
        Task<PagedResult<LeaseRecurringChargesDto>> GetAll(Paging paging);
        /// <summary>
        /// Retrieves a lease recurring charges by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task<LeaseRecurringChargesDto?> GetID(Guid ID);
        /// <summary>
        /// Creates a new lease recurring charges entry.
        /// </summary>
        /// <param name = "leaseRecurringChargesDto"></param>
        /// <returns></returns>
        Task<LeaseRecurringChargesDto> CreateLeaseRecurringCharges(LeaseRecurringChargesDto leaseRecurringChargesDto);
        /// <summary>
        /// Updates an existing lease recurring charges entry by its ID.
        /// </summary>
        /// <param name = "id"></param>
        /// <param name = "leaserecurringchargesDto"></param>
        /// <returns></returns>
        Task<LeaseRecurringChargesDto?> UpdateLeaseRecurringCharges(Guid id, LeaseRecurringChargesDto leaserecurringchargesDto);
        /// <summary>
        /// Deletes a leaserecurringcharges entry by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task DeleteLeaseRecurringCharges(Guid ID);
    }
}