// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;

namespace Arcora.Api.Repositories.Interfaces
{
    public interface IListingRepository : IRepository<Listing>
    {
        Task<List<Listing>> GetListingAsync();
        Task<bool> HasListingsAsync();
    }
}