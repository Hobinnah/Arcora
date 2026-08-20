// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.DTOs;
using Arcora.Api.Models;

namespace Arcora.Api.Services.Interfaces
{
    public interface IListingPhotoService
    {
        /// <summary>
        /// Retrieves all listing photos with optional paging support.
        /// </summary>
        /// <param name = "paging"></param>
        /// <returns></returns>
        Task<PagedResult<ListingPhotoDto>> GetAll(Paging paging);
        /// <summary>
        /// Retrieves a listing photo by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task<ListingPhotoDto?> GetID(Guid ID);
        /// <summary>
        /// Creates a new listing photo entry.
        /// </summary>
        /// <param name = "listingPhotoDto"></param>
        /// <returns></returns>
        Task<ListingPhotoDto> CreateListingPhoto(ListingPhotoDto listingPhotoDto);
        /// <summary>
        /// Updates an existing listing photo entry by its ID.
        /// </summary>
        /// <param name = "id"></param>
        /// <param name = "listingphotoDto"></param>
        /// <returns></returns>
        Task<ListingPhotoDto?> UpdateListingPhoto(Guid id, ListingPhotoDto listingphotoDto);
        /// <summary>
        /// Deletes a listingphoto entry by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task DeleteListingPhoto(Guid ID);
    }
}