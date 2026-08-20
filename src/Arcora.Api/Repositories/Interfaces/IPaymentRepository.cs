// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;

namespace Arcora.Api.Repositories.Interfaces
{
    public interface IPaymentRepository : IRepository<Payment>
    {
        Task<List<Payment>> GetPaymentAsync();
        Task<bool> HasPaymentsAsync();
    }
}