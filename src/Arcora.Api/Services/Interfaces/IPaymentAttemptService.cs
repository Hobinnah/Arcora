// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.DTOs;
using Arcora.Api.Models;

namespace Arcora.Api.Services.Interfaces
{
    public interface IPaymentAttemptService
    {
        /// <summary>
        /// Retrieves all payment attempts with optional paging support.
        /// </summary>
        /// <param name = "paging"></param>
        /// <returns></returns>
        Task<PagedResult<PaymentAttemptDto>> GetAll(Paging paging);
        /// <summary>
        /// Retrieves a payment attempt by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task<PaymentAttemptDto?> GetID(Guid ID);
        /// <summary>
        /// Creates a new payment attempt entry.
        /// </summary>
        /// <param name = "paymentAttemptDto"></param>
        /// <returns></returns>
        Task<PaymentAttemptDto> CreatePaymentAttempt(PaymentAttemptDto paymentAttemptDto);
        /// <summary>
        /// Updates an existing payment attempt entry by its ID.
        /// </summary>
        /// <param name = "id"></param>
        /// <param name = "paymentattemptDto"></param>
        /// <returns></returns>
        Task<PaymentAttemptDto?> UpdatePaymentAttempt(Guid id, PaymentAttemptDto paymentattemptDto);
        /// <summary>
        /// Deletes a paymentattempt entry by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task DeletePaymentAttempt(Guid ID);
        /// <summary>
        /// Updates the status of a paymentattempt by its ID.
        /// </summary>
        /// <param name = "id">The unique identifier of the paymentattempt to update.</param>
        /// <param name = "status">The new status value to assign to the paymentattempt.</param>
        /// <returns>
        /// Returns <see cref = "PaymentAttemptDto"/> with the updated paymentattempt if successful, or null if not found.
        /// </returns>
        Task<PaymentAttemptDto?> UpdatePaymentAttemptStatus(Guid id, string status);
    }
}