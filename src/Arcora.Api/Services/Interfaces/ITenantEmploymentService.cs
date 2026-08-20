// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.DTOs;
using Arcora.Api.Models;

namespace Arcora.Api.Services.Interfaces
{
    public interface ITenantEmploymentService
    {
        /// <summary>
        /// Retrieves all tenant employments with optional paging support.
        /// </summary>
        /// <param name = "paging"></param>
        /// <returns></returns>
        Task<PagedResult<TenantEmploymentDto>> GetAll(Paging paging);
        /// <summary>
        /// Retrieves a tenant employment by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task<TenantEmploymentDto?> GetID(Guid ID);
        /// <summary>
        /// Creates a new tenant employment entry.
        /// </summary>
        /// <param name = "tenantEmploymentDto"></param>
        /// <returns></returns>
        Task<TenantEmploymentDto> CreateTenantEmployment(TenantEmploymentDto tenantEmploymentDto);
        /// <summary>
        /// Updates an existing tenant employment entry by its ID.
        /// </summary>
        /// <param name = "id"></param>
        /// <param name = "tenantemploymentDto"></param>
        /// <returns></returns>
        Task<TenantEmploymentDto?> UpdateTenantEmployment(Guid id, TenantEmploymentDto tenantemploymentDto);
        /// <summary>
        /// Deletes a tenantemployment entry by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task DeleteTenantEmployment(Guid ID);
    }
}