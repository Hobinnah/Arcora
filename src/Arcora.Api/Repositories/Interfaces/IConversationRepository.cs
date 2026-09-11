// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;

namespace Arcora.Api.Repositories.Interfaces
{
    public interface IConversationRepository : IRepository<Conversation>
    {
        Task<List<Conversation>> GetConversationAsync();
        Task<bool> HasConversationsAsync();

        /// <summary>Returns the existing DIRECT thread for a (tenant, organization) pair, or null.</summary>
        Task<Conversation?> GetDirectThreadAsync(Guid tenantId, Guid organizationId);

        /// <summary>
        /// Returns paged inbox rows for a user acting as a tenant and/or as any member of an
        /// organization, newest activity first.
        /// </summary>
        Task<(List<Conversation> Items, int TotalCount)> GetInboxAsync(
            Guid? tenantId, Guid? organizationId, int pageNumber, int pageSize);
    }
}