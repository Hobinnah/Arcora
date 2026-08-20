// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;

namespace Arcora.Api.Repositories.Interfaces
{
    public interface IPayoutRepository : IRepository<Payout>
    {
        Task<List<Payout>> GetPayoutAsync();
        Task<bool> HasPayoutsAsync();
    }
}