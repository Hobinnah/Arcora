// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;

namespace Arcora.Api.Repositories.Interfaces
{
    public interface IChargebackRepository : IRepository<Chargeback>
    {
        Task<List<Chargeback>> GetChargebackAsync();
        Task<bool> HasChargebacksAsync();
    }
}