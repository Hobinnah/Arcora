// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;
using Arcora.Api.Models;
using Arcora.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Repositories.Implementations
{
    public class LeaseRepository : Repository<Lease>, ILeaseRepository //, IDisposable
    {
        private readonly ArcoraDbContext context;
        public LeaseRepository(ArcoraDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<Lease>> GetLeaseAsync()
        {
            return await ApplyDefaultOrder(this.context.Leases.AsNoTracking().Include(x => x.Organization).Include(x => x.Listing).Include(x => x.RentalUnit).Include(x => x.TenancyType).Include(x => x.Tenant).Include(x => x.RentalApplication)).ToListAsync();
        }

        public async Task<(List<Lease> Items, int TotalCount)> GetLeasePagedAsync(Paging paging)
        {
            paging ??= new Paging();

            IQueryable<Lease> query = this.context.Leases.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(paging.Search))
            {
                query = query.Where(x => x.LeaseNumber != null && x.LeaseNumber.Contains(paging.Search));
            }

            var totalCount = await query.CountAsync();

            var items = await ApplyDefaultOrder(query)
                .Skip((paging.PageNumber - 1) * paging.PageSize)
                .Take(paging.PageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<Lease?> GetLeaseByIDAsync(Guid Id)
        {
            return await this.context.Leases
                .AsNoTracking()
                .Include(x => x.Organization)
                .Include(x => x.Listing)
                .ThenInclude(x => x!.ListingPhotos)
                .Include(x => x.RentalUnit)
                .Include(x => x.TenancyType)
                .Include(x => x.Tenant)
                .Include(x => x.RentalApplication)
                .FirstOrDefaultAsync(x => x.LeaseID == Id);
        }

        public async Task<Lease?> GetLeaseWithContractContextByIDAsync(Guid Id)
        {
            return await this.context.Leases
                .AsNoTracking()
                .Include(x => x.Organization)
                .Include(x => x.Listing)
                .Include(x => x.RentalUnit)
                    .ThenInclude(x => x!.Property)
                        .ThenInclude(x => x!.Address)
                .Include(x => x.Tenant)
                    .ThenInclude(x => x!.User)
                .Include(x => x.TenancyType)
                .FirstOrDefaultAsync(x => x.LeaseID == Id);
        }

        public async Task<bool> HasLeasesAsync()
        {
            return await this.context.Set<Lease>().AnyAsync();
        }
    }
}