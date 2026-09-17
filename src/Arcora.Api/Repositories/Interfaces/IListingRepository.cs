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
        /// Retrieves all listings that belong to the specified organization, including the
        /// navigation data needed to project a <c>ListingDto</c>.
        /// </summary>
        /// <param name="organizationId">The organization whose listings should be returned.</param>
        /// <returns>The organization's listings ordered by the default (most recent first) order.</returns>
        Task<List<Listing>> GetListingsByOrganizationAsync(Guid organizationId);

        /// <summary>
        /// Counts the listings that belong to the specified organization. The count is
        /// evaluated server-side without materializing the entities.
        /// </summary>
        /// <param name="organizationId">The organization whose listings should be counted.</param>
        /// <returns>The number of listings owned by the organization.</returns>
        Task<int> CountListingsByOrganizationAsync(Guid organizationId);

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
