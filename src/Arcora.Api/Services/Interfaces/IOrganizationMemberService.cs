// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.DTOs;
using Arcora.Api.Models;

namespace Arcora.Api.Services.Interfaces
{
    public interface IOrganizationMemberService
    {
        /// <summary>
        /// Retrieves all organization members with optional paging support.
        /// </summary>
        /// <param name = "paging"></param>
        /// <returns></returns>
        Task<PagedResult<OrganizationMemberDto>> GetAll(Paging paging);
        /// <summary>
        /// Retrieves a organization member by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task<OrganizationMemberDto?> GetID(Guid ID);
        /// <summary>
        /// Creates a new organization member entry.
        /// </summary>
        /// <param name = "organizationMemberDto"></param>
        /// <returns></returns>
        Task<OrganizationMemberDto> CreateOrganizationMember(OrganizationMemberDto organizationMemberDto);

        /// <summary>
        ///  
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        Task<IEnumerable<OrganizationMemberDto>?> GetMemberOrganizationsAsync(long userID);
        /// <summary>
        /// Updates an existing organization member entry by its ID.
        /// </summary>
        /// <param name = "id"></param>
        /// <param name = "organizationmemberDto"></param>
        /// <returns></returns>
        Task<OrganizationMemberDto?> UpdateOrganizationMember(Guid id, OrganizationMemberDto organizationmemberDto);
        /// <summary>
        /// Deletes a organizationmember entry by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task DeleteOrganizationMember(Guid ID);
        /// <summary>
        /// Updates the status of a organizationmember by its ID.
        /// </summary>
        /// <param name = "id">The unique identifier of the organizationmember to update.</param>
        /// <param name = "status">The new status value to assign to the organizationmember.</param>
        /// <returns>
        /// Returns <see cref = "OrganizationMemberDto"/> with the updated organizationmember if successful, or null if not found.
        /// </returns>
        Task<OrganizationMemberDto?> UpdateOrganizationMemberStatus(Guid id, string status);
    }
}