// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Repositories.Implementations
{
    public class OrganizationMemberRepository : Repository<OrganizationMember>, IOrganizationMemberRepository //, IDisposable
    {
        private readonly ArcoraDbContext context;
        public OrganizationMemberRepository(ArcoraDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<OrganizationMember>> GetOrganizationMemberAsync()
        {
            return await this.context.OrganizationMembers.AsNoTracking().Include(x => x.Organization).Include(x => x.User).ToListAsync();
        }

        public async Task<bool> HasOrganizationMembersAsync()
        {
            return await this.context.Set<OrganizationMember>().AnyAsync();
        }
    }
}