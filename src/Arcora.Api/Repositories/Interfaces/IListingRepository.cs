// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.Entities;
using Arcora.Api.Models;

namespace Arcora.Api.Repositories.Interfaces
{
    public interface IListingRepository : IRepository<Listing>
    {
        Task<List<Listing>> GetListingsAsync();
        Task<Listing?> GetListingAsync(Guid id);
        Task<bool> HasListingsAsync();

        /// <summary>
        /// Efficiently searches published, available listings using Airbnb-style criteria
        /// (location, move-in date, stay/lease length, party size and optional filters).
        /// Filtering, availability checks and paging are performed server-side.
        /// </summary>
        /// <param name="criteria">The search criteria.</param>
        /// <returns>The matching page of listings and the total match count.</returns>
        Task<(List<Listing> Items, int TotalCount)> SearchListingsAsync(ListingSearchCriteria criteria);
    }
}
