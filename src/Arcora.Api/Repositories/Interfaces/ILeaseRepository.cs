// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;

namespace Arcora.Api.Repositories.Interfaces
{
    public interface ILeaseRepository : IRepository<Lease>
    {
        Task<List<Lease>> GetLeaseAsync();
        Task<bool> HasLeasesAsync();
    }
}