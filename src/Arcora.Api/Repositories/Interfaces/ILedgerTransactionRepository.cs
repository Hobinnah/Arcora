// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;

namespace Arcora.Api.Repositories.Interfaces
{
    public interface ILedgerTransactionRepository : IRepository<LedgerTransaction>
    {
        Task<List<LedgerTransaction>> GetLedgerTransactionAsync();
        Task<bool> HasLedgerTransactionsAsync();
    }
}