// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.DTOs;
using Arcora.Api.Models;

namespace Arcora.Api.Services.Interfaces
{
    public interface ISubscriptionPlanService
    {
        /// <summary>
        /// Retrieves all subscription plans with optional paging support.
        /// </summary>
        /// <param name = "paging"></param>
        /// <returns></returns>
        Task<PagedResult<SubscriptionPlanDto>> GetAll(Paging paging);
        /// <summary>
        /// Retrieves a subscription plan by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task<SubscriptionPlanDto?> GetID(Guid ID);
        /// <summary>
        /// Creates a new subscription plan entry.
        /// </summary>
        /// <param name = "subscriptionPlanDto"></param>
        /// <returns></returns>
        Task<SubscriptionPlanDto> CreateSubscriptionPlan(SubscriptionPlanDto subscriptionPlanDto);
        /// <summary>
        /// Updates an existing subscription plan entry by its ID.
        /// </summary>
        /// <param name = "id"></param>
        /// <param name = "subscriptionplanDto"></param>
        /// <returns></returns>
        Task<SubscriptionPlanDto?> UpdateSubscriptionPlan(Guid id, SubscriptionPlanDto subscriptionplanDto);
        /// <summary>
        /// Deletes a subscriptionplan entry by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task DeleteSubscriptionPlan(Guid ID);
    }
}