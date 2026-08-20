// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Repositories.Implementations
{
    public class AuditLogRepository : Repository<AuditLog>, IAuditLogRepository //, IDisposable
    {
        private readonly ArcoraDbContext context;
        public AuditLogRepository(ArcoraDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<AuditLog>> GetAuditLogAsync()
        {
            return await this.context.AuditLogs.AsNoTracking().Include(x => x.ActorUser) // Navigation property for User entity.
            .Include(x => x.Tenant).Include(x => x.OrganizationMember).Include(x => x.Organization).ToListAsync();
        }

        public async Task<bool> HasAuditLogsAsync()
        {
            return await this.context.Set<AuditLog>().AnyAsync();
        }
    }
}