// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;

namespace Arcora.Api.Repositories.Interfaces
{
    public interface IDisputeRepository : IRepository<Dispute>
    {
        Task<List<Dispute>> GetDisputeAsync();
        Task<bool> HasDisputesAsync();
    }
}