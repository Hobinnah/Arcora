// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.DTOs;
using Arcora.Api.Models;

namespace Arcora.Api.Services.Interfaces
{
    public interface IListingAmenityService
    {
        /// <summary>
        /// Retrieves all listing amenities with optional paging support.
        /// </summary>
        /// <param name = "paging"></param>
        /// <returns></returns>
        Task<PagedResult<ListingAmenityDto>> GetAll(Paging paging);
        /// <summary>
        /// Retrieves a listing amenity by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task<ListingAmenityDto?> GetID(Guid ID);
        /// <summary>
        /// Creates a new listing amenity entry.
        /// </summary>
        /// <param name = "listingAmenityDto"></param>
        /// <returns></returns>
        Task<ListingAmenityDto> CreateListingAmenity(ListingAmenityDto listingAmenityDto);
        /// <summary>
        /// Updates an existing listing amenity entry by its ID.
        /// </summary>
        /// <param name = "id"></param>
        /// <param name = "listingamenityDto"></param>
        /// <returns></returns>
        Task<ListingAmenityDto?> UpdateListingAmenity(Guid id, ListingAmenityDto listingamenityDto);
        /// <summary>
        /// Deletes a listingamenity entry by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task DeleteListingAmenity(Guid ID);
    }
}