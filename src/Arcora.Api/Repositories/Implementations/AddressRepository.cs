// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Repositories.Implementations
{
    public class AddressRepository : Repository<Address>, IAddressRepository //, IDisposable
    {
        private readonly ArcoraDbContext context;
        public AddressRepository(ArcoraDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<Address>> GetAddressAsync()
        {
            return await ApplyDefaultOrder(this.context.Addresses.AsNoTracking().Include(x => x.Organization)).ToListAsync();
        }

        public async Task<bool> HasAddressesAsync()
        {
            return await this.context.Set<Address>().AnyAsync();
        }
    }
}