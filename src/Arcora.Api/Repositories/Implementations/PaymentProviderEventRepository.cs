// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Repositories.Implementations
{
    public class PaymentProviderEventRepository : Repository<PaymentProviderEvent>, IPaymentProviderEventRepository //, IDisposable
    {
        private readonly ArcoraDbContext context;
        public PaymentProviderEventRepository(ArcoraDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<bool> HasPaymentProviderEventsAsync()
        {
            return await this.context.Set<PaymentProviderEvent>().AnyAsync();
        }
    }
}