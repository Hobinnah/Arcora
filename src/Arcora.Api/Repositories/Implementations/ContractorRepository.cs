// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Repositories.Implementations
{
    public class ContractorRepository : Repository<Contractor>, IContractorRepository //, IDisposable
    {
        private readonly ArcoraDbContext context;
        public ContractorRepository(ArcoraDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<Contractor>> GetContractorAsync()
        {
            return await ApplyDefaultOrder(this.context.Contractors.AsNoTracking().Include(x => x.Organization)).ToListAsync();
        }

        public async Task<bool> HasContractorsAsync()
        {
            return await this.context.Set<Contractor>().AnyAsync();
        }
    }
}