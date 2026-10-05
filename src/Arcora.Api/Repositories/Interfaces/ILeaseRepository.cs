// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;
using Arcora.Api.Models;

namespace Arcora.Api.Repositories.Interfaces
{
    public interface ILeaseRepository : IRepository<Lease>
    {
        Task<List<Lease>> GetLeaseAsync();
        Task<(List<Lease> Items, int TotalCount)> GetLeasePagedAsync(Paging paging);

        Task<Lease?> GetLeaseByIDAsync(Guid Id);
        Task<Lease?> GetLeaseWithContractContextByIDAsync(Guid Id);
        Task<bool> HasLeasesAsync();
    }
}