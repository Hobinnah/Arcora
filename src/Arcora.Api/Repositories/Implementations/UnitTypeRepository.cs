// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Repositories.Implementations
{
    public class UnitTypeRepository : Repository<UnitType>, IUnitTypeRepository //, IDisposable
    {
        private readonly ArcoraDbContext context;
        public UnitTypeRepository(ArcoraDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<bool> HasUnitTypesAsync()
        {
            return await this.context.Set<UnitType>().AnyAsync();
        }
    }
}