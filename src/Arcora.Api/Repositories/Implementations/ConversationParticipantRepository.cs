// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Arcora.Api.Repositories.Implementations
{
    public class ConversationParticipantRepository : Repository<ConversationParticipant>, IConversationParticipantRepository //, IDisposable
    {
        private readonly ArcoraDbContext context;
        public ConversationParticipantRepository(ArcoraDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<ConversationParticipant>> GetConversationParticipantAsync()
        {
            return await ApplyDefaultOrder(this.context.ConversationParticipants.AsNoTracking().Include(x => x.Conversation).Include(x => x.User).Include(x => x.Tenant).Include(x => x.OrganizationMember)).ToListAsync();
        }

        public async Task<bool> HasConversationParticipantsAsync()
        {
            return await this.context.Set<ConversationParticipant>().AnyAsync();
        }
    }
}