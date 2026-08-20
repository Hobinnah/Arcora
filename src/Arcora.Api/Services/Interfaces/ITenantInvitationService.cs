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
    }
}