// ===================================THIS FILE WAS AUTO GENERATED===================================
using AutoMapper;
using Arcora.Api.DTOs;
using Arcora.Api.Models;
using Arcora.Api.Enums;
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Arcora.Api.Services.Interfaces;
using Arcora.Api.Configurations;
using Arcora.Payments.Configuration;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stripe;
using System.Text.Json;
using LocalPayout = Arcora.Api.Entities.Payout;
using StripePayout = Stripe.Payout;

namespace Arcora.Api.Services.Implementations
{
    public class OrgPayoutAccountService : IOrgPayoutAccountService
    {
        private readonly IMapper mapper;
        private readonly IMemoryCache cache;
        private readonly ILogger<OrgPayoutAccountService> logger;
        private readonly IOrgPayoutAccountRepository orgpayoutaccountRepository;
        private readonly IOrganizationMemberRepository organizationMemberRepository;
        private readonly ArcoraDbContext dbContext;
        private readonly StripeOptions stripeOptions;
        private readonly AccountService stripeAccountService;
        private readonly AccountLinkService stripeAccountLinkService;
        private readonly IOptions<CacheConfiguration> _options;
        public OrgPayoutAccountService(
            IMapper mapper,
            IMemoryCache cache,
            IOptions<CacheConfiguration> options,
            ILogger<OrgPayoutAccountService> logger,
            IOrgPayoutAccountRepository orgpayoutaccountRepository,
            IOrganizationMemberRepository organizationMemberRepository,
            ArcoraDbContext dbContext,
            IOptions<StripeOptions> stripeOptions)
        {
            this.cache = cache;
            this.logger = logger;
            this.mapper = mapper;
            this.orgpayoutaccountRepository = orgpayoutaccountRepository;
            this.organizationMemberRepository = organizationMemberRepository;
            this.dbContext = dbContext;
            this.stripeOptions = stripeOptions.Value;
            var stripeClient = new StripeClient(this.stripeOptions.SecretKey);
            this.stripeAccountService = new AccountService(stripeClient);
            this.stripeAccountLinkService = new AccountLinkService(stripeClient);
            this._options = options;
            if (this._options.Value.ExpirationTimeInMinutes <= 0)
                this._options.Value.ExpirationTimeInMinutes = 15;
        }

