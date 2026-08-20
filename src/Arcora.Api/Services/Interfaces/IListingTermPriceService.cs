// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.DTOs;
using Arcora.Api.Models;

namespace Arcora.Api.Services.Interfaces
{
    public interface IListingTermPriceService
    {
        /// <summary>
        /// Retrieves all listing term prices with optional paging support.
        /// </summary>
        /// <param name = "paging"></param>
        /// <returns></returns>
        Task<PagedResult<ListingTermPriceDto>> GetAll(Paging paging);
        /// <summary>
        /// Retrieves a listing term price by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task<ListingTermPriceDto?> GetID(Guid ID);
        /// <summary>
        /// Creates a new listing term price entry.
        /// </summary>
        /// <param name = "listingTermPriceDto"></param>
        /// <returns></returns>
        Task<ListingTermPriceDto> CreateListingTermPrice(ListingTermPriceDto listingTermPriceDto);
        /// <summary>
        /// Updates an existing listing term price entry by its ID.
        /// </summary>
        /// <param name = "id"></param>
        /// <param name = "listingtermpriceDto"></param>
        /// <returns></returns>
        Task<ListingTermPriceDto?> UpdateListingTermPrice(Guid id, ListingTermPriceDto listingtermpriceDto);
        /// <summary>
        /// Deletes a listingtermprice entry by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task DeleteListingTermPrice(Guid ID);
    }
}