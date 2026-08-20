// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Repositories.Implementations
{
    public class CreditReportingEnrollmentRepository : Repository<CreditReportingEnrollment>, ICreditReportingEnrollmentRepository //, IDisposable
    {
        private readonly ArcoraDbContext context;
        public CreditReportingEnrollmentRepository(ArcoraDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<CreditReportingEnrollment>> GetCreditReportingEnrollmentAsync()
        {
            return await this.context.CreditReportingEnrollments.AsNoTracking().Include(x => x.Lease).Include(x => x.Tenant).ToListAsync();
        }

        public async Task<bool> HasCreditReportingEnrollmentsAsync()
        {
            return await this.context.Set<CreditReportingEnrollment>().AnyAsync();
        }
    }
}