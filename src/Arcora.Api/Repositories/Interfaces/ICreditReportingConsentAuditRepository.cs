// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;

namespace Arcora.Api.Repositories.Interfaces
{
    public interface ICreditReportingConsentAuditRepository : IRepository<CreditReportingConsentAudit>
    {
        Task<List<CreditReportingConsentAudit>> GetCreditReportingConsentAuditAsync();
        Task<bool> HasCreditReportingConsentAuditsAsync();
    }
}