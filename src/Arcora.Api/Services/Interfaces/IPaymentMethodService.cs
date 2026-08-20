// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.DTOs;
using Arcora.Api.Models;

namespace Arcora.Api.Services.Interfaces
{
    public interface IPaymentMethodService
    {
        /// <summary>
        /// Retrieves all payment methods with optional paging support.
        /// </summary>
        /// <param name = "paging"></param>
        /// <returns></returns>
        Task<PagedResult<PaymentMethodDto>> GetAll(Paging paging);
        /// <summary>
        /// Retrieves a payment method by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task<PaymentMethodDto?> GetID(Guid ID);
        /// <summary>
        /// Creates a new payment method entry.
        /// </summary>
        /// <param name = "paymentMethodDto"></param>
        /// <returns></returns>
        Task<PaymentMethodDto> CreatePaymentMethod(PaymentMethodDto paymentMethodDto);
        /// <summary>
        /// Updates an existing payment method entry by its ID.
        /// </summary>
        /// <param name = "id"></param>
        /// <param name = "paymentmethodDto"></param>
        /// <returns></returns>
        Task<PaymentMethodDto?> UpdatePaymentMethod(Guid id, PaymentMethodDto paymentmethodDto);
        /// <summary>
        /// Deletes a paymentmethod entry by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task DeletePaymentMethod(Guid ID);
    }
}