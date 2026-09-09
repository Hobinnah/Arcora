// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Repositories.Implementations
{
    public class TenantInvitationRepository : Repository<TenantInvitation>, ITenantInvitationRepository //, IDisposable
    {
        private readonly ArcoraDbContext context;
        public TenantInvitationRepository(ArcoraDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<TenantInvitation>> GetTenantInvitationAsync()
        {
            return await ApplyDefaultOrder(this.context.TenantInvitations.AsNoTracking().Include(x => x.Lease).Include(x => x.RentalApplication)).ToListAsync();
        }

        public async Task<bool> HasTenantInvitationsAsync()
        {
            return await this.context.Set<TenantInvitation>().AnyAsync();
        }
    }
}