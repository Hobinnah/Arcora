// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Repositories.Implementations
{
    public class OrganizationStatementRepository : Repository<OrganizationStatement>, IOrganizationStatementRepository //, IDisposable
    {
        private readonly ArcoraDbContext context;
        public OrganizationStatementRepository(ArcoraDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<OrganizationStatement>> GetOrganizationStatementAsync()
        {
            return await this.context.OrganizationStatements.AsNoTracking().Include(x => x.Organization).ToListAsync();
        }

        public async Task<bool> HasOrganizationStatementsAsync()
        {
            return await this.context.Set<OrganizationStatement>().AnyAsync();
        }
    }
}