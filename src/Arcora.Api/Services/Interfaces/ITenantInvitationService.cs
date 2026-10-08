// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.DTOs;
using Arcora.Api.Models;

namespace Arcora.Api.Services.Interfaces
{
    public interface ITenantInvitationService
    {
        /// <summary>
        /// Retrieves all tenant invitations with optional paging support.
        /// </summary>
        /// <param name = "paging"></param>
        /// <returns></returns>
        Task<PagedResult<TenantInvitationDto>> GetAll(Paging paging);
        Task<List<TenantInvitationDto>> GetByOrganizationAsync(Guid organizationID);
        /// <summary>
        /// Retrieves a tenant invitation by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task<TenantInvitationDto?> GetID(Guid ID);
        /// <summary>
        /// Creates a new tenant invitation entry.
        /// </summary>
        /// <param name = "tenantInvitationDto"></param>
        /// <returns></returns>
        Task<TenantInvitationDto> CreateTenantInvitation(TenantInvitationDto tenantInvitationDto);
        /// <summary>
        /// Updates an existing tenant invitation entry by its ID.
        /// </summary>
        /// <param name = "id"></param>
        /// <param name = "tenantinvitationDto"></param>
        /// <returns></returns>
        Task<TenantInvitationDto?> UpdateTenantInvitation(Guid id, TenantInvitationDto tenantinvitationDto);
        /// <summary>
        /// Deletes a tenantinvitation entry by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task DeleteTenantInvitation(Guid ID);
        /// <summary>
        /// Updates the status of a tenantinvitation by its ID.
        /// </summary>
        /// <param name = "id">The unique identifier of the tenantinvitation to update.</param>
        /// <param name = "status">The new status value to assign to the tenantinvitation.</param>
        /// <returns>
        /// Returns <see cref = "TenantInvitationDto"/> with the updated tenantinvitation if successful, or null if not found.
        /// </returns>
        Task<TenantInvitationDto?> UpdateTenantInvitationStatus(Guid id, string status);

        /// <summary>
        /// Rechecks listing availability and, when available, creates the tenant invitation and
        /// reservation hold in one transaction.
        /// </summary>
        Task<CreateTenantInvitationResultDto> CreateTenantInvitationWithHold(CreateTenantInvitationRequestDto request, long signedInUserID);
        Task<TenantInvitationDto?> RetryInvitationEmailAsync(Guid id);
        Task<TenantInvitationDto> GetAcceptedInvitationForUserAsync(Guid id, long userID);

        /// <summary>
        /// Creates or returns a lease from an accepted tenant invitation for the signed-in tenant.
        /// </summary>
        /// <param name="tenantInvitationID">Tenant invitation identifier.</param>
        /// <param name="tenantID">Tenant identifier.</param>
        /// <param name="signedInUserId">Identifier of the signed-in user.</param>
        /// <param name="signedInEmail">Email of the signed-in user.</param>
        /// <param name="capturedBy">Optional. Identifier for tracking who captured the request.</param>
        /// <returns>
        /// The created or existing lease mapped to <see cref="LeaseDto"/>, or null if invitation was not found.
        /// </returns>
        Task<CreateLeaseFromTenantInvitationResultDto?> CreateLeaseFromTenantInvitationAsync(Guid tenantInvitationID, Guid tenantID, long signedInUserId, string signedInEmail, string? capturedBy);

        /// <summary>
        /// Responds to a tenant invitation using a signed invitation token.
        /// </summary>
        Task<TenantInvitationTokenResponseDto> RespondToTenantInvitationAsync(string token, string response);

        /// <summary>
        /// Gets all accepted invitations for a signed-in tenant that don't have a lease yet.
        /// </summary>
        /// <param name="tenantId">The ID of the signed-in tenant.</param>
        /// <returns>A list of accepted invitations without leases for the tenant.</returns>
        Task<List<TenantInvitationDto>> GetPendingInvitationsForTenantAsync(Guid tenantId);
    }
}