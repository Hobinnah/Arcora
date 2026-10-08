// ===================================THIS FILE WAS AUTO GENERATED===================================
using Arcora.Api.DTOs;
using Arcora.Api.Models;

namespace Arcora.Api.Services.Interfaces
{
    public interface IOrgPayoutAccountService
    {
        /// <summary>
        /// Retrieves all org payout accounts with optional paging support.
        /// </summary>
        /// <param name = "paging"></param>
        /// <returns></returns>
        Task<PagedResult<OrgPayoutAccountDto>> GetAll(Paging paging);
        /// <summary>
        /// Retrieves a org payout account by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task<OrgPayoutAccountDto?> GetID(long ID);
        /// <summary>
        /// Creates a new org payout account entry.
        /// </summary>
        /// <param name = "orgPayoutAccountDto"></param>
        /// <returns></returns>
        Task<OrgPayoutAccountDto> CreateOrgPayoutAccount(OrgPayoutAccountDto orgPayoutAccountDto);
        /// <summary>
        /// Updates an existing org payout account entry by its ID.
        /// </summary>
        /// <param name = "id"></param>
        /// <param name = "orgpayoutaccountDto"></param>
        /// <returns></returns>
        Task<OrgPayoutAccountDto?> UpdateOrgPayoutAccount(long id, OrgPayoutAccountDto orgpayoutaccountDto);
        /// <summary>
        /// Deletes a orgpayoutaccount entry by its ID.
        /// </summary>
        /// <param name = "ID"></param>
        /// <returns></returns>
        Task DeleteOrgPayoutAccount(long ID);
        /// <summary>
        /// Retrieves payout account connection details for an organization.
        /// </summary>
        /// <param name="organizationID"></param>
        /// <returns></returns>
        Task<PayoutAccountStatusDto?> GetOrganizationPayoutAccountStatus(Guid organizationID);
        Task<PayoutAccountStatusDto?> GetOrganizationPayoutAccountStatus(Guid organizationID, long actorUserID, bool requireManageAccess = false);
        /// <summary>
        /// Creates a Stripe-hosted onboarding link for an organization's payout account.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="actorUserID"></param>
        /// <returns></returns>
        Task<OrgPayoutOnboardingLinkResponseDto> CreateOnboardingLink(OrgPayoutOnboardingLinkRequestDto request, long actorUserID);
        /// <summary>
        /// Refreshes local payout-account status from Stripe and returns the latest view model.
        /// </summary>
        /// <param name="organizationID"></param>
        /// <returns></returns>
        Task<PayoutAccountStatusDto?> RefreshOrganizationPayoutAccountStatus(Guid organizationID);
        Task<PayoutAccountStatusDto?> RefreshOrganizationPayoutAccountStatus(Guid organizationID, long actorUserID);
        /// <summary>
        /// Synchronizes organization payout account fields from Stripe for webhook updates.
        /// </summary>
        /// <param name="stripeAccountID"></param>
        /// <returns></returns>
        Task SyncStripeAccountByStripeAccountID(string stripeAccountID);
        /// <summary>
        /// Upserts payout records from Stripe webhook payloads.
        /// </summary>
        /// <param name="stripeAccountID"></param>
        /// <param name="rawPayload"></param>
        /// <returns></returns>
        Task SyncStripePayoutEvent(string stripeAccountID, string rawPayload);
    }
}