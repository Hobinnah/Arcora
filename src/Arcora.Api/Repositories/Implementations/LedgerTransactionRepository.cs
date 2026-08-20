// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Repositories.Implementations
{
    public class LedgerTransactionRepository : Repository<LedgerTransaction>, ILedgerTransactionRepository //, IDisposable
    {
        private readonly ArcoraDbContext context;
        public LedgerTransactionRepository(ArcoraDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<LedgerTransaction>> GetLedgerTransactionAsync()
        {
            return await this.context.LedgerTransactions.AsNoTracking().Include(x => x.Organization).Include(x => x.Payment).Include(x => x.InvoiceMaster).Include(x => x.Refund).Include(x => x.Payout).ToListAsync();
        }

        public async Task<bool> HasLedgerTransactionsAsync()
        {
            return await this.context.Set<LedgerTransaction>().AnyAsync();
        }
    }
}