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
        /// Asynchronously retrieves an organization member by the specified organization ID.
        /// </summary>
        /// <param name="organizationID">The unique identifier of the organization.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an OrganizationMemberDto if
        /// found; otherwise, null.</returns>
        Task<List<OrganizationMemberDto>?> GetOrganizationMemberByOrgID(Guid organizationID);

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

        /// <summary>
        /// Sends a signed cohost invitation email for an organization with a time-limited token.
        /// </summary>
        /// <param name="cohostInvitationDto">Invitation payload including organization, invitee and requested access level.</param>
        /// <returns>The invitation result including token and expiry metadata.</returns>
        Task<CohostInvitationResultDto> InviteCohostAsync(CohostInvitationDto cohostInvitationDto, long invitedByUserID);

        /// <summary>
        /// Returns pending cohost invitation records for the specified organization.
        /// Pending excludes accepted invites and only returns actionable invitations.
        /// </summary>
        Task<List<CohostInvitationRecordDto>> GetCohostInvitationsByOrganizationAsync(Guid organizationID);

        /// <summary>
        /// Returns all cohost invitation records for the specified organization.
        /// </summary>
        Task<List<CohostInvitationRecordDto>> GetAllCohostInvitationsByOrganizationAsync(Guid organizationID);

        /// <summary>
        /// Resolves a cohost invitation from token for the invite landing flow.
        /// </summary>
        Task<CohostInvitationRecordDto?> GetCohostInviteDetailsAsync(string token);

        /// <summary>
        /// Applies an invitee response (ACCEPT or DECLINE) for a cohost invitation token.
        /// </summary>
        Task<CohostInvitationRecordDto?> RespondToCohostInvitationAsync(string token, string response, long userID);

        /// <summary>
        /// Revokes a pending cohost invitation.
        /// </summary>
        /// <param name="cohostInvitationID">The invitation identifier.</param>
        /// <param name="updatedByUserID">The authenticated user performing the revoke operation.</param>
        Task<CohostInvitationRecordDto?> RevokeCohostInvitationAsync(Guid cohostInvitationID, long updatedByUserID);

        /// <summary>
        /// Reactivates a previously revoked cohost invitation.
        /// </summary>
        /// <param name="cohostInvitationID">The invitation identifier.</param>
        /// <param name="updatedByUserID">The authenticated user performing the reactivation operation.</param>
        Task<CohostInvitationRecordDto?> ReactivateRevokedCohostAsync(Guid cohostInvitationID, long updatedByUserID);
    }
}