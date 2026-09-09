// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Repositories.Implementations
{
    public class ListingAmenityRepository : Repository<ListingAmenity>, IListingAmenityRepository //, IDisposable
    {
        private readonly ArcoraDbContext context;
        public ListingAmenityRepository(ArcoraDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<ListingAmenity>> GetListingAmenityAsync()
        {
            return await ApplyDefaultOrder(this.context.ListingAmenities.AsNoTracking().Include(x => x.Listing).Include(x => x.AmenityCatalog) // FK to AmenityCatalog
            ).ToListAsync();
        }

        public async Task<bool> HasListingAmenitiesAsync()
        {
            return await this.context.Set<ListingAmenity>().AnyAsync();
        }
    }
}