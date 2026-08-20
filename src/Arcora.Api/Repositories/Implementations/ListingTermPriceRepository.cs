// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Repositories.Implementations
{
    public class ListingTermPriceRepository : Repository<ListingTermPrice>, IListingTermPriceRepository //, IDisposable
    {
        private readonly ArcoraDbContext context;
        public ListingTermPriceRepository(ArcoraDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<ListingTermPrice>> GetListingTermPriceAsync()
        {
            return await this.context.ListingTermPrices.AsNoTracking().Include(x => x.Listing).ToListAsync();
        }

        public async Task<bool> HasListingTermPricesAsync()
        {
            return await this.context.Set<ListingTermPrice>().AnyAsync();
        }
    }
}