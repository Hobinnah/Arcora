// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.DTOs;
using Arcora.Api.Models;

namespace Arcora.Api.Services.Interfaces
{
    public interface ITenancyTypeService
    {
        /// <summary>
        /// Retrieves all tenancy types with optional paging support.
        /// </summary>
        /// <param name = "paging"></param>
        /// <returns></returns>
        Task<PagedResult<TenancyTypeDto>> GetAll(Paging paging);
        /// <summary>
        /// Retrieves a tenancy type by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task<TenancyTypeDto?> GetID(int ID);
        /// <summary>
        /// Creates a new tenancy type entry.
        /// </summary>
        /// <param name = "tenancyTypeDto"></param>
        /// <returns></returns>
        Task<TenancyTypeDto> CreateTenancyType(TenancyTypeDto tenancyTypeDto);
        /// <summary>
        /// Updates an existing tenancy type entry by its ID.
        /// </summary>
        /// <param name = "id"></param>
        /// <param name = "tenancytypeDto"></param>
        /// <returns></returns>
        Task<TenancyTypeDto?> UpdateTenancyType(int id, TenancyTypeDto tenancytypeDto);
        /// <summary>
        /// Deletes a tenancytype entry by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task DeleteTenancyType(int ID);
    }
}