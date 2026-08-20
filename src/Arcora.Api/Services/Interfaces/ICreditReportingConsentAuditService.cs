// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.DTOs;
using Arcora.Api.Models;

namespace Arcora.Api.Services.Interfaces
{
    public interface ICreditReportingConsentAuditService
    {
        /// <summary>
        /// Retrieves all credit reporting consent audits with optional paging support.
        /// </summary>
        /// <param name = "paging"></param>
        /// <returns></returns>
        Task<PagedResult<CreditReportingConsentAuditDto>> GetAll(Paging paging);
        /// <summary>
        /// Retrieves a credit reporting consent audit by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task<CreditReportingConsentAuditDto?> GetID(Guid ID);
        /// <summary>
        /// Creates a new credit reporting consent audit entry.
        /// </summary>
        /// <param name = "creditReportingConsentAuditDto"></param>
        /// <returns></returns>
        Task<CreditReportingConsentAuditDto> CreateCreditReportingConsentAudit(CreditReportingConsentAuditDto creditReportingConsentAuditDto);
        /// <summary>
        /// Updates an existing credit reporting consent audit entry by its ID.
        /// </summary>
        /// <param name = "id"></param>
        /// <param name = "creditreportingconsentauditDto"></param>
        /// <returns></returns>
        Task<CreditReportingConsentAuditDto?> UpdateCreditReportingConsentAudit(Guid id, CreditReportingConsentAuditDto creditreportingconsentauditDto);
        /// <summary>
        /// Deletes a creditreportingconsentaudit entry by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task DeleteCreditReportingConsentAudit(Guid ID);
    }
}