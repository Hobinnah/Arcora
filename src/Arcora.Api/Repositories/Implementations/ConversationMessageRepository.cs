// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Repositories.Implementations
{
    public class ConversationMessageRepository : Repository<ConversationMessage>, IConversationMessageRepository //, IDisposable
    {
        private readonly ArcoraDbContext context;
        public ConversationMessageRepository(ArcoraDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<ConversationMessage>> GetConversationMessageAsync()
        {
            return await ApplyDefaultOrder(this.context.ConversationMessages.AsNoTracking().Include(x => x.Conversation).Include(x => x.SenderUser) // FK to User
            .Include(x => x.SenderTenant) // FK to Tenant
            .Include(x => x.SenderOrganizationMember) // FK to OrganizationMember
            .Include(x => x.ReplyToMessageConversationMessage) // FK to ConversationMessage (reply)
            ).ToListAsync();
        }

        public async Task<bool> HasConversationMessagesAsync()
        {
            return await this.context.Set<ConversationMessage>().AnyAsync();
        }

        public async Task<(List<ConversationMessage> Items, int TotalCount)> GetThreadMessagesAsync(
            Guid conversationId, int pageNumber, int pageSize)
        {
            var query = this.context.ConversationMessages.AsNoTracking()
                .Include(x => x.SenderUser)
                .Include(x => x.SenderTenant)!.ThenInclude(t => t!.User)
                .Include(x => x.SenderOrganizationMember)!.ThenInclude(m => m!.User)
                .Where(x => x.ConversationID == conversationId && x.DeletedAt == null);

            var total = await query.CountAsync();

            var items = await query
                .OrderBy(x => x.SentAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }

        public async Task<ConversationMessage?> GetLatestAsync(Guid conversationId)
        {
            return await this.context.ConversationMessages.AsNoTracking()
                .Where(x => x.ConversationID == conversationId && x.DeletedAt == null)
                .OrderByDescending(x => x.SentAt)
                .FirstOrDefaultAsync();
        }

        public async Task<int> CountUnreadAsync(Guid conversationId, DateTime? lastReadAt, long? excludeSenderUserId)
        {
            var since = lastReadAt ?? DateTime.MinValue;
            return await this.context.ConversationMessages.AsNoTracking()
                .CountAsync(x => x.ConversationID == conversationId
                                 && x.DeletedAt == null
                                 && x.SentAt > since
                                 && (excludeSenderUserId == null || x.SenderUserID != excludeSenderUserId));
        }
    }
}