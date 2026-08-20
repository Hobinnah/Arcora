// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.DTOs;
using Arcora.Api.Models;

namespace Arcora.Api.Services.Interfaces
{
    public interface IApplicationOccupantService
    {
        /// <summary>
        /// Retrieves all application occupants with optional paging support.
        /// </summary>
        /// <param name = "paging"></param>
        /// <returns></returns>
        Task<PagedResult<ApplicationOccupantDto>> GetAll(Paging paging);
        /// <summary>
        /// Retrieves a application occupant by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task<ApplicationOccupantDto?> GetID(Guid ID);
        /// <summary>
        /// Creates a new application occupant entry.
        /// </summary>
        /// <param name = "applicationOccupantDto"></param>
        /// <returns></returns>
        Task<ApplicationOccupantDto> CreateApplicationOccupant(ApplicationOccupantDto applicationOccupantDto);
        /// <summary>
        /// Updates an existing application occupant entry by its ID.
        /// </summary>
        /// <param name = "id"></param>
        /// <param name = "applicationoccupantDto"></param>
        /// <returns></returns>
        Task<ApplicationOccupantDto?> UpdateApplicationOccupant(Guid id, ApplicationOccupantDto applicationoccupantDto);
        /// <summary>
        /// Deletes a applicationoccupant entry by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task DeleteApplicationOccupant(Guid ID);
        /// <summary>
        /// Updates the status of a applicationoccupant by its ID.
        /// </summary>
        /// <param name = "id">The unique identifier of the applicationoccupant to update.</param>
        /// <param name = "status">The new status value to assign to the applicationoccupant.</param>
        /// <returns>
        /// Returns <see cref = "ApplicationOccupantDto"/> with the updated applicationoccupant if successful, or null if not found.
        /// </returns>
        Task<ApplicationOccupantDto?> UpdateApplicationOccupantStatus(Guid id, string status);
    }
}