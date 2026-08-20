// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.DTOs;
using Arcora.Api.Models;

namespace Arcora.Api.Services.Interfaces
{
    public interface ITenantScreeningCheckService
    {
        /// <summary>
        /// Retrieves all tenant screening checks with optional paging support.
        /// </summary>
        /// <param name = "paging"></param>
        /// <returns></returns>
        Task<PagedResult<TenantScreeningCheckDto>> GetAll(Paging paging);
        /// <summary>
        /// Retrieves a tenant screening check by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task<TenantScreeningCheckDto?> GetID(Guid ID);
        /// <summary>
        /// Creates a new tenant screening check entry.
        /// </summary>
        /// <param name = "tenantScreeningCheckDto"></param>
        /// <returns></returns>
        Task<TenantScreeningCheckDto> CreateTenantScreeningCheck(TenantScreeningCheckDto tenantScreeningCheckDto);
        /// <summary>
        /// Updates an existing tenant screening check entry by its ID.
        /// </summary>
        /// <param name = "id"></param>
        /// <param name = "tenantscreeningcheckDto"></param>
        /// <returns></returns>
        Task<TenantScreeningCheckDto?> UpdateTenantScreeningCheck(Guid id, TenantScreeningCheckDto tenantscreeningcheckDto);
        /// <summary>
        /// Deletes a tenantscreeningcheck entry by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task DeleteTenantScreeningCheck(Guid ID);
        /// <summary>
        /// Updates the status of a tenantscreeningcheck by its ID.
        /// </summary>
        /// <param name = "id">The unique identifier of the tenantscreeningcheck to update.</param>
        /// <param name = "status">The new status value to assign to the tenantscreeningcheck.</param>
        /// <returns>
        /// Returns <see cref = "TenantScreeningCheckDto"/> with the updated tenantscreeningcheck if successful, or null if not found.
        /// </returns>
        Task<TenantScreeningCheckDto?> UpdateTenantScreeningCheckStatus(Guid id, string status);
    }
}