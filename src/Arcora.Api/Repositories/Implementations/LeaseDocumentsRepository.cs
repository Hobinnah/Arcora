// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Repositories.Implementations
{
    public class LeaseDocumentsRepository : Repository<LeaseDocuments>, ILeaseDocumentsRepository //, IDisposable
    {
        private readonly ArcoraDbContext context;
        public LeaseDocumentsRepository(ArcoraDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<LeaseDocuments>> GetLeaseDocumentsAsync()
        {
            return await ApplyDefaultOrder(this.context.LeaseDocuments.AsNoTracking().Include(x => x.Lease).Include(x => x.LeaseRenewals) // FK to LeaseRenewals
            ).ToListAsync();
        }

        public async Task<bool> HasLeaseDocumentsAsync()
        {
            return await this.context.Set<LeaseDocuments>().AnyAsync();
        }
    }
}