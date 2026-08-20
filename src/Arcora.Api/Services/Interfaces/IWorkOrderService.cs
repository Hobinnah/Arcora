// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.DTOs;
using Arcora.Api.Models;

namespace Arcora.Api.Services.Interfaces
{
    public interface IWorkOrderService
    {
        /// <summary>
        /// Retrieves all work orders with optional paging support.
        /// </summary>
        /// <param name = "paging"></param>
        /// <returns></returns>
        Task<PagedResult<WorkOrderDto>> GetAll(Paging paging);
        /// <summary>
        /// Retrieves a work order by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task<WorkOrderDto?> GetID(Guid ID);
        /// <summary>
        /// Creates a new work order entry.
        /// </summary>
        /// <param name = "workOrderDto"></param>
        /// <returns></returns>
        Task<WorkOrderDto> CreateWorkOrder(WorkOrderDto workOrderDto);
        /// <summary>
        /// Updates an existing work order entry by its ID.
        /// </summary>
        /// <param name = "id"></param>
        /// <param name = "workorderDto"></param>
        /// <returns></returns>
        Task<WorkOrderDto?> UpdateWorkOrder(Guid id, WorkOrderDto workorderDto);
        /// <summary>
        /// Deletes a workorder entry by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task DeleteWorkOrder(Guid ID);
        /// <summary>
        /// Updates the status of a workorder by its ID.
        /// </summary>
        /// <param name = "id">The unique identifier of the workorder to update.</param>
        /// <param name = "status">The new status value to assign to the workorder.</param>
        /// <returns>
        /// Returns <see cref = "WorkOrderDto"/> with the updated workorder if successful, or null if not found.
        /// </returns>
        Task<WorkOrderDto?> UpdateWorkOrderStatus(Guid id, string status);
    }
}