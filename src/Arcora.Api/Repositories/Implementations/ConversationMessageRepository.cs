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

        public async Task<Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction> BeginAttachmentSendAsync(
            Guid conversationID, CancellationToken cancellationToken = default)
        {
            var transaction = await context.Database.BeginTransactionAsync(
                context.Database.IsSqlServer() ? System.Data.IsolationLevel.ReadCommitted : System.Data.IsolationLevel.Serializable,
                cancellationToken);
            try
            {
                if (context.Database.IsSqlServer())
                {
                    // A database-owned lock serializes uploads across API instances, before any blob is written.
                    // Serialize the thread's attachment transactions as they also update its last-message row.
                    var resource = $"attachment:{conversationID:N}";
                    await context.Database.ExecuteSqlInterpolatedAsync($@"
DECLARE @result int;
EXEC @result = sys.sp_getapplock @Resource={resource}, @LockMode='Exclusive',
    @LockOwner='Transaction', @LockTimeout=60000;
IF @result < 0 THROW 51000, 'Could not acquire attachment send lock. Retry with the same key.', 1;",
                        cancellationToken);
                }
                return transaction;
            }
            catch
            {
                await transaction.DisposeAsync();
                throw;
            }
        }

        public Task<ConversationMessage?> GetAttachmentSendAsync(Guid conversationID, string senderScope, Guid sendKey) =>
            context.ConversationMessages.AsNoTracking().SingleOrDefaultAsync(x =>
                x.ConversationID == conversationID && x.AttachmentSenderScope == senderScope && x.AttachmentSendKey == sendKey);

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
            Guid conversationId, int pageNumber, int pageSize, DateTime? before = null)
        {
            var query = this.context.ConversationMessages.AsNoTracking()
                .Include(x => x.SenderUser)
                .Include(x => x.SenderTenant)!.ThenInclude(t => t!.User)
                .Include(x => x.SenderOrganizationMember)!.ThenInclude(m => m!.User)
                .Where(x => x.ConversationID == conversationId && x.DeletedAt == null);
            if (before.HasValue)
                query = query.Where(x => x.SentAt < before.Value);

            var total = await query.CountAsync();

            // Newest first so page 1 is what the reader sees on open; flipped back to reading order below.
            var items = await query
                .OrderByDescending(x => x.SentAt)
                .ThenByDescending(x => x.CapturedDate)
                .Skip((Math.Max(pageNumber, 1) - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            items.Reverse();

            return (items, total);
        }

        public async Task<ConversationMessage?> GetLatestAsync(Guid conversationId)
        {
            return await this.context.ConversationMessages.AsNoTracking()
                .Where(x => x.ConversationID == conversationId && x.DeletedAt == null)
                .OrderByDescending(x => x.SentAt)
                .FirstOrDefaultAsync();
        }

        public async Task<int> CountUnreadAsync(Guid conversationId, DateTime? lastReadAt, bool fromOrganization)
        {
            var since = lastReadAt ?? DateTime.MinValue;
            return await this.context.ConversationMessages.AsNoTracking()
                .CountAsync(x => x.ConversationID == conversationId
                                 && x.DeletedAt == null
                                 && x.SentAt > since
                                 && (x.SenderOrganizationMemberID != null) == fromOrganization);
        }
    }
}