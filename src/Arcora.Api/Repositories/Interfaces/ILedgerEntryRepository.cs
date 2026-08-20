// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;

namespace Arcora.Api.Repositories.Interfaces
{
    public interface ILedgerEntryRepository : IRepository<LedgerEntry>
    {
        Task<List<LedgerEntry>> GetLedgerEntryAsync();
        Task<bool> HasLedgerEntriesAsync();
    }
}