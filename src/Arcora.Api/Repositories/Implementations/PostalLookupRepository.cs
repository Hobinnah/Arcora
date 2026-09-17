// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Repositories.Implementations
{
    public class PostalLookupRepository : Repository<PostalLookupSuggestion>, IPostalLookupRepository
    {
        private readonly ArcoraDbContext context;
        public PostalLookupRepository(ArcoraDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<PostalLookupSuggestion>> GetByLookupKeyAsync(string lookupKey)
        {
            if (string.IsNullOrWhiteSpace(lookupKey)) return new List<PostalLookupSuggestion>();
            return await ApplyDefaultOrder(this.context.Set<PostalLookupSuggestion>().AsNoTracking().Where(x => x.LookupKey == lookupKey)).ToListAsync();
        }

        public async Task<bool> HasAnyAsync()
        {
            return await this.context.Set<PostalLookupSuggestion>().AnyAsync();
        }
    }
}
