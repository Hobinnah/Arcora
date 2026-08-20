// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;

namespace Arcora.Api.Repositories.Interfaces
{
    public interface IPaymentMethodRepository : IRepository<PaymentMethod>
    {
        Task<List<PaymentMethod>> GetPaymentMethodAsync();
        Task<bool> HasPaymentMethodsAsync();
    }
}