// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;

namespace Arcora.Api.Repositories.Interfaces
{
    public interface IPaymentReminderRepository : IRepository<PaymentReminder>
    {
        Task<List<PaymentReminder>> GetPaymentReminderAsync();
        Task<bool> HasPaymentRemindersAsync();
    }
}