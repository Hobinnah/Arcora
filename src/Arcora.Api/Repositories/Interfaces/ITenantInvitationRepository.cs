// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;

namespace Arcora.Api.Repositories.Interfaces
{
    public interface ITenantInvitationRepository : IRepository<TenantInvitation>
    {
        Task<List<TenantInvitation>> GetTenantInvitationAsync();
        Task<bool> HasTenantInvitationsAsync();
    }
}