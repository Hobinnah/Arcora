// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Repositories.Implementations
{
    public class LeaseSignatoriesRepository : Repository<LeaseSignatory>, ILeaseSignatoriesRepository //, IDisposable
    {
        private readonly ArcoraDbContext context;
        public LeaseSignatoriesRepository(ArcoraDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<LeaseSignatory>> GetLeaseSignatoriesAsync()
        {
            return await ApplyDefaultOrder(this.context.LeaseSignatories.AsNoTracking().Include(x => x.User).Include(x => x.Tenant).Include(x => x.OrganizationMember)).ToListAsync();
        }

        public async Task<bool> HasLeaseSignatoriesAsync()
        {
            return await this.context.Set<LeaseSignatory>().AnyAsync();
        }
    }
}