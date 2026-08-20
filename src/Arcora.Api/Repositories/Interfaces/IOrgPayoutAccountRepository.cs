// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;

namespace Arcora.Api.Repositories.Interfaces
{
    public interface IOrgPayoutAccountRepository : IRepository<OrgPayoutAccount>
    {
        Task<List<OrgPayoutAccount>> GetOrgPayoutAccountAsync();
        Task<bool> HasOrgPayoutAccountsAsync();
    }
}