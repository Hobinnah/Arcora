// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;

namespace Arcora.Api.Repositories.Interfaces
{
    public interface IOrgSubscriptionRepository : IRepository<OrgSubscription>
    {
        Task<List<OrgSubscription>> GetOrgSubscriptionAsync();
        Task<bool> HasOrgSubscriptionsAsync();
    }
}