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

        /// <summary>
        /// Asynchronously retrieves all members associated with the specified organization.
        /// </summary>
        /// <param name="organizationID">The unique identifier of the organization.</param>
        /// <returns>A task representing the asynchronous operation, containing a list of organization members.</returns>
        Task<List<OrganizationMember>> GetOrganizationMembersByOrgIDAsync(Guid organizationID);

        /// <summary>
        /// Checks if an organization has registered members  
        /// </summary>
        /// <returns></returns>
        Task<bool> HasOrganizationMembersAsync();
    }
}