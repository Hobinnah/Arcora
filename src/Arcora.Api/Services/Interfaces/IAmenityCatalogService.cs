// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.DTOs;
using Arcora.Api.Models;

namespace Arcora.Api.Services.Interfaces
{
    public interface IAmenityCatalogService
    {
        /// <summary>
        /// Retrieves all amenity catalogs with optional paging support.
        /// </summary>
        /// <param name = "paging"></param>
        /// <returns></returns>
        Task<PagedResult<AmenityCatalogDto>> GetAll(Paging paging);
        /// <summary>
        /// Retrieves a amenity catalog by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task<AmenityCatalogDto?> GetID(Guid ID);
        /// <summary>
        /// Creates a new amenity catalog entry.
        /// </summary>
        /// <param name = "amenityCatalogDto"></param>
        /// <returns></returns>
        Task<AmenityCatalogDto> CreateAmenityCatalog(AmenityCatalogDto amenityCatalogDto);
        /// <summary>
        /// Updates an existing amenity catalog entry by its ID.
        /// </summary>
        /// <param name = "id"></param>
        /// <param name = "amenitycatalogDto"></param>
        /// <returns></returns>
        Task<AmenityCatalogDto?> UpdateAmenityCatalog(Guid id, AmenityCatalogDto amenitycatalogDto);
        /// <summary>
        /// Deletes a amenitycatalog entry by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task DeleteAmenityCatalog(Guid ID);
    }
}