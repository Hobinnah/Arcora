// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Repositories.Implementations
{
    public class LedgerEntryRepository : Repository<LedgerEntry>, ILedgerEntryRepository //, IDisposable
    {
        private readonly ArcoraDbContext context;
        public LedgerEntryRepository(ArcoraDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<LedgerEntry>> GetLedgerEntryAsync()
        {
            return await ApplyDefaultOrder(this.context.LedgerEntries.AsNoTracking().Include(x => x.LedgerTransaction).Include(x => x.LedgerAccount)).ToListAsync();
        }

        public async Task<bool> HasLedgerEntriesAsync()
        {
            return await this.context.Set<LedgerEntry>().AnyAsync();
        }
    }
}