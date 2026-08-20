// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Repositories.Implementations
{
    public class LeaseDocExtractedTermRepository : Repository<LeaseDocExtractedTerm>, ILeaseDocExtractedTermRepository //, IDisposable
    {
        private readonly ArcoraDbContext context;
        public LeaseDocExtractedTermRepository(ArcoraDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<bool> HasLeaseDocExtractedTermsAsync()
        {
            return await this.context.Set<LeaseDocExtractedTerm>().AnyAsync();
        }
    }
}