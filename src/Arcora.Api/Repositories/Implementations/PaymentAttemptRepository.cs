// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Repositories.Implementations
{
    public class PaymentAttemptRepository : Repository<PaymentAttempt>, IPaymentAttemptRepository //, IDisposable
    {
        private readonly ArcoraDbContext context;
        public PaymentAttemptRepository(ArcoraDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<PaymentAttempt>> GetPaymentAttemptAsync()
        {
            return await ApplyDefaultOrder(this.context.PaymentAttempts.AsNoTracking().Include(x => x.PaymentIntent)).ToListAsync();
        }

        public async Task<bool> HasPaymentAttemptsAsync()
        {
            return await this.context.Set<PaymentAttempt>().AnyAsync();
        }
    }
}