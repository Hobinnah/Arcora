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

        public async Task<Conversation?> GetDirectThreadAsync(Guid tenantId, Guid organizationId, Guid? listingId)
        {
            return await this.context.Conversations
                .Where(c => c.ConversationType == "DIRECT"
                            && c.TenantID == tenantId
                            && c.OrganizationID == organizationId
                            && c.ListingID == listingId)
                .OrderByDescending(c => c.LastMessageAt)
                .FirstOrDefaultAsync();
        }

        public async Task<Conversation> CreateDirectThreadOrGetExistingAsync(Conversation conversation)
        {
            await this.context.Conversations.AddAsync(conversation);
            try
            {
                await this.context.SaveChangesAsync();
                return conversation;
            }
            catch (DbUpdateException)
            {
                this.context.Entry(conversation).State = EntityState.Detached;
                var existing = await GetDirectThreadAsync(
                    conversation.TenantID!.Value, conversation.OrganizationID!.Value, conversation.ListingID);
                if (existing != null)
                    return existing;
                throw;
            }
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