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
        Task<PagedResult<RentalApplicationDto>> GetAllForActor(Paging paging, long actorUserID, bool isAdmin);
        /// <summary>
        /// Retrieves a rental application by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task<RentalApplicationDto?> GetID(Guid ID);
        Task<RentalApplicationDto?> GetIDForActor(Guid ID, long actorUserID, bool isAdmin);
        /// <summary>
        /// Creates a new rental application entry.
        /// </summary>
        /// <param name = "rentalApplicationDto"></param>
        /// <returns></returns>
        Task<RentalApplicationDto> CreateRentalApplication(RentalApplicationDto rentalApplicationDto);
        Task<RentalApplicationDto> CreateRentalApplicationForActor(RentalApplicationDto rentalApplicationDto, long actorUserID, bool isAdmin);
        /// <summary>
        /// Updates an existing rental application entry by its ID.
        /// </summary>
        /// <param name = "id"></param>
        /// <param name = "rentalapplicationDto"></param>
        /// <returns></returns>
        Task<RentalApplicationDto?> UpdateRentalApplication(Guid id, RentalApplicationDto rentalapplicationDto);
        Task<RentalApplicationDto?> UpdateRentalApplicationForActor(Guid id, RentalApplicationDto rentalapplicationDto, long actorUserID, bool isAdmin);
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
        Task<RentalApplicationDto?> UpdateRentalApplicationStatusForActor(Guid id, string status, long actorUserID, bool isAdmin);

        /// <summary>
        /// Returns the full set of rental application stages (status values) so the frontend can stay
        /// in sync with the backend enum.
        /// </summary>
        /// <returns>An ordered collection of <see cref="RentalApplicationStageDto"/>.</returns>
        IReadOnlyList<RentalApplicationStageDto> GetApplicationStages();
    }
}