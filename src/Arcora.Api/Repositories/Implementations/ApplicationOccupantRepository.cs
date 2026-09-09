// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Repositories.Implementations
{
    public class ApplicationOccupantRepository : Repository<ApplicationOccupant>, IApplicationOccupantRepository //, IDisposable
    {
        private readonly ArcoraDbContext context;
        public ApplicationOccupantRepository(ArcoraDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<ApplicationOccupant>> GetApplicationOccupantAsync()
        {
            return await ApplyDefaultOrder(this.context.ApplicationOccupants.AsNoTracking().Include(x => x.Tenant).Include(x => x.User)).ToListAsync();
        }

        public async Task<bool> HasApplicationOccupantsAsync()
        {
            return await this.context.Set<ApplicationOccupant>().AnyAsync();
        }
    }
}