// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;

namespace Arcora.Api.Repositories.Interfaces
{
    public interface ITenantScreeningCheckRepository : IRepository<TenantScreeningCheck>
    {
        Task<List<TenantScreeningCheck>> GetTenantScreeningCheckAsync();
        Task<bool> HasTenantScreeningChecksAsync();
    }
}