// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Repositories.Implementations
{
    public class RefundRepository : Repository<Refund>, IRefundRepository //, IDisposable
    {
        private readonly ArcoraDbContext context;
        public RefundRepository(ArcoraDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<Refund>> GetRefundAsync()
        {
            return await this.context.Refunds.AsNoTracking().Include(x => x.Payment).Include(x => x.Tenant).ToListAsync();
        }

        public async Task<bool> HasRefundsAsync()
        {
            return await this.context.Set<Refund>().AnyAsync();
        }
    }
}