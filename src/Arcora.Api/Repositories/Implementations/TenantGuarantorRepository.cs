// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Repositories.Implementations
{
    public class TenantGuarantorRepository : Repository<TenantGuarantor>, ITenantGuarantorRepository //, IDisposable
    {
        private readonly ArcoraDbContext context;
        public TenantGuarantorRepository(ArcoraDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<TenantGuarantor>> GetTenantGuarantorAsync()
        {
            return await ApplyDefaultOrder(this.context.TenantGuarantors.AsNoTracking().Include(x => x.Tenant).Include(x => x.User)).ToListAsync();
        }

        public async Task<bool> HasTenantGuarantorsAsync()
        {
            return await this.context.Set<TenantGuarantor>().AnyAsync();
        }
    }
}