// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.DTOs;
using Arcora.Api.Models;

namespace Arcora.Api.Services.Interfaces
{
    public interface IPaymentReminderService
    {
        /// <summary>
        /// Retrieves all payment reminders with optional paging support.
        /// </summary>
        /// <param name = "paging"></param>
        /// <returns></returns>
        Task<PagedResult<PaymentReminderDto>> GetAll(Paging paging);
        /// <summary>
        /// Retrieves a payment reminder by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task<PaymentReminderDto?> GetID(Guid ID);
        /// <summary>
        /// Creates a new payment reminder entry.
        /// </summary>
        /// <param name = "paymentReminderDto"></param>
        /// <returns></returns>
        Task<PaymentReminderDto> CreatePaymentReminder(PaymentReminderDto paymentReminderDto);
        /// <summary>
        /// Updates an existing payment reminder entry by its ID.
        /// </summary>
        /// <param name = "id"></param>
        /// <param name = "paymentreminderDto"></param>
        /// <returns></returns>
        Task<PaymentReminderDto?> UpdatePaymentReminder(Guid id, PaymentReminderDto paymentreminderDto);
        /// <summary>
        /// Deletes a paymentreminder entry by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task DeletePaymentReminder(Guid ID);
        /// <summary>
        /// Updates the status of a paymentreminder by its ID.
        /// </summary>
        /// <param name = "id">The unique identifier of the paymentreminder to update.</param>
        /// <param name = "status">The new status value to assign to the paymentreminder.</param>
        /// <returns>
        /// Returns <see cref = "PaymentReminderDto"/> with the updated paymentreminder if successful, or null if not found.
        /// </returns>
        Task<PaymentReminderDto?> UpdatePaymentReminderStatus(Guid id, string status);
    }
}