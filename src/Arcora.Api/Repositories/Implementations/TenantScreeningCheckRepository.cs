// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Repositories.Implementations
{
    public class TenantScreeningCheckRepository : Repository<TenantScreeningCheck>, ITenantScreeningCheckRepository //, IDisposable
    {
        private readonly ArcoraDbContext context;
        public TenantScreeningCheckRepository(ArcoraDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<TenantScreeningCheck>> GetTenantScreeningCheckAsync()
        {
            return await this.context.TenantScreeningChecks.AsNoTracking().Include(x => x.Tenant).ToListAsync();
        }

        public async Task<bool> HasTenantScreeningChecksAsync()
        {
            return await this.context.Set<TenantScreeningCheck>().AnyAsync();
        }
    }
}