// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Repositories.Implementations
{
    public class CreditReportingConsentAuditRepository : Repository<CreditReportingConsentAudit>, ICreditReportingConsentAuditRepository //, IDisposable
    {
        private readonly ArcoraDbContext context;
        public CreditReportingConsentAuditRepository(ArcoraDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<CreditReportingConsentAudit>> GetCreditReportingConsentAuditAsync()
        {
            return await ApplyDefaultOrder(this.context.CreditReportingConsentAudits.AsNoTracking().Include(x => x.CreditReportingEnrollment).Include(x => x.Tenant)).ToListAsync();
        }

        public async Task<bool> HasCreditReportingConsentAuditsAsync()
        {
            return await this.context.Set<CreditReportingConsentAudit>().AnyAsync();
        }
    }
}