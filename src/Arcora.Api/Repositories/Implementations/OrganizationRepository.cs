// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Repositories.Implementations
{
    public class OrganizationRepository : Repository<Organization>, IOrganizationRepository //, IDisposable
    {
        private readonly ArcoraDbContext context;
        public OrganizationRepository(ArcoraDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<bool> HasOrganizationsAsync()
        {
            return await this.context.Set<Organization>().AnyAsync();
        }
    }
}