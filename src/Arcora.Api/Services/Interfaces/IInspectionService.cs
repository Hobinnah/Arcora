// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.DTOs;
using Arcora.Api.Models;

namespace Arcora.Api.Services.Interfaces
{
    public interface IInspectionService
    {
        /// <summary>
        /// Retrieves all inspections with optional paging support.
        /// </summary>
        /// <param name = "paging"></param>
        /// <returns></returns>
        Task<PagedResult<InspectionDto>> GetAll(Paging paging);
        /// <summary>
        /// Retrieves a inspection by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task<InspectionDto?> GetID(Guid ID);
        /// <summary>
        /// Creates a new inspection entry.
        /// </summary>
        /// <param name = "inspectionDto"></param>
        /// <returns></returns>
        Task<InspectionDto> CreateInspection(InspectionDto inspectionDto);
        /// <summary>
        /// Updates an existing inspection entry by its ID.
        /// </summary>
        /// <param name = "id"></param>
        /// <param name = "inspectionDto"></param>
        /// <returns></returns>
        Task<InspectionDto?> UpdateInspection(Guid id, InspectionDto inspectionDto);
        /// <summary>
        /// Deletes a inspection entry by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task DeleteInspection(Guid ID);
        /// <summary>
        /// Updates the status of a inspection by its ID.
        /// </summary>
        /// <param name = "id">The unique identifier of the inspection to update.</param>
        /// <param name = "status">The new status value to assign to the inspection.</param>
        /// <returns>
        /// Returns <see cref = "InspectionDto"/> with the updated inspection if successful, or null if not found.
        /// </returns>
        Task<InspectionDto?> UpdateInspectionStatus(Guid id, string status);
    }
}