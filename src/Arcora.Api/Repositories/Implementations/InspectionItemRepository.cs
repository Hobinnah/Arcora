// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Repositories.Implementations
{
    public class InspectionItemRepository : Repository<InspectionItem>, IInspectionItemRepository //, IDisposable
    {
        private readonly ArcoraDbContext context;
        public InspectionItemRepository(ArcoraDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<InspectionItem>> GetInspectionItemAsync()
        {
            return await this.context.InspectionItems.AsNoTracking().Include(x => x.Inspection).ToListAsync();
        }

        public async Task<bool> HasInspectionItemsAsync()
        {
            return await this.context.Set<InspectionItem>().AnyAsync();
        }
    }
}