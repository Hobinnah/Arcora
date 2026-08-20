// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Repositories.Implementations
{
    public class PayoutRepository : Repository<Payout>, IPayoutRepository //, IDisposable
    {
        private readonly ArcoraDbContext context;
        public PayoutRepository(ArcoraDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<Payout>> GetPayoutAsync()
        {
            return await this.context.Payouts.AsNoTracking().Include(x => x.Organization).Include(x => x.OrgPayoutAccount).ToListAsync();
        }

        public async Task<bool> HasPayoutsAsync()
        {
            return await this.context.Set<Payout>().AnyAsync();
        }
    }
}