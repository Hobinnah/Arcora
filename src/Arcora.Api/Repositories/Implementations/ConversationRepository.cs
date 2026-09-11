// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Repositories.Implementations
{
    public class ConversationRepository : Repository<Conversation>, IConversationRepository //, IDisposable
    {
        private readonly ArcoraDbContext context;
        public ConversationRepository(ArcoraDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<Conversation>> GetConversationAsync()
        {
            return await ApplyDefaultOrder(this.context.Conversations.AsNoTracking().Include(x => x.Lease).Include(x => x.MaintenanceRequest).Include(x => x.Dispute)).ToListAsync();
        }

        public async Task<bool> HasConversationsAsync()
        {
            return await this.context.Set<Conversation>().AnyAsync();
        }

        public async Task<Conversation?> GetDirectThreadAsync(Guid tenantId, Guid organizationId)
        {
            return await this.context.Conversations
                .Where(c => c.ConversationType == "DIRECT"
                            && c.TenantID == tenantId
                            && c.OrganizationID == organizationId)
                .OrderByDescending(c => c.LastMessageAt)
                .FirstOrDefaultAsync();
        }

        public async Task<(List<Conversation> Items, int TotalCount)> GetInboxAsync(
            Guid? tenantId, Guid? organizationId, int pageNumber, int pageSize)
        {
            var query = this.context.Conversations.AsNoTracking()
                .Include(c => c.Tenant)!.ThenInclude(t => t!.User)
                .Include(c => c.Organization)
                .Where(c =>
                    (tenantId != null && c.TenantID == tenantId) ||
                    (organizationId != null && c.OrganizationID == organizationId));

            var total = await query.CountAsync();

            var items = await query
                .OrderByDescending(c => c.LastMessageAt ?? c.CapturedDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }
    }
}