// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.DTOs;
using Arcora.Api.Models;

namespace Arcora.Api.Services.Interfaces
{
    public interface IAutopayMandateService
    {
        /// <summary>
        /// Retrieves all autopay mandates with optional paging support.
        /// </summary>
        /// <param name = "paging"></param>
        /// <returns></returns>
        Task<PagedResult<AutopayMandateDto>> GetAll(Paging paging);
        /// <summary>
        /// Retrieves a autopay mandate by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task<AutopayMandateDto?> GetID(Guid ID);
        /// <summary>
        /// Creates a new autopay mandate entry.
        /// </summary>
        /// <param name = "autopayMandateDto"></param>
        /// <returns></returns>
        Task<AutopayMandateDto> CreateAutopayMandate(AutopayMandateDto autopayMandateDto);
        /// <summary>
        /// Updates an existing autopay mandate entry by its ID.
        /// </summary>
        /// <param name = "id"></param>
        /// <param name = "autopaymandateDto"></param>
        /// <returns></returns>
        Task<AutopayMandateDto?> UpdateAutopayMandate(Guid id, AutopayMandateDto autopaymandateDto);
        /// <summary>
        /// Deletes a autopaymandate entry by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task DeleteAutopayMandate(Guid ID);
        /// <summary>
        /// Updates the status of a autopaymandate by its ID.
        /// </summary>
        /// <param name = "id">The unique identifier of the autopaymandate to update.</param>
        /// <param name = "status">The new status value to assign to the autopaymandate.</param>
        /// <returns>
        /// Returns <see cref = "AutopayMandateDto"/> with the updated autopaymandate if successful, or null if not found.
        /// </returns>
        Task<AutopayMandateDto?> UpdateAutopayMandateStatus(Guid id, string status);
    }
}