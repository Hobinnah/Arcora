// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;

namespace Arcora.Api.Repositories.Interfaces
{
    public interface IListingPolicyRepository : IRepository<ListingPolicy>
    {
        Task<List<ListingPolicy>> GetListingPolicyAsync();
        Task<bool> HasListingPoliciesAsync();
    }
}