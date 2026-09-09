// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Repositories.Implementations
{
    public class LeaseOccupantsRepository : Repository<LeaseOccupants>, ILeaseOccupantsRepository //, IDisposable
    {
        private readonly ArcoraDbContext context;
        public LeaseOccupantsRepository(ArcoraDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<LeaseOccupants>> GetLeaseOccupantsAsync()
        {
            return await ApplyDefaultOrder(this.context.LeaseOccupants.AsNoTracking().Include(x => x.Lease).Include(x => x.LeaseRenewalLeaseRenewals) // FK to LeaseRenewals
            .Include(x => x.Tenant).Include(x => x.User)).ToListAsync();
        }

        public async Task<bool> HasLeaseOccupantsAsync()
        {
            return await this.context.Set<LeaseOccupants>().AnyAsync();
        }
    }
}