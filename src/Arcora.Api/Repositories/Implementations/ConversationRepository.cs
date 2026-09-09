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
    }
}