// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Repositories.Implementations
{
    public class PaymentIntentRepository : Repository<PaymentIntent>, IPaymentIntentRepository //, IDisposable
    {
        private readonly ArcoraDbContext context;
        public PaymentIntentRepository(ArcoraDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<PaymentIntent>> GetPaymentIntentAsync()
        {
            return await ApplyDefaultOrder(this.context.PaymentIntents.AsNoTracking().Include(x => x.InvoiceMaster).Include(x => x.AutopayMandate).Include(x => x.Tenant).Include(x => x.PaymentMethod)).ToListAsync();
        }

        public async Task<bool> HasPaymentIntentsAsync()
        {
            return await this.context.Set<PaymentIntent>().AnyAsync();
        }
    }
}