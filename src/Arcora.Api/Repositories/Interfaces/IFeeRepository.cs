// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;

namespace Arcora.Api.Repositories.Interfaces
{
    public interface IFeeRepository : IRepository<Fee>
    {
        Task<List<Fee>> GetFeeAsync();
        Task<bool> HasFeesAsync();
    }
}