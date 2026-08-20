// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;

namespace Arcora.Api.Repositories.Interfaces
{
    public interface ISecurityDepositRepository : IRepository<SecurityDeposit>
    {
        Task<List<SecurityDeposit>> GetSecurityDepositAsync();
        Task<bool> HasSecurityDepositsAsync();
    }
}