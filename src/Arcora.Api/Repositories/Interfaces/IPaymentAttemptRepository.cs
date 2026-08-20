// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;

namespace Arcora.Api.Repositories.Interfaces
{
    public interface IPaymentAttemptRepository : IRepository<PaymentAttempt>
    {
        Task<List<PaymentAttempt>> GetPaymentAttemptAsync();
        Task<bool> HasPaymentAttemptsAsync();
    }
}