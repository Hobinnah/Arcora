// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.DTOs;
using Arcora.Api.Models;

namespace Arcora.Api.Services.Interfaces
{
    public interface IListingRuleService
    {
        /// <summary>
        /// Retrieves all listing rules with optional paging support.
        /// </summary>
        /// <param name = "paging"></param>
        /// <returns></returns>
        Task<PagedResult<ListingRuleDto>> GetAll(Paging paging);
        /// <summary>
        /// Retrieves a listing rule by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task<ListingRuleDto?> GetID(Guid ID);
        /// <summary>
        /// Retrieves listing rules for a specific listing.
        /// </summary>
        /// <param name = "listingID"></param>
        /// <returns></returns>
        Task<IEnumerable<ListingRuleDto>> GetByListingID(Guid listingID);
        /// <summary>
        /// Creates a new listing rule entry.
        /// </summary>
        /// <param name = "listingRuleDto"></param>
        /// <returns></returns>
        Task<ListingRuleDto> CreateListingRule(ListingRuleDto listingRuleDto);
        /// <summary>
        /// Updates an existing listing rule entry by its ID.
        /// </summary>
        /// <param name = "id"></param>
        /// <param name = "listingruleDto"></param>
        /// <returns></returns>
        Task<ListingRuleDto?> UpdateListingRule(Guid id, ListingRuleDto listingruleDto);
        /// <summary>
        /// Deletes a listingrule entry by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task DeleteListingRule(Guid ID);
    }
}