// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;

namespace Arcora.Api.Repositories.Interfaces
{
    public interface ILedgerAccountRepository : IRepository<LedgerAccount>
    {
        Task<List<LedgerAccount>> GetLedgerAccountAsync();
        Task<bool> HasLedgerAccountsAsync();
    }
}