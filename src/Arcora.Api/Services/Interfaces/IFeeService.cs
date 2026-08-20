// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.DTOs;
using Arcora.Api.Models;

namespace Arcora.Api.Services.Interfaces
{
    public interface IFeeService
    {
        /// <summary>
        /// Retrieves all fees with optional paging support.
        /// </summary>
        /// <param name = "paging"></param>
        /// <returns></returns>
        Task<PagedResult<FeeDto>> GetAll(Paging paging);
        /// <summary>
        /// Retrieves a fee by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task<FeeDto?> GetID(Guid ID);
        /// <summary>
        /// Creates a new fee entry.
        /// </summary>
        /// <param name = "feeDto"></param>
        /// <returns></returns>
        Task<FeeDto> CreateFee(FeeDto feeDto);
        /// <summary>
        /// Updates an existing fee entry by its ID.
        /// </summary>
        /// <param name = "id"></param>
        /// <param name = "feeDto"></param>
        /// <returns></returns>
        Task<FeeDto?> UpdateFee(Guid id, FeeDto feeDto);
        /// <summary>
        /// Deletes a fee entry by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task DeleteFee(Guid ID);
    }
}