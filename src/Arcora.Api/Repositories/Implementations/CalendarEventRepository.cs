// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Repositories.Implementations
{
    public class CalendarEventRepository : Repository<CalendarEvent>, ICalendarEventRepository //, IDisposable
    {
        private readonly ArcoraDbContext context;
        public CalendarEventRepository(ArcoraDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<CalendarEvent>> GetCalendarEventAsync()
        {
            return await ApplyDefaultOrder(this.context.CalendarEvents.AsNoTracking().Include(x => x.Listing).Include(x => x.Lease).Include(x => x.RentalApplication).Include(x => x.ReservationHold).Include(x => x.MaintenanceRequest)).ToListAsync();
        }

        public async Task<bool> HasCalendarEventsAsync()
        {
            return await this.context.Set<CalendarEvent>().AnyAsync();
        }
    }
}