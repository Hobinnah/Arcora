// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;

namespace Arcora.Api.Repositories.Interfaces
{
    public interface ITenantGuarantorRepository : IRepository<TenantGuarantor>
    {
        Task<List<TenantGuarantor>> GetTenantGuarantorAsync();
        Task<bool> HasTenantGuarantorsAsync();
    }
}