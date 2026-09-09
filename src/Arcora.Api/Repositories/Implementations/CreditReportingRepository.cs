// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Repositories.Implementations
{
    public class CreditReportingRepository : Repository<CreditReporting>, ICreditReportingRepository //, IDisposable
    {
        private readonly ArcoraDbContext context;
        public CreditReportingRepository(ArcoraDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<CreditReporting>> GetCreditReportingAsync()
        {
            return await ApplyDefaultOrder(this.context.CreditReportings.AsNoTracking().Include(x => x.CreditReportingEnrollment).Include(x => x.InvoiceMaster).Include(x => x.Payment)).ToListAsync();
        }

        public async Task<bool> HasCreditReportingsAsync()
        {
            return await this.context.Set<CreditReporting>().AnyAsync();
        }
    }
}