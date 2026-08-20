// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Repositories.Implementations
{
    public class InvoiceMasterRepository : Repository<InvoiceMaster>, IInvoiceMasterRepository //, IDisposable
    {
        private readonly ArcoraDbContext context;
        public InvoiceMasterRepository(ArcoraDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<InvoiceMaster>> GetInvoiceMasterAsync()
        {
            return await this.context.InvoiceMasters.AsNoTracking().Include(x => x.Lease).Include(x => x.LeaseRenewalLease) // FK to Lease Renewal
            .Include(x => x.Organization).Include(x => x.Tenant).ToListAsync();
        }

        public async Task<bool> HasInvoiceMastersAsync()
        {
            return await this.context.Set<InvoiceMaster>().AnyAsync();
        }
    }
}