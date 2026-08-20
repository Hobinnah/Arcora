// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Repositories.Implementations
{
    public class ListingTypeRepository : Repository<ListingType>, IListingTypeRepository //, IDisposable
    {
        private readonly ArcoraDbContext context;
        public ListingTypeRepository(ArcoraDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<bool> HasListingTypesAsync()
        {
            return await this.context.Set<ListingType>().AnyAsync();
        }
    }
}