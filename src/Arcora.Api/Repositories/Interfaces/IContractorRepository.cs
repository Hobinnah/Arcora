// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;

namespace Arcora.Api.Repositories.Interfaces
{
    public interface IContractorRepository : IRepository<Contractor>
    {
        Task<List<Contractor>> GetContractorAsync();
        Task<bool> HasContractorsAsync();
    }
}