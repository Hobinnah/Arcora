// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Repositories.Implementations
{
    public class SecurityDepositRepository : Repository<SecurityDeposit>, ISecurityDepositRepository //, IDisposable
    {
        private readonly ArcoraDbContext context;
        public SecurityDepositRepository(ArcoraDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<SecurityDeposit>> GetSecurityDepositAsync()
        {
            return await this.context.SecurityDeposits.AsNoTracking().Include(x => x.Lease).Include(x => x.Tenant).Include(x => x.Organization).ToListAsync();
        }

        public async Task<bool> HasSecurityDepositsAsync()
        {
            return await this.context.Set<SecurityDeposit>().AnyAsync();
        }
    }
}