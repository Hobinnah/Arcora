// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.DTOs;
using Arcora.Api.Models;

namespace Arcora.Api.Services.Interfaces
{
    public interface IFeeTypeService
    {
        /// <summary>
        /// Retrieves all fee types with optional paging support.
        /// </summary>
        /// <param name = "paging"></param>
        /// <returns></returns>
        Task<PagedResult<FeeTypeDto>> GetAll(Paging paging);
        /// <summary>
        /// Retrieves a fee type by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task<FeeTypeDto?> GetID(int ID);
        /// <summary>
        /// Creates a new fee type entry.
        /// </summary>
        /// <param name = "feeTypeDto"></param>
        /// <returns></returns>
        Task<FeeTypeDto> CreateFeeType(FeeTypeDto feeTypeDto);
        /// <summary>
        /// Updates an existing fee type entry by its ID.
        /// </summary>
        /// <param name = "id"></param>
        /// <param name = "feetypeDto"></param>
        /// <returns></returns>
        Task<FeeTypeDto?> UpdateFeeType(int id, FeeTypeDto feetypeDto);
        /// <summary>
        /// Deletes a feetype entry by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task DeleteFeeType(int ID);
    }
}