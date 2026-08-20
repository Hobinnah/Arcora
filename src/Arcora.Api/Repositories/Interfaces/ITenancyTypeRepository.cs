// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;

namespace Arcora.Api.Repositories.Interfaces
{
    public interface ITenancyTypeRepository : IRepository<TenancyType>
    {
        Task<bool> HasTenancyTypesAsync();
    }
}