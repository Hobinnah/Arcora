// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Repositories.Implementations
{
    public class SecurityDepositTransactionRepository : Repository<SecurityDepositTransaction>, ISecurityDepositTransactionRepository //, IDisposable
    {
        private readonly ArcoraDbContext context;
        public SecurityDepositTransactionRepository(ArcoraDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<SecurityDepositTransaction>> GetSecurityDepositTransactionAsync()
        {
            return await this.context.SecurityDepositTransactions.AsNoTracking().Include(x => x.SecurityDeposit).Include(x => x.Payment).Include(x => x.Refund).Include(x => x.InvoiceMaster).Include(x => x.InvoiceDetail).ToListAsync();
        }

        public async Task<bool> HasSecurityDepositTransactionsAsync()
        {
            return await this.context.Set<SecurityDepositTransaction>().AnyAsync();
        }
    }
}