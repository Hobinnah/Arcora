// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;

namespace Arcora.Api.Repositories.Interfaces
{
    public interface IOrganizationMemberRepository : IRepository<OrganizationMember>
    {
        Task<List<OrganizationMember>> GetOrganizationMemberAsync();
        Task<bool> HasOrganizationMembersAsync();
    }
}