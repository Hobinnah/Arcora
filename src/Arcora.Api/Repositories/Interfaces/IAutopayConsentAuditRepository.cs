// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;

namespace Arcora.Api.Repositories.Interfaces
{
    public interface IAutopayConsentAuditRepository : IRepository<AutopayConsentAudit>
    {
        Task<List<AutopayConsentAudit>> GetAutopayConsentAuditAsync();
        Task<bool> HasAutopayConsentAuditsAsync();
    }
}