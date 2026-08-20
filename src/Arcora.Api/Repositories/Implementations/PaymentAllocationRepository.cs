// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Repositories.Implementations
{
    public class PaymentAllocationRepository : Repository<PaymentAllocation>, IPaymentAllocationRepository //, IDisposable
    {
        private readonly ArcoraDbContext context;
        public PaymentAllocationRepository(ArcoraDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<PaymentAllocation>> GetPaymentAllocationAsync()
        {
            return await this.context.PaymentAllocations.AsNoTracking().Include(x => x.Payment).Include(x => x.InvoiceMaster).Include(x => x.InvoiceDetail).ToListAsync();
        }

        public async Task<bool> HasPaymentAllocationsAsync()
        {
            return await this.context.Set<PaymentAllocation>().AnyAsync();
        }
    }
}