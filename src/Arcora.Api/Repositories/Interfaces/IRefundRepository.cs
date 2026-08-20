// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;

namespace Arcora.Api.Repositories.Interfaces
{
    public interface IRefundRepository : IRepository<Refund>
    {
        Task<List<Refund>> GetRefundAsync();
        Task<bool> HasRefundsAsync();
    }
}