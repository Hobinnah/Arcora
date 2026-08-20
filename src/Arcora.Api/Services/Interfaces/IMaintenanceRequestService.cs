// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.DTOs;
using Arcora.Api.Models;

namespace Arcora.Api.Services.Interfaces
{
    public interface IMaintenanceRequestService
    {
        /// <summary>
        /// Retrieves all maintenance requests with optional paging support.
        /// </summary>
        /// <param name = "paging"></param>
        /// <returns></returns>
        Task<PagedResult<MaintenanceRequestDto>> GetAll(Paging paging);
        /// <summary>
        /// Retrieves a maintenance request by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task<MaintenanceRequestDto?> GetID(Guid ID);
        /// <summary>
        /// Creates a new maintenance request entry.
        /// </summary>
        /// <param name = "maintenanceRequestDto"></param>
        /// <returns></returns>
        Task<MaintenanceRequestDto> CreateMaintenanceRequest(MaintenanceRequestDto maintenanceRequestDto);
        /// <summary>
        /// Updates an existing maintenance request entry by its ID.
        /// </summary>
        /// <param name = "id"></param>
        /// <param name = "maintenancerequestDto"></param>
        /// <returns></returns>
        Task<MaintenanceRequestDto?> UpdateMaintenanceRequest(Guid id, MaintenanceRequestDto maintenancerequestDto);
        /// <summary>
        /// Deletes a maintenancerequest entry by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task DeleteMaintenanceRequest(Guid ID);
        /// <summary>
        /// Updates the status of a maintenancerequest by its ID.
        /// </summary>
        /// <param name = "id">The unique identifier of the maintenancerequest to update.</param>
        /// <param name = "status">The new status value to assign to the maintenancerequest.</param>
        /// <returns>
        /// Returns <see cref = "MaintenanceRequestDto"/> with the updated maintenancerequest if successful, or null if not found.
        /// </returns>
        Task<MaintenanceRequestDto?> UpdateMaintenanceRequestStatus(Guid id, string status);
    }
}