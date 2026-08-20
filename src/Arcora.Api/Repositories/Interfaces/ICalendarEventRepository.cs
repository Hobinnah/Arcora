// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;

namespace Arcora.Api.Repositories.Interfaces
{
    public interface ICalendarEventRepository : IRepository<CalendarEvent>
    {
        Task<List<CalendarEvent>> GetCalendarEventAsync();
        Task<bool> HasCalendarEventsAsync();
    }
}