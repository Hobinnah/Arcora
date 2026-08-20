// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.DTOs;
using Arcora.Api.Models;

namespace Arcora.Api.Services.Interfaces
{
    public interface IListingTypeService
    {
        /// <summary>
        /// Retrieves all listing types with optional paging support.
        /// </summary>
        /// <param name = "paging"></param>
        /// <returns></returns>
        Task<PagedResult<ListingTypeDto>> GetAll(Paging paging);
        /// <summary>
        /// Retrieves a listing type by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task<ListingTypeDto?> GetID(Guid ID);
        /// <summary>
        /// Creates a new listing type entry.
        /// </summary>
        /// <param name = "listingTypeDto"></param>
        /// <returns></returns>
        Task<ListingTypeDto> CreateListingType(ListingTypeDto listingTypeDto);
        /// <summary>
        /// Updates an existing listing type entry by its ID.
        /// </summary>
        /// <param name = "id"></param>
        /// <param name = "listingtypeDto"></param>
        /// <returns></returns>
        Task<ListingTypeDto?> UpdateListingType(Guid id, ListingTypeDto listingtypeDto);
        /// <summary>
        /// Deletes a listingtype entry by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task DeleteListingType(Guid ID);
    }
}