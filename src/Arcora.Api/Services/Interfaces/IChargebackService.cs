// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.DTOs;
using Arcora.Api.Models;

namespace Arcora.Api.Services.Interfaces
{
    public interface IChargebackService
    {
        /// <summary>
        /// Retrieves all chargebacks with optional paging support.
        /// </summary>
        /// <param name = "paging"></param>
        /// <returns></returns>
        Task<PagedResult<ChargebackDto>> GetAll(Paging paging);
        /// <summary>
        /// Retrieves a chargeback by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task<ChargebackDto?> GetID(Guid ID);
        /// <summary>
        /// Creates a new chargeback entry.
        /// </summary>
        /// <param name = "chargebackDto"></param>
        /// <returns></returns>
        Task<ChargebackDto> CreateChargeback(ChargebackDto chargebackDto);
        /// <summary>
        /// Updates an existing chargeback entry by its ID.
        /// </summary>
        /// <param name = "id"></param>
        /// <param name = "chargebackDto"></param>
        /// <returns></returns>
        Task<ChargebackDto?> UpdateChargeback(Guid id, ChargebackDto chargebackDto);
        /// <summary>
        /// Deletes a chargeback entry by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task DeleteChargeback(Guid ID);
        /// <summary>
        /// Updates the status of a chargeback by its ID.
        /// </summary>
        /// <param name = "id">The unique identifier of the chargeback to update.</param>
        /// <param name = "status">The new status value to assign to the chargeback.</param>
        /// <returns>
        /// Returns <see cref = "ChargebackDto"/> with the updated chargeback if successful, or null if not found.
        /// </returns>
        Task<ChargebackDto?> UpdateChargebackStatus(Guid id, string status);
    }
}