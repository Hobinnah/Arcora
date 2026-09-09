// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Repositories.Implementations
{
    public class TenantEmergencyContactRepository : Repository<TenantEmergencyContact>, ITenantEmergencyContactRepository //, IDisposable
    {
        private readonly ArcoraDbContext context;
        public TenantEmergencyContactRepository(ArcoraDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<TenantEmergencyContact>> GetTenantEmergencyContactAsync()
        {
            return await ApplyDefaultOrder(this.context.TenantEmergencyContacts.AsNoTracking().Include(x => x.Tenant)).ToListAsync();
        }

        public async Task<bool> HasTenantEmergencyContactsAsync()
        {
            return await this.context.Set<TenantEmergencyContact>().AnyAsync();
        }
    }
}