// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Repositories.Implementations
{
    public class PaymentMethodRepository : Repository<PaymentMethod>, IPaymentMethodRepository //, IDisposable
    {
        private readonly ArcoraDbContext context;
        public PaymentMethodRepository(ArcoraDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<PaymentMethod>> GetPaymentMethodAsync()
        {
            return await this.context.PaymentMethods.AsNoTracking().Include(x => x.Tenant).ToListAsync();
        }

        public async Task<bool> HasPaymentMethodsAsync()
        {
            return await this.context.Set<PaymentMethod>().AnyAsync();
        }
    }
}