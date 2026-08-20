// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Repositories.Implementations
{
    public class LedgerAccountRepository : Repository<LedgerAccount>, ILedgerAccountRepository //, IDisposable
    {
        private readonly ArcoraDbContext context;
        public LedgerAccountRepository(ArcoraDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<LedgerAccount>> GetLedgerAccountAsync()
        {
            return await this.context.LedgerAccounts.AsNoTracking().Include(x => x.Organization).ToListAsync();
        }

        public async Task<bool> HasLedgerAccountsAsync()
        {
            return await this.context.Set<LedgerAccount>().AnyAsync();
        }
    }
}