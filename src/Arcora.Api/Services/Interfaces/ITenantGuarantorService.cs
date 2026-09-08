// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.DTOs;
using Arcora.Api.Models;

namespace Arcora.Api.Services.Interfaces
{
    public interface ITenantGuarantorService
    {
        /// <summary>
        /// Retrieves all tenant guarantors with optional paging support.
        /// </summary>
        /// <param name = "paging"></param>
        /// <returns></returns>
        Task<PagedResult<TenantGuarantorDto>> GetAll(Paging paging);
        /// <summary>
        /// Retrieves a tenant guarantor by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task<TenantGuarantorDto?> GetID(Guid ID);
        /// <summary>
        /// Creates a new tenant guarantor entry.
        /// </summary>
        /// <param name = "tenantGuarantorDto"></param>
        /// <returns></returns>
        Task<TenantGuarantorDto> CreateTenantGuarantor(TenantGuarantorDto tenantGuarantorDto);
        /// <summary>
        /// Updates an existing tenant guarantor entry by its ID.
        /// </summary>
        /// <param name = "id"></param>
        /// <param name = "tenantguarantorDto"></param>
        /// <returns></returns>
        Task<TenantGuarantorDto?> UpdateTenantGuarantor(Guid id, TenantGuarantorDto tenantguarantorDto);
        /// <summary>
        /// Deletes a tenantguarantor entry by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task DeleteTenantGuarantor(Guid ID);
        /// <summary>
        /// Updates the status of a tenantguarantor by its ID.
        /// </summary>
        /// <param name = "id">The unique identifier of the tenantguarantor to update.</param>
        /// <param name = "status">The new status value to assign to the tenantguarantor.</param>
        /// <returns>
        /// Returns <see cref = "TenantGuarantorDto"/> with the updated tenantguarantor if successful, or null if not found.
        /// </returns>
        Task<TenantGuarantorDto?> UpdateTenantGuarantorStatus(Guid id, string status);

        /// <summary>
        /// Processes a guarantor's response to an invitation link (accept or decline) and updates the
        /// guarantor record accordingly. The guarantor is identified from the signed, self-expiring token
        /// embedded in the invitation link.
        /// </summary>
        /// <param name="response">The response, either "ACCEPT" or "DECLINE".</param>
        /// <param name="token">The signed token from the invitation link (identifies the guarantor and carries expiry).</param>
        /// <returns>The updated <see cref="TenantGuarantorDto"/>, or null if not found or the token is invalid/expired.</returns>
        Task<TenantGuarantorDto?> RespondToInvitationAsync(string response, string token);

        /// <summary>
        /// Resolves the display details for a guarantor invitation from its signed, self-expiring token.
        /// Used by the public /guarantor-invite/:token landing page to render before the guarantor responds.
        /// </summary>
        /// <param name="token">The signed token from the invitation link.</param>
        /// <returns>The invite details, or null if the token is invalid/expired or the guarantor is missing.</returns>
        Task<GuarantorInviteDetailsDto?> GetInviteDetailsAsync(string token);

        /// <summary>
        /// Links any of the tenant's pending guarantors (that don't yet reference an application) to the
        /// newly created rental application and sends each of them the invitation email containing the
        /// tenant, listing, rent and deposit details.
        /// </summary>
        /// <param name="rentalApplicationId">The id of the saved rental application.</param>
        /// <param name="tenantId">The tenant that owns the application and the guarantors.</param>
        /// <param name="listingId">The listing the application is for.</param>
        Task NotifyGuarantorsForApplicationAsync(Guid rentalApplicationId, Guid tenantId, Guid listingId);
    }
}