        /// <inheritdoc/>
        public async Task<PagedResult<OrgPayoutAccountDto>> GetAll(Paging paging)
        {
            IEnumerable<OrgPayoutAccount> entities;
            try
            {
                entities = cache.Get<IEnumerable<OrgPayoutAccount>>(Cache.ORGPAYOUTACCOUNTS.ToString()) ?? new List<OrgPayoutAccount>();
                if (entities == null || !entities.Any())
                {
                    entities = (await this.orgpayoutaccountRepository.GetOrgPayoutAccountAsync())?.Where(x => x != null) ?? new List<OrgPayoutAccount>();
                    if (entities != null && entities.Any())
                        cache.Set<IEnumerable<OrgPayoutAccount>>(Cache.ORGPAYOUTACCOUNTS.ToString(), entities, DateTime.UtcNow.AddMinutes(this._options.Value.ExpirationTimeInMinutes));
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching OrgPayoutAccount by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                return new PagedResult<OrgPayoutAccountDto>
                {
                    Data = new List<OrgPayoutAccountDto>(),
                    TotalCount = 0
                };
            }

            IEnumerable<OrgPayoutAccount> filteredEntities = entities!;
            if (!string.IsNullOrEmpty(paging?.Search))
            {
                filteredEntities = entities!.Where(x => !string.IsNullOrEmpty(x.ProviderAccountID) && x.ProviderAccountID.Contains(paging.Search, StringComparison.OrdinalIgnoreCase));
            }

            int totalCount = filteredEntities.Count();
            var pagedEntities = filteredEntities.OrderByDescending(x => x.OrgPayoutAccountID).Skip((paging!.PageNumber - 1) * paging.PageSize).Take(paging.PageSize).ToList();
            var pagedDtos = this.mapper.Map<IEnumerable<OrgPayoutAccountDto>>(pagedEntities);
            return new PagedResult<OrgPayoutAccountDto>
            {
                Data = pagedDtos,
                TotalCount = totalCount
            };
        }

        /// <inheritdoc/>
        public async Task<OrgPayoutAccountDto?> GetID(long ID)
        {
            try
            {
                IEnumerable<OrgPayoutAccount> entities = cache.Get<IEnumerable<OrgPayoutAccount>>(Cache.ORGPAYOUTACCOUNTS.ToString()) ?? new List<OrgPayoutAccount>();
                OrgPayoutAccount? match;
                if (entities != null && entities.Any())
                {
                    match = entities.FirstOrDefault(x => x.OrgPayoutAccountID == ID);
                }
                else
                {
                    match = await this.orgpayoutaccountRepository.GetByID(ID);
                }

                return match == null ? null : this.mapper.Map<OrgPayoutAccountDto>(match);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while fetching OrgPayoutAccount by ID. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<OrgPayoutAccountDto> CreateOrgPayoutAccount(OrgPayoutAccountDto orgpayoutaccountDto)
        {
            OrgPayoutAccount orgPayoutAccount = new OrgPayoutAccount();
            IEnumerable<OrgPayoutAccount?> checkEntity;
            try
            {
                checkEntity = await this.orgpayoutaccountRepository.Find(x => x.ProviderAccountID!.ToLower().Trim() == orgpayoutaccountDto.ProviderAccountID!.ToLower().Trim());
                if (checkEntity == null || !checkEntity.Any())
                {
                    orgPayoutAccount = this.mapper.Map<OrgPayoutAccount>(orgpayoutaccountDto);
                    orgPayoutAccount.CapturedDate = DateTime.UtcNow;
                    orgPayoutAccount = await orgpayoutaccountRepository.Create(orgPayoutAccount) ?? new OrgPayoutAccount();
                    await orgpayoutaccountRepository.Save();
                    cache.Remove(Cache.ORGPAYOUTACCOUNTS.ToString());
                }
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while creating OrgPayoutAccount. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return this.mapper.Map<OrgPayoutAccountDto>(orgPayoutAccount);
        }

        /// <inheritdoc/>
        public async Task<OrgPayoutAccountDto?> UpdateOrgPayoutAccount(long id, OrgPayoutAccountDto orgpayoutaccountDto)
        {
            try
            {
                var existing = await this.orgpayoutaccountRepository.GetByID(id);
                if (existing == null)
                    return null;
                OrgPayoutAccount orgPayoutAccount = this.mapper.Map<OrgPayoutAccount>(orgpayoutaccountDto);
                orgPayoutAccount = await orgpayoutaccountRepository.Update(orgPayoutAccount) ?? new OrgPayoutAccount();
                await orgpayoutaccountRepository.Save();
                cache.Remove(Cache.ORGPAYOUTACCOUNTS.ToString());
                orgpayoutaccountDto = this.mapper.Map<OrgPayoutAccountDto>(orgPayoutAccount);
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while updating OrgPayoutAccount. Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }

            return orgpayoutaccountDto;
        }

        /// <inheritdoc/>
        public async Task DeleteOrgPayoutAccount(long ID)
        {
            try
            {
                var orgPayoutAccount = await this.orgpayoutaccountRepository.GetByID(ID);
                if (orgPayoutAccount == null)
                    throw new KeyNotFoundException("OrgPayoutAccount with the specified ID was not found.");
                await orgpayoutaccountRepository.Delete(orgPayoutAccount);
                await orgpayoutaccountRepository.Save();
                cache.Remove(Cache.ORGPAYOUTACCOUNTS.ToString());
            }
            catch (Exception er)
            {
                logger.LogError(er, "An error occurred while deleting OrgPayoutAccount . Timestamp: {Timestamp}", DateTime.UtcNow);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<PayoutAccountStatusDto?> GetOrganizationPayoutAccountStatus(Guid organizationID)
        {
            var account = await GetPreferredActiveOrgAccount(organizationID);
            return account == null ? null : MapStatus(account);
        }

        /// <inheritdoc/>
        public async Task<OrgPayoutOnboardingLinkResponseDto> CreateOnboardingLink(OrgPayoutOnboardingLinkRequestDto request, long actorUserID)
        {
            if (request.OrganizationID == Guid.Empty)
                throw new ArgumentException("OrganizationID is required.", nameof(request.OrganizationID));
            if (string.IsNullOrWhiteSpace(request.ReturnUrl) || string.IsNullOrWhiteSpace(request.RefreshUrl))
                throw new ArgumentException("ReturnUrl and RefreshUrl are required.");
            if (string.IsNullOrWhiteSpace(stripeOptions.SecretKey))
                throw new ArgumentException("Stripe is not configured.");

            await EnsureUserCanManageOrganization(request.OrganizationID, actorUserID);

            var account = await GetOrCreateStripeAccount(request.OrganizationID, actorUserID);
            var stripeAccountID = account.StripeAccountID ?? account.ProviderAccountID;
            if (string.IsNullOrWhiteSpace(stripeAccountID))
                throw new InvalidOperationException("Stripe account could not be resolved.");

            var link = await stripeAccountLinkService.CreateAsync(new AccountLinkCreateOptions
            {
                Account = stripeAccountID,
                Type = "account_onboarding",
                ReturnUrl = request.ReturnUrl,
                RefreshUrl = request.RefreshUrl
            });

            return new OrgPayoutOnboardingLinkResponseDto
            {
                OrganizationID = request.OrganizationID,
                StripeAccountID = stripeAccountID,
                OnboardingUrl = link.Url,
                ExpiresAt = link.ExpiresAt
            };
        }

        /// <inheritdoc/>
        public async Task<PayoutAccountStatusDto?> RefreshOrganizationPayoutAccountStatus(Guid organizationID)
        {
            var account = await GetPreferredActiveOrgAccount(organizationID);
            if (account == null)
                return null;

            var stripeAccountID = account.StripeAccountID ?? account.ProviderAccountID;
            if (!string.IsNullOrWhiteSpace(stripeAccountID))
            {
                await UpdateFromStripeAccount(account, stripeAccountID);
            }

            return MapStatus(account);
        }

        /// <inheritdoc/>
        public async Task SyncStripeAccountByStripeAccountID(string stripeAccountID)
        {
            if (string.IsNullOrWhiteSpace(stripeAccountID))
                return;

            var account = (await orgpayoutaccountRepository.Find(x =>
                    x.IsActive && (x.StripeAccountID == stripeAccountID || x.ProviderAccountID == stripeAccountID)))
                .Where(x => x != null)
                .Select(x => x!)
                .OrderByDescending(x => x.IsDefault)
                .FirstOrDefault();

            if (account == null)
                return;

            await UpdateFromStripeAccount(account, stripeAccountID);
        }

        /// <inheritdoc/>
        public async Task SyncStripePayoutEvent(string stripeAccountID, string rawPayload)
        {
            if (string.IsNullOrWhiteSpace(stripeAccountID) || string.IsNullOrWhiteSpace(rawPayload))
                return;

            var account = (await orgpayoutaccountRepository.Find(x =>
                    x.IsActive && (x.StripeAccountID == stripeAccountID || x.ProviderAccountID == stripeAccountID)))
                .Where(x => x != null)
                .Select(x => x!)
                .OrderByDescending(x => x.IsDefault)
                .FirstOrDefault();

            if (account == null)
                return;

            var stripeEvent = EventUtility.ParseEvent(rawPayload);
            if (stripeEvent.Data.Object is not StripePayout payout)
                return;

            var existing = this.dbContext.Payouts.FirstOrDefault(x => x.ProviderPayoutID == payout.Id);
            var payoutDate = payout.ArrivalDate != DateTime.MinValue ? payout.ArrivalDate : (DateTime?)null;
            var requestedAt = payout.Created != DateTime.MinValue ? payout.Created : DateTime.UtcNow;
            var normalizedStatus = payout.Status?.ToString()?.ToUpperInvariant() ?? string.Empty;

            if (existing == null)
            {
                existing = new LocalPayout
                {
                    OrganizationID = account.OrganizationID,
                    OrgPayoutAccountID = account.OrgPayoutAccountID,
                    Amount = payout.Amount / 100m,
                    Currency = payout.Currency?.ToUpperInvariant() ?? account.Currency,
                    Status = normalizedStatus,
                    ScheduledAt = payoutDate,
                    RequestedAt = requestedAt,
                    PaidAt = normalizedStatus == "PAID" ? DateTime.UtcNow : null,
                    ProviderName = "Stripe",
                    ProviderPayoutID = payout.Id,
                    FailureReason = payout.FailureMessage,
                    CapturedDate = DateTime.UtcNow,
                    CapturedBy = "STRIPE_WEBHOOK"
                };
                this.dbContext.Payouts.Add(existing);
            }
            else
            {
                existing.Status = normalizedStatus;
                existing.ScheduledAt = payoutDate ?? existing.ScheduledAt;
                existing.PaidAt = normalizedStatus == "PAID" ? DateTime.UtcNow : existing.PaidAt;
                existing.FailureReason = payout.FailureMessage;
                existing.ProcessedAt = DateTime.UtcNow;
            }

            await this.dbContext.SaveChangesAsync();
            cache.Remove(Cache.PAYOUTS.ToString());
        }

        private async Task EnsureUserCanManageOrganization(Guid organizationID, long actorUserID)
        {
            var members = await organizationMemberRepository.GetOrganizationMembersByOrgIDAsync(organizationID);
            var hasAccess = members.Any(x =>
                x.UserID == actorUserID &&
                !string.Equals(x.Status, "DEACTIVATED", StringComparison.OrdinalIgnoreCase) &&
                (x.IsPrimaryOwner ||
                 string.Equals(x.RoleName, "OWNER", StringComparison.OrdinalIgnoreCase) ||
                 string.Equals(x.RoleName, "ADMIN", StringComparison.OrdinalIgnoreCase) ||
                 string.Equals(x.RoleName, "LANDLORD", StringComparison.OrdinalIgnoreCase)));

            if (!hasAccess)
                throw new UnauthorizedAccessException("User is not authorized to manage this organization.");
        }

        private async Task<OrgPayoutAccount> GetOrCreateStripeAccount(Guid organizationID, long actorUserID)
        {
            var existing = await GetPreferredActiveOrgAccount(organizationID);
            if (existing != null && (!string.IsNullOrWhiteSpace(existing.StripeAccountID) || string.Equals(existing.ProviderName, "Stripe", StringComparison.OrdinalIgnoreCase)))
            {
                existing.StripeAccountID ??= existing.ProviderAccountID;
                if (!string.IsNullOrWhiteSpace(existing.StripeAccountID))
                    return existing;
            }

            Account stripeAccount;
            try
            {
                stripeAccount = await stripeAccountService.CreateAsync(new AccountCreateOptions
                {
                    Type = "express",
                    Capabilities = new AccountCapabilitiesOptions
                    {
                        CardPayments = new AccountCapabilitiesCardPaymentsOptions { Requested = true },
                        Transfers = new AccountCapabilitiesTransfersOptions { Requested = true }
                    },
                    Metadata = new Dictionary<string, string>
                    {
                        ["organization_id"] = organizationID.ToString()
                    }
                });
            }
            catch (StripeException ex) when ((ex.Message ?? string.Empty).Contains("Accounts v1", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Stripe account creation is blocked by your Stripe API policy. Enable 'Accounts v1 support' in Stripe Dashboard (Developers > API policies), or migrate this flow to Stripe Accounts v2.",
                    ex);
            }

            if (existing == null)
            {
                existing = new OrgPayoutAccount
                {
                    OrganizationID = organizationID,
                    ProviderName = "Stripe",
                    ProviderAccountID = stripeAccount.Id,
                    StripeAccountID = stripeAccount.Id,
                    Currency = "CAD",
                    VerificationStatus = "PENDING",
                    IsDefault = true,
                    IsActive = true,
                    CapturedDate = DateTime.UtcNow,
                    CapturedBy = actorUserID.ToString()
                };
                await orgpayoutaccountRepository.Create(existing);
            }
            else
            {
                existing.ProviderName = "Stripe";
                existing.ProviderAccountID = stripeAccount.Id;
                existing.StripeAccountID = stripeAccount.Id;
                existing.UpdatedDate = DateTime.UtcNow;
                existing.UpdatedBy = actorUserID.ToString();
                await orgpayoutaccountRepository.Update(existing);
            }

            await orgpayoutaccountRepository.Save();
            cache.Remove(Cache.ORGPAYOUTACCOUNTS.ToString());

            await UpdateFromStripeAccount(existing, stripeAccount.Id);
            return existing;
        }

        private async Task UpdateFromStripeAccount(OrgPayoutAccount localAccount, string stripeAccountID)
        {
            var stripeAccount = await stripeAccountService.GetAsync(stripeAccountID);
            localAccount.StripeAccountID = stripeAccount.Id;
            localAccount.ProviderName = "Stripe";
            localAccount.ProviderAccountID = stripeAccount.Id;
            localAccount.DetailsSubmitted = stripeAccount.DetailsSubmitted;
            localAccount.ChargesEnabled = stripeAccount.ChargesEnabled;
            localAccount.PayoutsEnabled = stripeAccount.PayoutsEnabled;
            localAccount.LastStripeSyncAt = DateTime.UtcNow;
            localAccount.VerificationStatus = ResolveVerificationStatus(stripeAccount);

            var currentlyDue = stripeAccount.Requirements?.CurrentlyDue?.Where(x => !string.IsNullOrWhiteSpace(x)).ToList() ?? new List<string>();
            var eventuallyDue = stripeAccount.Requirements?.EventuallyDue?.Where(x => !string.IsNullOrWhiteSpace(x)).ToList() ?? new List<string>();
            localAccount.RequirementsCurrentlyDue = JsonSerializer.Serialize(currentlyDue);
            localAccount.RequirementsEventuallyDue = JsonSerializer.Serialize(eventuallyDue);

            var bank = stripeAccount.ExternalAccounts?.Data?.OfType<BankAccount>().FirstOrDefault();
            if (bank != null)
            {
                localAccount.AccountLast4 = bank.Last4;
                localAccount.BankName = bank.BankName;
                localAccount.AccountType = bank.AccountType;
                localAccount.Currency = (bank.Currency ?? localAccount.Currency)?.ToUpperInvariant();
            }

            localAccount.UpdatedDate = DateTime.UtcNow;
            localAccount.UpdatedBy = "STRIPE_SYNC";
            await orgpayoutaccountRepository.Update(localAccount);
            await orgpayoutaccountRepository.Save();
            cache.Remove(Cache.ORGPAYOUTACCOUNTS.ToString());
        }

        private async Task<OrgPayoutAccount?> GetPreferredActiveOrgAccount(Guid organizationID)
        {
            return (await this.orgpayoutaccountRepository.Find(x => x.OrganizationID == organizationID && x.IsActive))
                .Where(x => x != null)
                .Select(x => x!)
                .OrderByDescending(x => x.IsDefault)
                .ThenByDescending(x => x.VerifiedAt ?? x.CapturedDate)
                .FirstOrDefault();
        }

        private static string ResolveVerificationStatus(Account stripeAccount)
        {
            if (stripeAccount.ChargesEnabled && stripeAccount.PayoutsEnabled)
                return "VERIFIED";

            var currentlyDue = stripeAccount.Requirements?.CurrentlyDue?.Any() == true;
            if (currentlyDue || stripeAccount.DetailsSubmitted)
                return "PENDING";

            return "UNVERIFIED";
        }

        private static PayoutAccountStatusDto MapStatus(OrgPayoutAccount account)
        {
            var currentlyDue = DeserializeStringList(account.RequirementsCurrentlyDue);
            var eventuallyDue = DeserializeStringList(account.RequirementsEventuallyDue);

            return new PayoutAccountStatusDto
            {
                OrganizationID = account.OrganizationID,
                IsConnected = account.ChargesEnabled && account.PayoutsEnabled,
                Provider = account.ProviderName,
                StripeAccountID = account.StripeAccountID ?? account.ProviderAccountID,
                VerificationStatus = account.VerificationStatus,
                MaskedAccount = string.IsNullOrWhiteSpace(account.AccountLast4) ? null : $".... {account.AccountLast4}",
                BankName = account.BankName,
                AccountType = account.AccountType,
                IsDefault = account.IsDefault,
                ChargesEnabled = account.ChargesEnabled,
                PayoutsEnabled = account.PayoutsEnabled,
                DetailsSubmitted = account.DetailsSubmitted,
                RequirementsCurrentlyDue = currentlyDue,
                RequirementsEventuallyDue = eventuallyDue
            };
        }

        private static List<string> DeserializeStringList(string? json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return new List<string>();

            try
            {
                return JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
            }
            catch
            {
                return new List<string>();
            }
        }
    }
}