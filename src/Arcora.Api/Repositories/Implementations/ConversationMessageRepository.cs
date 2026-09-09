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
    }
}