// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Repositories.Implementations
{
    public class PaymentReminderRepository : Repository<PaymentReminder>, IPaymentReminderRepository //, IDisposable
    {
        private readonly ArcoraDbContext context;
        public PaymentReminderRepository(ArcoraDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<PaymentReminder>> GetPaymentReminderAsync()
        {
            return await ApplyDefaultOrder(this.context.PaymentReminders.AsNoTracking().Include(x => x.InvoiceMaster)).ToListAsync();
        }

        public async Task<bool> HasPaymentRemindersAsync()
        {
            return await this.context.Set<PaymentReminder>().AnyAsync();
        }
    }
}