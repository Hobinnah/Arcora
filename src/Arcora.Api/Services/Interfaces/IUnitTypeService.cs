// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.DTOs;
using Arcora.Api.Models;

namespace Arcora.Api.Services.Interfaces
{
    public interface IUnitTypeService
    {
        /// <summary>
        /// Retrieves all unit types with optional paging support.
        /// </summary>
        /// <param name = "paging"></param>
        /// <returns></returns>
        Task<PagedResult<UnitTypeDto>> GetAll(Paging paging);
        /// <summary>
        /// Retrieves a unit type by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task<UnitTypeDto?> GetID(Guid ID);
        /// <summary>
        /// Creates a new unit type entry.
        /// </summary>
        /// <param name = "unitTypeDto"></param>
        /// <returns></returns>
        Task<UnitTypeDto> CreateUnitType(UnitTypeDto unitTypeDto);
        /// <summary>
        /// Updates an existing unit type entry by its ID.
        /// </summary>
        /// <param name = "id"></param>
        /// <param name = "unittypeDto"></param>
        /// <returns></returns>
        Task<UnitTypeDto?> UpdateUnitType(Guid id, UnitTypeDto unittypeDto);
        /// <summary>
        /// Deletes a unittype entry by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task DeleteUnitType(Guid ID);
    }
}