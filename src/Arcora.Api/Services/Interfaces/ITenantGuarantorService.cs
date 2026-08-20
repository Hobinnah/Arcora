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
    }
}