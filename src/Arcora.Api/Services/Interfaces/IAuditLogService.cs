// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.DTOs;
using Arcora.Api.Models;

namespace Arcora.Api.Services.Interfaces
{
    public interface IAuditLogService
    {
        /// <summary>
        /// Retrieves all audit logs with optional paging support.
        /// </summary>
        /// <param name = "paging"></param>
        /// <returns></returns>
        Task<PagedResult<AuditLogDto>> GetAll(Paging paging);
        /// <summary>
        /// Retrieves a audit log by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task<AuditLogDto?> GetID(Guid ID);
        /// <summary>
        /// Creates a new audit log entry.
        /// </summary>
        /// <param name = "auditLogDto"></param>
        /// <returns></returns>
        Task<AuditLogDto> CreateAuditLog(AuditLogDto auditLogDto);
        /// <summary>
        /// Updates an existing audit log entry by its ID.
        /// </summary>
        /// <param name = "id"></param>
        /// <param name = "auditlogDto"></param>
        /// <returns></returns>
        Task<AuditLogDto?> UpdateAuditLog(Guid id, AuditLogDto auditlogDto);
        /// <summary>
        /// Deletes a auditlog entry by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task DeleteAuditLog(Guid ID);
    }
}