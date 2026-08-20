// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;

namespace Arcora.Api.Repositories.Interfaces
{
    public interface ICreditReportingRepository : IRepository<CreditReporting>
    {
        Task<List<CreditReporting>> GetCreditReportingAsync();
        Task<bool> HasCreditReportingsAsync();
    }
}