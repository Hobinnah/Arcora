// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Repositories.Implementations
{
    public class RentalUnitRepository : Repository<RentalUnit>, IRentalUnitRepository //, IDisposable
    {
        private readonly ArcoraDbContext context;
        public RentalUnitRepository(ArcoraDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<RentalUnit>> GetRentalUnitAsync()
        {
            return await this.context.RentalUnits.AsNoTracking().Include(x => x.Property).Include(x => x.UnitType).ToListAsync();
        }

        public async Task<bool> HasRentalUnitsAsync()
        {
            return await this.context.Set<RentalUnit>().AnyAsync();
        }
    }
}