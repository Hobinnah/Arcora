// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Repositories.Implementations
{
    public class ListingAccessInstructionRepository : Repository<ListingAccessInstruction>, IListingAccessInstructionRepository //, IDisposable
    {
        private readonly ArcoraDbContext context;
        public ListingAccessInstructionRepository(ArcoraDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<ListingAccessInstruction>> GetListingAccessInstructionAsync()
        {
            return await this.context.ListingAccessInstructions.AsNoTracking().Include(x => x.Listing).Include(x => x.Lease).ToListAsync();
        }

        public async Task<bool> HasListingAccessInstructionsAsync()
        {
            return await this.context.Set<ListingAccessInstruction>().AnyAsync();
        }
    }
}