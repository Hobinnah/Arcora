// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Repositories.Implementations
{
    public class OrgSubscriptionRepository : Repository<OrgSubscription>, IOrgSubscriptionRepository //, IDisposable
    {
        private readonly ArcoraDbContext context;
        public OrgSubscriptionRepository(ArcoraDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<OrgSubscription>> GetOrgSubscriptionAsync()
        {
            return await ApplyDefaultOrder(this.context.OrgSubscriptions.AsNoTracking().Include(x => x.Organization).Include(x => x.SubscriptionPlan)).ToListAsync();
        }

        public async Task<bool> HasOrgSubscriptionsAsync()
        {
            return await this.context.Set<OrgSubscription>().AnyAsync();
        }
    }
}