// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;

namespace Arcora.Api.Repositories.Interfaces
{
    public interface ILeaseSignatoriesRepository : IRepository<LeaseSignatory>
    {
        Task<List<LeaseSignatory>> GetLeaseSignatoriesAsync();
        Task<bool> HasLeaseSignatoriesAsync();
    }
}