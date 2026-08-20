// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.DTOs;
using Arcora.Api.Models;

namespace Arcora.Api.Services.Interfaces
{
    public interface ICalendarEventService
    {
        /// <summary>
        /// Retrieves all calendar events with optional paging support.
        /// </summary>
        /// <param name = "paging"></param>
        /// <returns></returns>
        Task<PagedResult<CalendarEventDto>> GetAll(Paging paging);
        /// <summary>
        /// Retrieves a calendar event by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task<CalendarEventDto?> GetID(Guid ID);
        /// <summary>
        /// Creates a new calendar event entry.
        /// </summary>
        /// <param name = "calendarEventDto"></param>
        /// <returns></returns>
        Task<CalendarEventDto> CreateCalendarEvent(CalendarEventDto calendarEventDto);
        /// <summary>
        /// Updates an existing calendar event entry by its ID.
        /// </summary>
        /// <param name = "id"></param>
        /// <param name = "calendareventDto"></param>
        /// <returns></returns>
        Task<CalendarEventDto?> UpdateCalendarEvent(Guid id, CalendarEventDto calendareventDto);
        /// <summary>
        /// Deletes a calendarevent entry by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task DeleteCalendarEvent(Guid ID);
        /// <summary>
        /// Updates the status of a calendarevent by its ID.
        /// </summary>
        /// <param name = "id">The unique identifier of the calendarevent to update.</param>
        /// <param name = "status">The new status value to assign to the calendarevent.</param>
        /// <returns>
        /// Returns <see cref = "CalendarEventDto"/> with the updated calendarevent if successful, or null if not found.
        /// </returns>
        Task<CalendarEventDto?> UpdateCalendarEventStatus(Guid id, string status);
    }
}