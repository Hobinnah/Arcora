// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Repositories.Implementations
{
    public class PaymentRepository : Repository<Payment>, IPaymentRepository //, IDisposable
    {
        private readonly ArcoraDbContext context;
        public PaymentRepository(ArcoraDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<Payment>> GetPaymentAsync()
        {
            return await ApplyDefaultOrder(this.context.Payments.AsNoTracking().Include(x => x.PaymentIntent).Include(x => x.Tenant)).ToListAsync();
        }

        public async Task<bool> HasPaymentsAsync()
        {
            return await this.context.Set<Payment>().AnyAsync();
        }
    }
}