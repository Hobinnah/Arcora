// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.DTOs;
using Arcora.Api.Models;

namespace Arcora.Api.Services.Interfaces
{
    public interface IPaymentProviderEventService
    {
        /// <summary>
        /// Retrieves all payment provider events with optional paging support.
        /// </summary>
        /// <param name = "paging"></param>
        /// <returns></returns>
        Task<PagedResult<PaymentProviderEventDto>> GetAll(Paging paging);
        /// <summary>
        /// Retrieves a payment provider event by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task<PaymentProviderEventDto?> GetID(Guid ID);
        /// <summary>
        /// Creates a new payment provider event entry.
        /// </summary>
        /// <param name = "paymentProviderEventDto"></param>
        /// <returns></returns>
        Task<PaymentProviderEventDto> CreatePaymentProviderEvent(PaymentProviderEventDto paymentProviderEventDto);
        /// <summary>
        /// Updates an existing payment provider event entry by its ID.
        /// </summary>
        /// <param name = "id"></param>
        /// <param name = "paymentprovidereventDto"></param>
        /// <returns></returns>
        Task<PaymentProviderEventDto?> UpdatePaymentProviderEvent(Guid id, PaymentProviderEventDto paymentprovidereventDto);
        /// <summary>
        /// Deletes a paymentproviderevent entry by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task DeletePaymentProviderEvent(Guid ID);
    }
}