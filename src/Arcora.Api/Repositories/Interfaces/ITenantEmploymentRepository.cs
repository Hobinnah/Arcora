// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;

namespace Arcora.Api.Repositories.Interfaces
{
    public interface ITenantEmploymentRepository : IRepository<TenantEmployment>
    {
        Task<List<TenantEmployment>> GetTenantEmploymentAsync();
        Task<bool> HasTenantEmploymentsAsync();
    }
}