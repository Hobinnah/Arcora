// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.DTOs;
using Arcora.Api.Models;

namespace Arcora.Api.Services.Interfaces
{
    public interface IOrganizationStatementService
    {
        /// <summary>
        /// Retrieves all organization statements with optional paging support.
        /// </summary>
        /// <param name = "paging"></param>
        /// <returns></returns>
        Task<PagedResult<OrganizationStatementDto>> GetAll(Paging paging);
        /// <summary>
        /// Retrieves a organization statement by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task<OrganizationStatementDto?> GetID(Guid ID);
        /// <summary>
        /// Creates a new organization statement entry.
        /// </summary>
        /// <param name = "organizationStatementDto"></param>
        /// <returns></returns>
        Task<OrganizationStatementDto> CreateOrganizationStatement(OrganizationStatementDto organizationStatementDto);
        /// <summary>
        /// Updates an existing organization statement entry by its ID.
        /// </summary>
        /// <param name = "id"></param>
        /// <param name = "organizationstatementDto"></param>
        /// <returns></returns>
        Task<OrganizationStatementDto?> UpdateOrganizationStatement(Guid id, OrganizationStatementDto organizationstatementDto);
        /// <summary>
        /// Deletes a organizationstatement entry by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task DeleteOrganizationStatement(Guid ID);
    }
}