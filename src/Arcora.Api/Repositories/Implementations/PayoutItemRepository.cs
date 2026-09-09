// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Repositories.Implementations
{
    public class PayoutItemRepository : Repository<PayoutItem>, IPayoutItemRepository //, IDisposable
    {
        private readonly ArcoraDbContext context;
        public PayoutItemRepository(ArcoraDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<PayoutItem>> GetPayoutItemAsync()
        {
            return await ApplyDefaultOrder(this.context.PayoutItems.AsNoTracking().Include(x => x.Payout).Include(x => x.Payment)).ToListAsync();
        }

        public async Task<bool> HasPayoutItemsAsync()
        {
            return await this.context.Set<PayoutItem>().AnyAsync();
        }
    }
}