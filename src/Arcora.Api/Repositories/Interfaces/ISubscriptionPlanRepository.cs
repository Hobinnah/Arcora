// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;

namespace Arcora.Api.Repositories.Interfaces
{
    public interface ISubscriptionPlanRepository : IRepository<SubscriptionPlan>
    {
        Task<bool> HasSubscriptionPlansAsync();
    }
}