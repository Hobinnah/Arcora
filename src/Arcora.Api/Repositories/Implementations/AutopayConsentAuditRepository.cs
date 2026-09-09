// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Repositories.Implementations
{
    public class AutopayConsentAuditRepository : Repository<AutopayConsentAudit>, IAutopayConsentAuditRepository //, IDisposable
    {
        private readonly ArcoraDbContext context;
        public AutopayConsentAuditRepository(ArcoraDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<AutopayConsentAudit>> GetAutopayConsentAuditAsync()
        {
            return await ApplyDefaultOrder(this.context.AutopayConsentAudits.AsNoTracking().Include(x => x.AutopayMandate).Include(x => x.Tenant)).ToListAsync();
        }

        public async Task<bool> HasAutopayConsentAuditsAsync()
        {
            return await this.context.Set<AutopayConsentAudit>().AnyAsync();
        }
    }
}