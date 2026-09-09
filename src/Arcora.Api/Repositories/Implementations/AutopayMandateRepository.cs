// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Repositories.Implementations
{
    public class AutopayMandateRepository : Repository<AutopayMandate>, IAutopayMandateRepository //, IDisposable
    {
        private readonly ArcoraDbContext context;
        public AutopayMandateRepository(ArcoraDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<AutopayMandate>> GetAutopayMandateAsync()
        {
            return await ApplyDefaultOrder(this.context.AutopayMandates.AsNoTracking().Include(x => x.Lease).Include(x => x.Tenant).Include(x => x.PaymentMethod)).ToListAsync();
        }

        public async Task<bool> HasAutopayMandatesAsync()
        {
            return await this.context.Set<AutopayMandate>().AnyAsync();
        }
    }
}