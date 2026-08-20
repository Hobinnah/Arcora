// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;

namespace Arcora.Api.Repositories.Interfaces
{
    public interface IListingRuleRepository : IRepository<ListingRule>
    {
        Task<List<ListingRule>> GetListingRuleAsync();
        Task<bool> HasListingRulesAsync();
    }
}