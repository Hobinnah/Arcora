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
        /// Retrieves listing photos by the capturedBy field. 
        /// </summary>
        /// <param name="capturedBy"> The user who captured the photos. </param>
        /// <returns></returns>
        Task<List<ListingPhotoDto?>> GetPhotosByCapturedBy(string capturedBy);

        /// <summary>
        /// Creates a new listing photo entry.
        /// </summary>
        /// <param name = "listingPhotoDto"></param>
        /// <returns></returns>
        Task<ListingPhotoDto> CreateListingPhoto(ListingPhotoDto listingPhotoDto);
        /// <summary>
        /// Uploads a listing photo via multipart/form-data and persists metadata.
        /// </summary>
        Task<ListingPhotoDto> UploadListingPhoto(Arcora.Api.Models.ListingPhotoUploadRequest request, CancellationToken cancellationToken = default);
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