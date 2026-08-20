// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.DTOs;
using Arcora.Api.Models;

namespace Arcora.Api.Services.Interfaces
{
    public interface IPaymentAllocationService
    {
        /// <summary>
        /// Retrieves all payment allocations with optional paging support.
        /// </summary>
        /// <param name = "paging"></param>
        /// <returns></returns>
        Task<PagedResult<PaymentAllocationDto>> GetAll(Paging paging);
        /// <summary>
        /// Retrieves a payment allocation by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task<PaymentAllocationDto?> GetID(long ID);
        /// <summary>
        /// Creates a new payment allocation entry.
        /// </summary>
        /// <param name = "paymentAllocationDto"></param>
        /// <returns></returns>
        Task<PaymentAllocationDto> CreatePaymentAllocation(PaymentAllocationDto paymentAllocationDto);
        /// <summary>
        /// Updates an existing payment allocation entry by its ID.
        /// </summary>
        /// <param name = "id"></param>
        /// <param name = "paymentallocationDto"></param>
        /// <returns></returns>
        Task<PaymentAllocationDto?> UpdatePaymentAllocation(long id, PaymentAllocationDto paymentallocationDto);
        /// <summary>
        /// Deletes a paymentallocation entry by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task DeletePaymentAllocation(long ID);
    }
}