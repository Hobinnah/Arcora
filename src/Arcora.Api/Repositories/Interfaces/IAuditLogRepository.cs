// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;

namespace Arcora.Api.Repositories.Interfaces
{
    public interface IAuditLogRepository : IRepository<AuditLog>
    {
        Task<List<AuditLog>> GetAuditLogAsync();
        Task<bool> HasAuditLogsAsync();
    }
}