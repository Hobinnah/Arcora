// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.DTOs;
using Arcora.Api.Models;

namespace Arcora.Api.Services.Interfaces
{
    public interface ICreditReportingService
    {
        /// <summary>
        /// Retrieves all credit reportings with optional paging support.
        /// </summary>
        /// <param name = "paging"></param>
        /// <returns></returns>
        Task<PagedResult<CreditReportingDto>> GetAll(Paging paging);
        /// <summary>
        /// Retrieves a credit reporting by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task<CreditReportingDto?> GetID(Guid ID);
        /// <summary>
        /// Creates a new credit reporting entry.
        /// </summary>
        /// <param name = "creditReportingDto"></param>
        /// <returns></returns>
        Task<CreditReportingDto> CreateCreditReporting(CreditReportingDto creditReportingDto);
        /// <summary>
        /// Updates an existing credit reporting entry by its ID.
        /// </summary>
        /// <param name = "id"></param>
        /// <param name = "creditreportingDto"></param>
        /// <returns></returns>
        Task<CreditReportingDto?> UpdateCreditReporting(Guid id, CreditReportingDto creditreportingDto);
        /// <summary>
        /// Deletes a creditreporting entry by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task DeleteCreditReporting(Guid ID);
    }
}