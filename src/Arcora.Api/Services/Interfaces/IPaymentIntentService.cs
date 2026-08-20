// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.DTOs;
using Arcora.Api.Models;

namespace Arcora.Api.Services.Interfaces
{
    public interface IPaymentIntentService
    {
        /// <summary>
        /// Retrieves all payment intents with optional paging support.
        /// </summary>
        /// <param name = "paging"></param>
        /// <returns></returns>
        Task<PagedResult<PaymentIntentDto>> GetAll(Paging paging);
        /// <summary>
        /// Retrieves a payment intent by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task<PaymentIntentDto?> GetID(Guid ID);
        /// <summary>
        /// Creates a new payment intent entry.
        /// </summary>
        /// <param name = "paymentIntentDto"></param>
        /// <returns></returns>
        Task<PaymentIntentDto> CreatePaymentIntent(PaymentIntentDto paymentIntentDto);
        /// <summary>
        /// Updates an existing payment intent entry by its ID.
        /// </summary>
        /// <param name = "id"></param>
        /// <param name = "paymentintentDto"></param>
        /// <returns></returns>
        Task<PaymentIntentDto?> UpdatePaymentIntent(Guid id, PaymentIntentDto paymentintentDto);
        /// <summary>
        /// Deletes a paymentintent entry by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task DeletePaymentIntent(Guid ID);
        /// <summary>
        /// Updates the status of a paymentintent by its ID.
        /// </summary>
        /// <param name = "id">The unique identifier of the paymentintent to update.</param>
        /// <param name = "status">The new status value to assign to the paymentintent.</param>
        /// <returns>
        /// Returns <see cref = "PaymentIntentDto"/> with the updated paymentintent if successful, or null if not found.
        /// </returns>
        Task<PaymentIntentDto?> UpdatePaymentIntentStatus(Guid id, string status);
    }
}