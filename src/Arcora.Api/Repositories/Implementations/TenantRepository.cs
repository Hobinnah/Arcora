// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Repositories.Implementations
{
    public class TenantRepository : Repository<Tenant>, ITenantRepository //, IDisposable
    {
        private readonly ArcoraDbContext context;
        public TenantRepository(ArcoraDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<Tenant>> GetTenantAsync()
        {
            return await ApplyDefaultOrder(this.context.Tenants.AsNoTracking().Include(x => x.User)).ToListAsync();
        }

        public async Task<Tenant?> GetTenantByUserIDAsync(long userID)
        {
            return await this.context.Tenants.AsNoTracking().Include(x => x.User).FirstOrDefaultAsync(x => x.UserID == userID);
        }

        public async Task<bool> HasTenantsAsync()
        {
            return await this.context.Set<Tenant>().AnyAsync();
        }
    }
}