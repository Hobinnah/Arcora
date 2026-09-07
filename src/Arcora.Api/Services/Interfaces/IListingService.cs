// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.DTOs;
using Arcora.Api.Models;

namespace Arcora.Api.Services.Interfaces
{
    public interface IListingService
    {
        /// <summary>
        /// Retrieves all listings with optional paging support.
        /// </summary>
        /// <param name = "paging"></param>
        /// <returns></returns>
        Task<PagedResult<ListingDto>> GetAll(Paging paging);
        /// <summary>
        /// Efficiently searches available listings using Airbnb-style criteria (location,
        /// move-in date, stay/lease length, party size and optional filters).
        /// </summary>
        /// <param name = "criteria">The search criteria.</param>
        /// <returns>A paged set of matching listings.</returns>
        Task<PagedResult<ListingDto>> SearchListings(ListingSearchCriteria criteria);
        /// <summary>
        /// Retrieves a listing by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task<ListingDto?> GetID(Guid ID);
        /// <summary>
        /// Creates a new listing entry.
        /// </summary>
        /// <param name = "listingDto"></param>
        /// <returns></returns>
        Task<ListingDto> CreateListing(ListingDto listingDto);
        /// <summary>
        /// Updates an existing listing entry by its ID.
        /// </summary>
        /// <param name = "id"></param>
        /// <param name = "listingDto"></param>
        /// <returns></returns>
        Task<ListingDto?> UpdateListing(Guid id, ListingDto listingDto);
        /// <summary>
        /// Deletes a listing entry by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task DeleteListing(Guid ID);
        /// <summary>
        /// Updates the status of a listing by its ID.
        /// </summary>
        /// <param name = "id">The unique identifier of the listing to update.</param>
        /// <param name = "status">The new status value to assign to the listing.</param>
        /// <returns>
        /// Returns <see cref = "ListingDto"/> with the updated listing if successful, or null if not found.
        /// </returns>
        Task<ListingDto?> UpdateListingStatus(Guid id, string status);
    }
}