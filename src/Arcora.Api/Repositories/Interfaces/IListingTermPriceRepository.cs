// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;

namespace Arcora.Api.Repositories.Interfaces
{
    public interface IListingTermPriceRepository : IRepository<ListingTermPrice>
    {
        Task<List<ListingTermPrice>> GetListingTermPriceAsync();
        Task<bool> HasListingTermPricesAsync();
    }
}