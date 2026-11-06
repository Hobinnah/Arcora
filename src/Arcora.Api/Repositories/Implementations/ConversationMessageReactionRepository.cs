using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Repositories.Implementations
{
    public class ConversationMessageReactionRepository : Repository<ConversationMessageReaction>, IConversationMessageReactionRepository
    {
        private readonly ArcoraDbContext context;

        public ConversationMessageReactionRepository(ArcoraDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<ConversationMessageReaction>> GetForMessagesAsync(IReadOnlyCollection<Guid> messageIds)
        {
            if (messageIds.Count == 0)
                return new List<ConversationMessageReaction>();

            return await context.ConversationMessageReactions.AsNoTracking()
                .Where(x => messageIds.Contains(x.ConversationMessageID))
                .OrderBy(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<ConversationMessageReaction>> ToggleAsync(Guid messageId, string emoji, Guid? tenantId, Guid? organizationMemberId)
        {
            var existing = await context.ConversationMessageReactions.FirstOrDefaultAsync(x =>
                x.ConversationMessageID == messageId &&
                (tenantId != null ? x.ReactorTenantID == tenantId : x.ReactorOrganizationMemberID == organizationMemberId));

            if (existing == null)
            {
                context.ConversationMessageReactions.Add(new ConversationMessageReaction
                {
                    ConversationMessageReactionID = Guid.NewGuid(),
                    ConversationMessageID = messageId,
                    ReactorTenantID = tenantId,
                    ReactorOrganizationMemberID = tenantId != null ? null : organizationMemberId,
                    Emoji = emoji,
                    CreatedAt = DateTime.UtcNow
                });
            }
            else if (existing.Emoji == emoji)
            {
                context.ConversationMessageReactions.Remove(existing);
            }
            else
            {
                existing.Emoji = emoji;
                existing.CreatedAt = DateTime.UtcNow;
            }

            await context.SaveChangesAsync();
            return await GetForMessagesAsync(new[] { messageId });
        }
    }
}
