// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Repositories.Implementations
{
    public class MaintenanceRequestRepository : Repository<MaintenanceRequest>, IMaintenanceRequestRepository //, IDisposable
    {
        private readonly ArcoraDbContext context;
        public MaintenanceRequestRepository(ArcoraDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<MaintenanceRequest>> GetMaintenanceRequestAsync()
        {
            return await ApplyDefaultOrder(this.context.MaintenanceRequests.AsNoTracking().Include(x => x.Property).Include(x => x.RentalUnit).Include(x => x.Listing).Include(x => x.Lease).Include(x => x.SubmittedByTenant) // FK to Tenant
            .Include(x => x.Category)).ToListAsync();
        }

        public async Task<bool> HasMaintenanceRequestsAsync()
        {
            return await this.context.Set<MaintenanceRequest>().AnyAsync();
        }
    }
}