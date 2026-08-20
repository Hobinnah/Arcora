// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.DTOs;
using Arcora.Api.Models;

namespace Arcora.Api.Services.Interfaces
{
    public interface IViewingAppointmentsService
    {
        /// <summary>
        /// Retrieves all viewing appointments with optional paging support.
        /// </summary>
        /// <param name = "paging"></param>
        /// <returns></returns>
        Task<PagedResult<ViewingAppointmentsDto>> GetAll(Paging paging);
        /// <summary>
        /// Retrieves a viewing appointments by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task<ViewingAppointmentsDto?> GetID(Guid ID);
        /// <summary>
        /// Creates a new viewing appointments entry.
        /// </summary>
        /// <param name = "viewingAppointmentsDto"></param>
        /// <returns></returns>
        Task<ViewingAppointmentsDto> CreateViewingAppointments(ViewingAppointmentsDto viewingAppointmentsDto);
        /// <summary>
        /// Updates an existing viewing appointments entry by its ID.
        /// </summary>
        /// <param name = "id"></param>
        /// <param name = "viewingappointmentsDto"></param>
        /// <returns></returns>
        Task<ViewingAppointmentsDto?> UpdateViewingAppointments(Guid id, ViewingAppointmentsDto viewingappointmentsDto);
        /// <summary>
        /// Deletes a viewingappointments entry by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task DeleteViewingAppointments(Guid ID);
        /// <summary>
        /// Updates the status of a viewingappointments by its ID.
        /// </summary>
        /// <param name = "id">The unique identifier of the viewingappointments to update.</param>
        /// <param name = "status">The new status value to assign to the viewingappointments.</param>
        /// <returns>
        /// Returns <see cref = "ViewingAppointmentsDto"/> with the updated viewingappointments if successful, or null if not found.
        /// </returns>
        Task<ViewingAppointmentsDto?> UpdateViewingAppointmentsStatus(Guid id, string status);
    }
}