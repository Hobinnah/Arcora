// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Repositories.Implementations
{
    public class LeaseRecurringChargesRepository : Repository<LeaseRecurringCharges>, ILeaseRecurringChargesRepository //, IDisposable
    {
        private readonly ArcoraDbContext context;
        public LeaseRecurringChargesRepository(ArcoraDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<LeaseRecurringCharges>> GetLeaseRecurringChargesAsync()
        {
            return await this.context.LeaseRecurringCharges.AsNoTracking().Include(x => x.Lease).Include(x => x.Fee).ToListAsync();
        }

        public async Task<bool> HasLeaseRecurringChargesAsync()
        {
            return await this.context.Set<LeaseRecurringCharges>().AnyAsync();
        }
    }
}