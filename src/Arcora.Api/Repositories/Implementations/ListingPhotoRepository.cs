// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Repositories.Implementations
{
    public class ListingPhotoRepository : Repository<ListingPhoto>, IListingPhotoRepository //, IDisposable
    {
        private readonly ArcoraDbContext context;
        public ListingPhotoRepository(ArcoraDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<ListingPhoto>> GetListingPhotoAsync()
        {
            return await this.context.ListingPhotos.AsNoTracking().Include(x => x.Listing).ToListAsync();
        }

        public async Task<bool> HasListingPhotosAsync()
        {
            return await this.context.Set<ListingPhoto>().AnyAsync();
        }
    }
}