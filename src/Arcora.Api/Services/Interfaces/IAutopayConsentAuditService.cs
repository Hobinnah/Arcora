// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.DTOs;
using Arcora.Api.Models;

namespace Arcora.Api.Services.Interfaces
{
    public interface IAutopayConsentAuditService
    {
        /// <summary>
        /// Retrieves all autopay consent audits with optional paging support.
        /// </summary>
        /// <param name = "paging"></param>
        /// <returns></returns>
        Task<PagedResult<AutopayConsentAuditDto>> GetAll(Paging paging);
        /// <summary>
        /// Retrieves a autopay consent audit by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task<AutopayConsentAuditDto?> GetID(Guid ID);
        /// <summary>
        /// Creates a new autopay consent audit entry.
        /// </summary>
        /// <param name = "autopayConsentAuditDto"></param>
        /// <returns></returns>
        Task<AutopayConsentAuditDto> CreateAutopayConsentAudit(AutopayConsentAuditDto autopayConsentAuditDto);
        /// <summary>
        /// Updates an existing autopay consent audit entry by its ID.
        /// </summary>
        /// <param name = "id"></param>
        /// <param name = "autopayconsentauditDto"></param>
        /// <returns></returns>
        Task<AutopayConsentAuditDto?> UpdateAutopayConsentAudit(Guid id, AutopayConsentAuditDto autopayconsentauditDto);
        /// <summary>
        /// Deletes a autopayconsentaudit entry by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task DeleteAutopayConsentAudit(Guid ID);
    }
}