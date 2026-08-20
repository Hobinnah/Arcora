// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.DTOs;
using Arcora.Api.Models;

namespace Arcora.Api.Services.Interfaces
{
    public interface IReservationHoldService
    {
        /// <summary>
        /// Retrieves all reservation holds with optional paging support.
        /// </summary>
        /// <param name = "paging"></param>
        /// <returns></returns>
        Task<PagedResult<ReservationHoldDto>> GetAll(Paging paging);
        /// <summary>
        /// Retrieves a reservation hold by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task<ReservationHoldDto?> GetID(Guid ID);
        /// <summary>
        /// Creates a new reservation hold entry.
        /// </summary>
        /// <param name = "reservationHoldDto"></param>
        /// <returns></returns>
        Task<ReservationHoldDto> CreateReservationHold(ReservationHoldDto reservationHoldDto);
        /// <summary>
        /// Updates an existing reservation hold entry by its ID.
        /// </summary>
        /// <param name = "id"></param>
        /// <param name = "reservationholdDto"></param>
        /// <returns></returns>
        Task<ReservationHoldDto?> UpdateReservationHold(Guid id, ReservationHoldDto reservationholdDto);
        /// <summary>
        /// Deletes a reservationhold entry by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task DeleteReservationHold(Guid ID);
        /// <summary>
        /// Updates the status of a reservationhold by its ID.
        /// </summary>
        /// <param name = "id">The unique identifier of the reservationhold to update.</param>
        /// <param name = "status">The new status value to assign to the reservationhold.</param>
        /// <returns>
        /// Returns <see cref = "ReservationHoldDto"/> with the updated reservationhold if successful, or null if not found.
        /// </returns>
        Task<ReservationHoldDto?> UpdateReservationHoldStatus(Guid id, string status);
    }
}