// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.DTOs;
using Arcora.Api.Models;

namespace Arcora.Api.Services.Interfaces
{
    public interface IListingPolicyService
    {
        /// <summary>
        /// Retrieves all listing policies with optional paging support.
        /// </summary>
        /// <param name = "paging"></param>
        /// <returns></returns>
        Task<PagedResult<ListingPolicyDto>> GetAll(Paging paging);
        /// <summary>
        /// Retrieves a listing policy by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task<ListingPolicyDto?> GetID(Guid ID);
        /// <summary>
        /// Creates a new listing policy entry.
        /// </summary>
        /// <param name = "listingPolicyDto"></param>
        /// <returns></returns>
        Task<ListingPolicyDto> CreateListingPolicy(ListingPolicyDto listingPolicyDto);
        /// <summary>
        /// Updates an existing listing policy entry by its ID.
        /// </summary>
        /// <param name = "id"></param>
        /// <param name = "listingpolicyDto"></param>
        /// <returns></returns>
        Task<ListingPolicyDto?> UpdateListingPolicy(Guid id, ListingPolicyDto listingpolicyDto);
        /// <summary>
        /// Deletes a listingpolicy entry by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task DeleteListingPolicy(Guid ID);
    }
}