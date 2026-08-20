// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Repositories.Implementations
{
    public class ListingRepository : Repository<Listing>, IListingRepository //, IDisposable
    {
        private readonly ArcoraDbContext context;
        public ListingRepository(ArcoraDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<Listing>> GetListingAsync()
        {
            return await this.context.Listings.AsNoTracking().Include(x => x.RentalUnit).Include(x => x.ListingType).Include(x => x.Organization).ToListAsync();
        }

        public async Task<bool> HasListingsAsync()
        {
            return await this.context.Set<Listing>().AnyAsync();
        }
    }
}