// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Repositories.Implementations
{
    public class TenantEmploymentRepository : Repository<TenantEmployment>, ITenantEmploymentRepository //, IDisposable
    {
        private readonly ArcoraDbContext context;
        public TenantEmploymentRepository(ArcoraDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<TenantEmployment>> GetTenantEmploymentAsync()
        {
            return await ApplyDefaultOrder(this.context.TenantEmployments.AsNoTracking().Include(x => x.Tenant)).ToListAsync();
        }

        public async Task<bool> HasTenantEmploymentsAsync()
        {
            return await this.context.Set<TenantEmployment>().AnyAsync();
        }
    }
}