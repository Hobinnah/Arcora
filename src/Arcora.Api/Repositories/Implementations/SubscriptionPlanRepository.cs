// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Repositories.Implementations
{
    public class SubscriptionPlanRepository : Repository<SubscriptionPlan>, ISubscriptionPlanRepository //, IDisposable
    {
        private readonly ArcoraDbContext context;
        public SubscriptionPlanRepository(ArcoraDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<bool> HasSubscriptionPlansAsync()
        {
            return await this.context.Set<SubscriptionPlan>().AnyAsync();
        }
    }
}