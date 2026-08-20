// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;

namespace Arcora.Api.Repositories.Interfaces
{
    public interface ILeaseOccupantsRepository : IRepository<LeaseOccupants>
    {
        Task<List<LeaseOccupants>> GetLeaseOccupantsAsync();
        Task<bool> HasLeaseOccupantsAsync();
    }
}