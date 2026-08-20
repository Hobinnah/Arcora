// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Repositories.Implementations
{
    public class FeeRepository : Repository<Fee>, IFeeRepository //, IDisposable
    {
        private readonly ArcoraDbContext context;
        public FeeRepository(ArcoraDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<Fee>> GetFeeAsync()
        {
            return await this.context.Fees.AsNoTracking().Include(x => x.FeeType).Include(x => x.Organization).ToListAsync();
        }

        public async Task<bool> HasFeesAsync()
        {
            return await this.context.Set<Fee>().AnyAsync();
        }
    }
}