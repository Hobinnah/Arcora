// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;

namespace Arcora.Api.Repositories.Interfaces
{
    public interface IPostalLookupRepository : IRepository<PostalLookupSuggestion>
    {
        Task<List<PostalLookupSuggestion>> GetByLookupKeyAsync(string lookupKey);
        Task<bool> HasAnyAsync();
    }
}
