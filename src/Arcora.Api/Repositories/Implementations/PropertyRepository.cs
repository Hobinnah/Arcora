// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Repositories.Implementations
{
    public class PropertyRepository : Repository<Property>, IPropertyRepository //, IDisposable
    {
        private readonly ArcoraDbContext context;
        public PropertyRepository(ArcoraDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<Property>> GetPropertyAsync()
        {
            return await ApplyDefaultOrder(this.context.Properties.AsNoTracking().Include(x => x.Organization).Include(x => x.Address)).ToListAsync();
        }

        public async Task<bool> HasPropertiesAsync()
        {
            return await this.context.Set<Property>().AnyAsync();
        }
    }
}