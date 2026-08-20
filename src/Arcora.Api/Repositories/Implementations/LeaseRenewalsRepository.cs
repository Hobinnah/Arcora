// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Repositories.Implementations
{
    public class LeaseRenewalsRepository : Repository<LeaseRenewals>, ILeaseRenewalsRepository //, IDisposable
    {
        private readonly ArcoraDbContext context;
        public LeaseRenewalsRepository(ArcoraDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<LeaseRenewals>> GetLeaseRenewalsAsync()
        {
            return await this.context.LeaseRenewals.AsNoTracking().Include(x => x.Lease).ToListAsync();
        }

        public async Task<bool> HasLeaseRenewalsAsync()
        {
            return await this.context.Set<LeaseRenewals>().AnyAsync();
        }
    }
}