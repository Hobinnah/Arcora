// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Repositories.Implementations
{
    public class ReceiptMasterRepository : Repository<ReceiptMaster>, IReceiptMasterRepository //, IDisposable
    {
        private readonly ArcoraDbContext context;
        public ReceiptMasterRepository(ArcoraDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<ReceiptMaster>> GetReceiptMasterAsync()
        {
            return await ApplyDefaultOrder(this.context.ReceiptMasters.AsNoTracking().Include(x => x.Payment).Include(x => x.Tenant)).ToListAsync();
        }

        public async Task<bool> HasReceiptMastersAsync()
        {
            return await this.context.Set<ReceiptMaster>().AnyAsync();
        }
    }
}