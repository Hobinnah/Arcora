// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Repositories.Implementations
{
    public class InspectionRepository : Repository<Inspection>, IInspectionRepository //, IDisposable
    {
        private readonly ArcoraDbContext context;
        public InspectionRepository(ArcoraDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<Inspection>> GetInspectionAsync()
        {
            return await ApplyDefaultOrder(this.context.Inspections.AsNoTracking().Include(x => x.Property).Include(x => x.RentalUnit).Include(x => x.Lease)).ToListAsync();
        }

        public async Task<bool> HasInspectionsAsync()
        {
            return await this.context.Set<Inspection>().AnyAsync();
        }
    }
}