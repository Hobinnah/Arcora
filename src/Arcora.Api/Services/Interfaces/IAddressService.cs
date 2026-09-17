// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.DTOs;
using Arcora.Api.Models;

namespace Arcora.Api.Services.Interfaces
{
    public interface IAddressService
    {
        /// <summary>
        /// Retrieves all addresses with optional paging support.
        /// </summary>
        /// <param name = "paging"></param>
        /// <returns></returns>
        Task<PagedResult<AddressDto>> GetAll(Paging paging);
        /// <summary>
        /// Retrieves a address by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task<AddressDto?> GetID(Guid ID);
        /// <summary>
        /// Creates a new address entry.
        /// </summary>
        /// <param name = "addressDto"></param>
        /// <returns></returns>
        Task<AddressDto> CreateAddress(AddressDto addressDto);
        /// <summary>
        /// Updates an existing address entry by its ID.
        /// </summary>
        /// <param name = "id"></param>
        /// <param name = "addressDto"></param>
        /// <returns></returns>
        Task<AddressDto?> UpdateAddress(Guid id, AddressDto addressDto);
        /// <summary>
        /// Deletes a address entry by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task DeleteAddress(Guid ID);
        /// <summary>
        /// Lookup addresses via Canada Post AddressComplete using postal code and country.
        /// Returns a list of suggestion DTOs.
        /// </summary>
        Task<List<AddressLookupSuggestionDto>> LookupAddressesByPostalCode(string postalCode, string country = "CA");
    }
}
