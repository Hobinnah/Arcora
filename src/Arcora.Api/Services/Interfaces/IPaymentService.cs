// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.DTOs;
using Arcora.Api.Models;

namespace Arcora.Api.Services.Interfaces
{
    public interface IPaymentService
    {
        /// <summary>
        /// Retrieves all payments with optional paging support.
        /// </summary>
        /// <param name = "paging"></param>
        /// <returns></returns>
        Task<PagedResult<PaymentDto>> GetAll(Paging paging);
        /// <summary>
        /// Retrieves a payment by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task<PaymentDto?> GetID(Guid ID);
        /// <summary>
        /// Creates a new payment entry.
        /// </summary>
        /// <param name = "paymentDto"></param>
        /// <returns></returns>
        Task<PaymentDto> CreatePayment(PaymentDto paymentDto);
        /// <summary>
        /// Updates an existing payment entry by its ID.
        /// </summary>
        /// <param name = "id"></param>
        /// <param name = "paymentDto"></param>
        /// <returns></returns>
        Task<PaymentDto?> UpdatePayment(Guid id, PaymentDto paymentDto);
        /// <summary>
        /// Deletes a payment entry by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task DeletePayment(Guid ID);
        /// <summary>
        /// Updates the status of a payment by its ID.
        /// </summary>
        /// <param name = "id">The unique identifier of the payment to update.</param>
        /// <param name = "status">The new status value to assign to the payment.</param>
        /// <returns>
        /// Returns <see cref = "PaymentDto"/> with the updated payment if successful, or null if not found.
        /// </returns>
        Task<PaymentDto?> UpdatePaymentStatus(Guid id, string status);
    }
}