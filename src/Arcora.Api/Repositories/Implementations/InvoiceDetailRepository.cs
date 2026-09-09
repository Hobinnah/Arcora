// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Repositories.Implementations
{
    public class InvoiceDetailRepository : Repository<InvoiceDetail>, IInvoiceDetailRepository //, IDisposable
    {
        private readonly ArcoraDbContext context;
        public InvoiceDetailRepository(ArcoraDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<InvoiceDetail>> GetInvoiceDetailAsync()
        {
            return await ApplyDefaultOrder(this.context.InvoiceDetails.AsNoTracking().Include(x => x.InvoiceMaster).Include(x => x.Fee)).ToListAsync();
        }

        public async Task<bool> HasInvoiceDetailsAsync()
        {
            return await this.context.Set<InvoiceDetail>().AnyAsync();
        }
    }
}