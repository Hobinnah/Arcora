// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.DTOs;
using Arcora.Api.Models;

namespace Arcora.Api.Services.Interfaces
{
    public interface IRentalApplicationService
    {
        /// <summary>
        /// Retrieves all rental applications with optional paging support.
        /// </summary>
        /// <param name = "paging"></param>
        /// <returns></returns>
        Task<PagedResult<RentalApplicationDto>> GetAll(Paging paging);
        /// <summary>
        /// Retrieves a rental application by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task<RentalApplicationDto?> GetID(Guid ID);
        /// <summary>
        /// Creates a new rental application entry.
        /// </summary>
        /// <param name = "rentalApplicationDto"></param>
        /// <returns></returns>
        Task<RentalApplicationDto> CreateRentalApplication(RentalApplicationDto rentalApplicationDto);
        /// <summary>
        /// Updates an existing rental application entry by its ID.
        /// </summary>
        /// <param name = "id"></param>
        /// <param name = "rentalapplicationDto"></param>
        /// <returns></returns>
        Task<RentalApplicationDto?> UpdateRentalApplication(Guid id, RentalApplicationDto rentalapplicationDto);
        /// <summary>
        /// Deletes a rentalapplication entry by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task DeleteRentalApplication(Guid ID);
        /// <summary>
        /// Updates the status of a rentalapplication by its ID.
        /// </summary>
        /// <param name = "id">The unique identifier of the rentalapplication to update.</param>
        /// <param name = "status">The new status value to assign to the rentalapplication.</param>
        /// <returns>
        /// Returns <see cref = "RentalApplicationDto"/> with the updated rentalapplication if successful, or null if not found.
        /// </returns>
        Task<RentalApplicationDto?> UpdateRentalApplicationStatus(Guid id, string status);

        /// <summary>
        /// Returns the full set of rental application stages (status values) so the frontend can stay
        /// in sync with the backend enum.
        /// </summary>
        /// <returns>An ordered collection of <see cref="RentalApplicationStageDto"/>.</returns>
        IReadOnlyList<RentalApplicationStageDto> GetApplicationStages();
    }
}