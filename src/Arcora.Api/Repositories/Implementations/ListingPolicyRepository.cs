// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Repositories.Implementations
{
    public class ListingPolicyRepository : Repository<ListingPolicy>, IListingPolicyRepository //, IDisposable
    {
        private readonly ArcoraDbContext context;
        public ListingPolicyRepository(ArcoraDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<ListingPolicy>> GetListingPolicyAsync()
        {
            return await ApplyDefaultOrder(this.context.ListingPolicies.AsNoTracking().Include(x => x.Listing)).ToListAsync();
        }

        public async Task<bool> HasListingPoliciesAsync()
        {
            return await this.context.Set<ListingPolicy>().AnyAsync();
        }
    }
}