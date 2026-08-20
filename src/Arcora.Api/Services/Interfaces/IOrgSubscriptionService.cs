// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.DTOs;
using Arcora.Api.Models;

namespace Arcora.Api.Services.Interfaces
{
    public interface IOrgSubscriptionService
    {
        /// <summary>
        /// Retrieves all org subscriptions with optional paging support.
        /// </summary>
        /// <param name = "paging"></param>
        /// <returns></returns>
        Task<PagedResult<OrgSubscriptionDto>> GetAll(Paging paging);
        /// <summary>
        /// Retrieves a org subscription by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task<OrgSubscriptionDto?> GetID(Guid ID);
        /// <summary>
        /// Creates a new org subscription entry.
        /// </summary>
        /// <param name = "orgSubscriptionDto"></param>
        /// <returns></returns>
        Task<OrgSubscriptionDto> CreateOrgSubscription(OrgSubscriptionDto orgSubscriptionDto);
        /// <summary>
        /// Updates an existing org subscription entry by its ID.
        /// </summary>
        /// <param name = "id"></param>
        /// <param name = "orgsubscriptionDto"></param>
        /// <returns></returns>
        Task<OrgSubscriptionDto?> UpdateOrgSubscription(Guid id, OrgSubscriptionDto orgsubscriptionDto);
        /// <summary>
        /// Deletes a orgsubscription entry by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task DeleteOrgSubscription(Guid ID);
        /// <summary>
        /// Updates the status of a orgsubscription by its ID.
        /// </summary>
        /// <param name = "id">The unique identifier of the orgsubscription to update.</param>
        /// <param name = "status">The new status value to assign to the orgsubscription.</param>
        /// <returns>
        /// Returns <see cref = "OrgSubscriptionDto"/> with the updated orgsubscription if successful, or null if not found.
        /// </returns>
        Task<OrgSubscriptionDto?> UpdateOrgSubscriptionStatus(Guid id, string status);
    }
}