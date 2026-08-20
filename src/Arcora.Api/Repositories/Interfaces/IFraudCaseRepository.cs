// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;

namespace Arcora.Api.Repositories.Interfaces
{
    public interface IFraudCaseRepository : IRepository<FraudCase>
    {
        Task<List<FraudCase>> GetFraudCaseAsync();
        Task<bool> HasFraudCasesAsync();
    }
}