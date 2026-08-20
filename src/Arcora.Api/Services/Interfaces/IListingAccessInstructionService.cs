// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.DTOs;
using Arcora.Api.Models;

namespace Arcora.Api.Services.Interfaces
{
    public interface IListingAccessInstructionService
    {
        /// <summary>
        /// Retrieves all listing access instructions with optional paging support.
        /// </summary>
        /// <param name = "paging"></param>
        /// <returns></returns>
        Task<PagedResult<ListingAccessInstructionDto>> GetAll(Paging paging);
        /// <summary>
        /// Retrieves a listing access instruction by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task<ListingAccessInstructionDto?> GetID(Guid ID);
        /// <summary>
        /// Creates a new listing access instruction entry.
        /// </summary>
        /// <param name = "listingAccessInstructionDto"></param>
        /// <returns></returns>
        Task<ListingAccessInstructionDto> CreateListingAccessInstruction(ListingAccessInstructionDto listingAccessInstructionDto);
        /// <summary>
        /// Updates an existing listing access instruction entry by its ID.
        /// </summary>
        /// <param name = "id"></param>
        /// <param name = "listingaccessinstructionDto"></param>
        /// <returns></returns>
        Task<ListingAccessInstructionDto?> UpdateListingAccessInstruction(Guid id, ListingAccessInstructionDto listingaccessinstructionDto);
        /// <summary>
        /// Deletes a listingaccessinstruction entry by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task DeleteListingAccessInstruction(Guid ID);
    }
}