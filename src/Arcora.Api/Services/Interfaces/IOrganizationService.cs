// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.DTOs;
using Arcora.Api.Models;

namespace Arcora.Api.Services.Interfaces
{
    public interface IOrganizationService
    {
        /// <summary>
        /// Retrieves all organizations with optional paging support.
        /// </summary>
        /// <param name = "paging"></param>
        /// <returns></returns>
        Task<PagedResult<OrganizationDto>> GetAll(Paging paging);
        /// <summary>
        /// Retrieves a organization by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task<OrganizationDto?> GetID(Guid ID);
        /// <summary>
        /// Creates a new organization entry.
        /// </summary>
        /// <param name = "organizationDto"></param>
        /// <returns></returns>
        Task<OrganizationDto> CreateOrganization(OrganizationDto organizationDto);
        /// <summary>
        /// Updates an existing organization entry by its ID.
        /// </summary>
        /// <param name = "id"></param>
        /// <param name = "organizationDto"></param>
        /// <returns></returns>
        Task<OrganizationDto?> UpdateOrganization(Guid id, OrganizationDto organizationDto);
        /// <summary>
        /// Deletes a organization entry by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task DeleteOrganization(Guid ID);
        /// <summary>
        /// Updates the status of a organization by its ID.
        /// </summary>
        /// <param name = "id">The unique identifier of the organization to update.</param>
        /// <param name = "status">The new status value to assign to the organization.</param>
        /// <returns>
        /// Returns <see cref = "OrganizationDto"/> with the updated organization if successful, or null if not found.
        /// </returns>
        Task<OrganizationDto?> UpdateOrganizationStatus(Guid id, string status);
    }
}