// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Repositories.Implementations
{
    public class OrgPayoutAccountRepository : Repository<OrgPayoutAccount>, IOrgPayoutAccountRepository //, IDisposable
    {
        private readonly ArcoraDbContext context;
        public OrgPayoutAccountRepository(ArcoraDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<OrgPayoutAccount>> GetOrgPayoutAccountAsync()
        {
            return await this.context.OrgPayoutAccounts.AsNoTracking().Include(x => x.Organization).ToListAsync();
        }

        public async Task<bool> HasOrgPayoutAccountsAsync()
        {
            return await this.context.Set<OrgPayoutAccount>().AnyAsync();
        }
    }
}