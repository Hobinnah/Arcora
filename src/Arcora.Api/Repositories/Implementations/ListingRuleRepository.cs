// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Repositories.Implementations
{
    public class ListingRuleRepository : Repository<ListingRule>, IListingRuleRepository //, IDisposable
    {
        private readonly ArcoraDbContext context;
        public ListingRuleRepository(ArcoraDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<ListingRule>> GetListingRuleAsync()
        {
            return await ApplyDefaultOrder(this.context.ListingRules.AsNoTracking().Include(x => x.Listing)).ToListAsync();
        }

        public async Task<bool> HasListingRulesAsync()
        {
            return await this.context.Set<ListingRule>().AnyAsync();
        }
    }
}