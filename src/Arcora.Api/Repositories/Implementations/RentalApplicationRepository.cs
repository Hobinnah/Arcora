// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Repositories.Implementations
{
    public class RentalApplicationRepository : Repository<RentalApplication>, IRentalApplicationRepository //, IDisposable
    {
        private readonly ArcoraDbContext context;
        public RentalApplicationRepository(ArcoraDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<RentalApplication>> GetRentalApplicationAsync()
        {
            return await this.context.RentalApplications.AsNoTracking().Include(x => x.Listing).Include(x => x.Tenant).Include(x => x.ReviewedByOrganizationMember) // FK to OrganizationMember who reviewed
            .ToListAsync();
        }

        public async Task<bool> HasRentalApplicationsAsync()
        {
            return await this.context.Set<RentalApplication>().AnyAsync();
        }
    }
}