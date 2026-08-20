// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.DTOs;
using Arcora.Api.Models;

namespace Arcora.Api.Services.Interfaces
{
    public interface IRentalUnitService
    {
        /// <summary>
        /// Retrieves all rental units with optional paging support.
        /// </summary>
        /// <param name = "paging"></param>
        /// <returns></returns>
        Task<PagedResult<RentalUnitDto>> GetAll(Paging paging);
        /// <summary>
        /// Retrieves a rental unit by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task<RentalUnitDto?> GetID(Guid ID);
        /// <summary>
        /// Creates a new rental unit entry.
        /// </summary>
        /// <param name = "rentalUnitDto"></param>
        /// <returns></returns>
        Task<RentalUnitDto> CreateRentalUnit(RentalUnitDto rentalUnitDto);
        /// <summary>
        /// Updates an existing rental unit entry by its ID.
        /// </summary>
        /// <param name = "id"></param>
        /// <param name = "rentalunitDto"></param>
        /// <returns></returns>
        Task<RentalUnitDto?> UpdateRentalUnit(Guid id, RentalUnitDto rentalunitDto);
        /// <summary>
        /// Deletes a rentalunit entry by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task DeleteRentalUnit(Guid ID);
        /// <summary>
        /// Updates the status of a rentalunit by its ID.
        /// </summary>
        /// <param name = "id">The unique identifier of the rentalunit to update.</param>
        /// <param name = "status">The new status value to assign to the rentalunit.</param>
        /// <returns>
        /// Returns <see cref = "RentalUnitDto"/> with the updated rentalunit if successful, or null if not found.
        /// </returns>
        Task<RentalUnitDto?> UpdateRentalUnitStatus(Guid id, string status);
    }
}