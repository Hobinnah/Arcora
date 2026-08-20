// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.DTOs;
using Arcora.Api.Models;

namespace Arcora.Api.Services.Interfaces
{
    public interface IPropertyService
    {
        /// <summary>
        /// Retrieves all properties with optional paging support.
        /// </summary>
        /// <param name = "paging"></param>
        /// <returns></returns>
        Task<PagedResult<PropertyDto>> GetAll(Paging paging);
        /// <summary>
        /// Retrieves a property by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task<PropertyDto?> GetID(Guid ID);
        /// <summary>
        /// Creates a new property entry.
        /// </summary>
        /// <param name = "propertyDto"></param>
        /// <returns></returns>
        Task<PropertyDto> CreateProperty(PropertyDto propertyDto);
        /// <summary>
        /// Updates an existing property entry by its ID.
        /// </summary>
        /// <param name = "id"></param>
        /// <param name = "propertyDto"></param>
        /// <returns></returns>
        Task<PropertyDto?> UpdateProperty(Guid id, PropertyDto propertyDto);
        /// <summary>
        /// Deletes a property entry by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task DeleteProperty(Guid ID);
        /// <summary>
        /// Updates the status of a property by its ID.
        /// </summary>
        /// <param name = "id">The unique identifier of the property to update.</param>
        /// <param name = "status">The new status value to assign to the property.</param>
        /// <returns>
        /// Returns <see cref = "PropertyDto"/> with the updated property if successful, or null if not found.
        /// </returns>
        Task<PropertyDto?> UpdatePropertyStatus(Guid id, string status);
    }
}