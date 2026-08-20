// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Repositories.Implementations
{
    public class LeaseRepository : Repository<Lease>, ILeaseRepository //, IDisposable
    {
        private readonly ArcoraDbContext context;
        public LeaseRepository(ArcoraDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<Lease>> GetLeaseAsync()
        {
            return await this.context.Leases.AsNoTracking().Include(x => x.Organization).Include(x => x.Listing).Include(x => x.RentalUnit).Include(x => x.TenancyType).Include(x => x.Tenant).Include(x => x.RentalApplication).ToListAsync();
        }

        public async Task<bool> HasLeasesAsync()
        {
            return await this.context.Set<Lease>().AnyAsync();
        }
    }
}