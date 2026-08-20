// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;

namespace Arcora.Api.Repositories.Interfaces
{
    public interface ITenantRepository : IRepository<Tenant>
    {
        Task<List<Tenant>> GetTenantAsync();
        Task<bool> HasTenantsAsync();
    }
}