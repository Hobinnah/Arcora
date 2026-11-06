// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;

namespace Arcora.Api.Repositories.Interfaces
{
    public interface IConversationRepository : IRepository<Conversation>
    {
        Task<List<Conversation>> GetConversationAsync();
        Task<bool> HasConversationsAsync();

        /// <summary>Returns the existing DIRECT thread for a (tenant, organization, listing) tuple, or null.</summary>
        Task<Conversation?> GetDirectThreadAsync(Guid tenantId, Guid organizationId, Guid? listingId);

        /// <summary>Creates a direct thread, or returns the matching thread if a concurrent request created it first.</summary>
        Task<Conversation> CreateDirectThreadOrGetExistingAsync(Conversation conversation);

        /// <summary>
        /// Returns paged inbox rows for a user acting as a tenant and/or as any member of an
        /// organization, newest activity first.
        /// </summary>
        Task<(List<Conversation> Items, int TotalCount)> GetInboxAsync(
            Guid? tenantId, Guid? organizationId, int pageNumber, int pageSize);
    }
}