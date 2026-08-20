// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Repositories.Implementations
{
    public class DisputeRepository : Repository<Dispute>, IDisputeRepository //, IDisposable
    {
        private readonly ArcoraDbContext context;
        public DisputeRepository(ArcoraDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<Dispute>> GetDisputeAsync()
        {
            return await this.context.Disputes.AsNoTracking().Include(x => x.Tenant).Include(x => x.Organization).Include(x => x.Lease).Include(x => x.InvoiceMaster).Include(x => x.Payment).Include(x => x.Chargeback).Include(x => x.MaintenanceRequest).ToListAsync();
        }

        public async Task<bool> HasDisputesAsync()
        {
            return await this.context.Set<Dispute>().AnyAsync();
        }
    }
}