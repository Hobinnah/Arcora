// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Repositories.Implementations
{
    public class ChargebackRepository : Repository<Chargeback>, IChargebackRepository //, IDisposable
    {
        private readonly ArcoraDbContext context;
        public ChargebackRepository(ArcoraDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<Chargeback>> GetChargebackAsync()
        {
            return await this.context.Chargebacks.AsNoTracking().Include(x => x.Payment).Include(x => x.Tenant).Include(x => x.Organization).ToListAsync();
        }

        public async Task<bool> HasChargebacksAsync()
        {
            return await this.context.Set<Chargeback>().AnyAsync();
        }
    }
}