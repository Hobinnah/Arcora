// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Repositories.Implementations
{
    public class ViewingAppointmentsRepository : Repository<ViewingAppointments>, IViewingAppointmentsRepository //, IDisposable
    {
        private readonly ArcoraDbContext context;
        public ViewingAppointmentsRepository(ArcoraDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<ViewingAppointments>> GetViewingAppointmentsAsync()
        {
            return await this.context.ViewingAppointments.AsNoTracking().Include(x => x.Listing).Include(x => x.RequestedByUser) // FK to User who requested the appointment
            .Include(x => x.Tenant).Include(x => x.AssignedOrganizationMember) // FK to OrganizationMember assigned to the appointment
            .ToListAsync();
        }

        public async Task<bool> HasViewingAppointmentsAsync()
        {
            return await this.context.Set<ViewingAppointments>().AnyAsync();
        }
    }
}