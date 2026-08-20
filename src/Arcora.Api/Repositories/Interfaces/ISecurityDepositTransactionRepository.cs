// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;

namespace Arcora.Api.Repositories.Interfaces
{
    public interface ISecurityDepositTransactionRepository : IRepository<SecurityDepositTransaction>
    {
        Task<List<SecurityDepositTransaction>> GetSecurityDepositTransactionAsync();
        Task<bool> HasSecurityDepositTransactionsAsync();
    }
}