// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.DTOs;
using Arcora.Api.Models;

namespace Arcora.Api.Services.Interfaces
{
    public interface ITenantService
    {
        /// <summary>
        /// Retrieves all tenants with optional paging support.
        /// </summary>
        /// <param name = "paging"></param>
        /// <returns></returns>
        Task<PagedResult<TenantDto>> GetAll(Paging paging);
        /// <summary>
        /// Retrieves a tenant by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task<TenantDto?> GetID(Guid ID);

        /// <summary>
        ///  
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        Task<TenantDto?> GetTenantByUserID(long userID);

        /// <summary>
        /// Creates a new tenant entry.
        /// </summary>
        /// <param name = "tenantDto"></param>
        /// <returns></returns>
        Task<TenantDto> CreateTenant(TenantDto tenantDto);
        /// <summary>
        /// Updates an existing tenant entry by its ID.
        /// </summary>
        /// <param name = "id"></param>
        /// <param name = "tenantDto"></param>
        /// <returns></returns>
        Task<TenantDto?> UpdateTenant(Guid id, TenantDto tenantDto);
        /// <summary>
        /// Deletes a tenant entry by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task DeleteTenant(Guid ID);
    }
}