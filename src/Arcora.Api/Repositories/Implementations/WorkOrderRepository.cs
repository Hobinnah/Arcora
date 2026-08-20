// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Repositories.Implementations
{
    public class WorkOrderRepository : Repository<WorkOrder>, IWorkOrderRepository //, IDisposable
    {
        private readonly ArcoraDbContext context;
        public WorkOrderRepository(ArcoraDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<WorkOrder>> GetWorkOrderAsync()
        {
            return await this.context.WorkOrders.AsNoTracking().Include(x => x.MaintenanceRequest).Include(x => x.Contractor).Include(x => x.AssignedOrganizationMember) // FK to OrganizationMember
            .Include(x => x.Lease).ToListAsync();
        }

        public async Task<bool> HasWorkOrdersAsync()
        {
            return await this.context.Set<WorkOrder>().AnyAsync();
        }
    }
}