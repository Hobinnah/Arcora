// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Repositories.Implementations
{
    public class FraudCaseRepository : Repository<FraudCase>, IFraudCaseRepository //, IDisposable
    {
        private readonly ArcoraDbContext context;
        public FraudCaseRepository(ArcoraDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<FraudCase>> GetFraudCaseAsync()
        {
            return await ApplyDefaultOrder(this.context.FraudCases.AsNoTracking().Include(x => x.Tenant).Include(x => x.Organization).Include(x => x.Lease).Include(x => x.PaymentIntent).Include(x => x.Payment).Include(x => x.Chargeback)).ToListAsync();
        }

        public async Task<bool> HasFraudCasesAsync()
        {
            return await this.context.Set<FraudCase>().AnyAsync();
        }
    }
}