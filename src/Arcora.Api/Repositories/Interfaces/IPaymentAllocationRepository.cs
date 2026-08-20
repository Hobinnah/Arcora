// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;

namespace Arcora.Api.Repositories.Interfaces
{
    public interface IPaymentAllocationRepository : IRepository<PaymentAllocation>
    {
        Task<List<PaymentAllocation>> GetPaymentAllocationAsync();
        Task<bool> HasPaymentAllocationsAsync();
    }
}