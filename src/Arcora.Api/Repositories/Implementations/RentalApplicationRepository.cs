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
            return await ApplyDefaultOrder(this.context.RentalApplications.AsNoTracking()
                .Include(x => x.Listing)!.ThenInclude(l => l!.ListingPhotos)
                .Include(x => x.Tenant)!.ThenInclude(t => t!.User)
                .Include(x => x.Organization)
                .Include(x => x.ReviewedByOrganizationMember) // FK to OrganizationMember who reviewed
                .Include(x => x.ApplicationOccupants)
                .Include(x => x.LeaseDocuments)
                .Include(x => x.TenantGuarantors)
                .Include(x => x.TenantEmergencyContacts)
                .Include(x => x.TenantEmployments)
                .Include(x => x.TenantScreeningChecks)
                .Include(x => x.TenantInvitations)
                .Include(x => x.ReservationHolds)
                .Include(x => x.ViewingAppointments)
                .Include(x => x.CalendarEvents)
                .Include(x => x.Leases)
                .AsSplitQuery()
            ).ToListAsync();
        }

        public async Task<bool> HasRentalApplicationsAsync()
        {
            return await this.context.Set<RentalApplication>().AnyAsync();
        }
    }
}