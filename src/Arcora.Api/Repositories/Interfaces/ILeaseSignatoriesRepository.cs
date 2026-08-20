// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;

namespace Arcora.Api.Repositories.Interfaces
{
    public interface ILeaseSignatoriesRepository : IRepository<LeaseSignatories>
    {
        Task<List<LeaseSignatories>> GetLeaseSignatoriesAsync();
        Task<bool> HasLeaseSignatoriesAsync();
    }
}