// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Repositories.Implementations
{
    public class FeeTypeRepository : Repository<FeeType>, IFeeTypeRepository //, IDisposable
    {
        private readonly ArcoraDbContext context;
        public FeeTypeRepository(ArcoraDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<bool> HasFeeTypesAsync()
        {
            return await this.context.Set<FeeType>().AnyAsync();
        }
    }
}