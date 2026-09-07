// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;

namespace Arcora.Api.Repositories.Interfaces
{
    public interface IOrganizationMemberRepository : IRepository<OrganizationMember>
    {
        /// <summary>
        ///   
        /// </summary>
        /// <returns></returns>
        Task<List<OrganizationMember>> GetOrganizationMembersAsync();

        /// <summary>
        ///  
        /// </summary>
        /// <param name="organisationID"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        Task<List<OrganizationMember>> GetMemberOrganizationsAsync(long userID);

        Task<bool> HasOrganizationMembersAsync();
    }
}