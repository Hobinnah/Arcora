using AutoMapper;
using Arcora.Api.DTOs;
using Arcora.Api.Entities;
using Arcora.Api.Repositories.Interfaces;
using Arcora.Api.Services.Interfaces;
using Arcora.Payments.Abstractions;
using Arcora.Payments.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Arcora.Api.Services.Implementations
{
    /// <summary>
    /// Implements tenant payment onboarding. See <see cref="IPaymentOnboardingService"/>.
    /// </summary>
    public class PaymentOnboardingService : IPaymentOnboardingService
    {
        private const string RolePrimary = "PRIMARY";
        private const string RoleBackup = "BACKUP";

        private readonly IMapper mapper;
        private readonly ILogger<PaymentOnboardingService> logger;
        private readonly IPaymentProvider paymentProvider;
        private readonly IOptions<StripeOptions> stripeOptions;
        private readonly UserManager<User> userManager;
        private readonly ITenantRepository tenantRepository;
        private readonly IPaymentMethodRepository paymentMethodRepository;
        private readonly IAutopayMandateRepository autopayMandateRepository;

        public PaymentOnboardingService(
            IMapper mapper,
            ILogger<PaymentOnboardingService> logger,
            IPaymentProvider paymentProvider,
            IOptions<StripeOptions> stripeOptions,
            UserManager<User> userManager,
            ITenantRepository tenantRepository,
            IPaymentMethodRepository paymentMethodRepository,
            IAutopayMandateRepository autopayMandateRepository)
        {
            this.mapper = mapper;
            this.logger = logger;
            this.paymentProvider = paymentProvider;
            this.stripeOptions = stripeOptions;
            this.userManager = userManager;
            this.tenantRepository = tenantRepository;
            this.paymentMethodRepository = paymentMethodRepository;
            this.autopayMandateRepository = autopayMandateRepository;
        }

        /// <inheritdoc/>
        public async Task<PaymentOnboardingStartResponse> StartAsync(Guid tenantId, CancellationToken cancellationToken = default)
        {
            var tenant = await tenantRepository.GetByID(tenantId)
                ?? throw new KeyNotFoundException("Tenant with the specified ID was not found.");

            var existingCustomerId = await GetExistingCustomerIdAsync(tenantId);

            var user = await userManager.FindByIdAsync(tenant.UserID.ToString());
            var email = user?.Email;
            var name = user?.DisplayName;

            var customer = await paymentProvider.EnsureCustomerAsync(existingCustomerId, tenantId.ToString(), email, name, cancellationToken);

            return new PaymentOnboardingStartResponse
            {
                TenantID = tenantId,
                ProviderCustomerID = customer.CustomerId,
                PublishableKey = stripeOptions.Value.PublishableKey,
                ProviderName = paymentProvider.ProviderName
            };
        }

        /// <inheritdoc/>
        public async Task<CreateSetupIntentResponse> CreateSetupIntentAsync(CreateSetupIntentRequest request, CancellationToken cancellationToken = default)
        {
            _ = await tenantRepository.GetByID(request.TenantID)
                ?? throw new KeyNotFoundException("Tenant with the specified ID was not found.");

            var kind = ParseKind(request.MethodKind);

            var customerId = await GetExistingCustomerIdAsync(request.TenantID);
            if (string.IsNullOrWhiteSpace(customerId))
            {
                var start = await StartAsync(request.TenantID, cancellationToken);
                customerId = start.ProviderCustomerID;
            }

            var setupIntent = await paymentProvider.CreateSetupIntentAsync(customerId!, kind, cancellationToken);

            return new CreateSetupIntentResponse
            {
                TenantID = request.TenantID,
                SetupIntentID = setupIntent.SetupIntentId,
                ClientSecret = setupIntent.ClientSecret,
                PublishableKey = stripeOptions.Value.PublishableKey,
                Status = setupIntent.Status
            };
        }

        /// <inheritdoc/>
        public async Task<PaymentMethodDto> SavePaymentMethodAsync(SavePaymentMethodRequest request, CancellationToken cancellationToken = default)
        {
            var tenant = await tenantRepository.GetByID(request.TenantID)
                ?? throw new KeyNotFoundException("Tenant with the specified ID was not found.");

            var kind = ParseKind(request.MethodKind);

            var customerId = await GetExistingCustomerIdAsync(request.TenantID);
            if (string.IsNullOrWhiteSpace(customerId))
            {
                var start = await StartAsync(request.TenantID, cancellationToken);
                customerId = start.ProviderCustomerID;
            }

            var setup = await paymentProvider.SetupPaymentMethodAsync(
                new SetupPaymentMethodRequest(customerId!, kind, request.ProviderPaymentMethodID),
                cancellationToken);

            // Enforce credit-only for the mandatory backup card.
            if (kind == PaymentMethodKind.Card &&
                !string.Equals(setup.Funding, "credit", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("The backup payment method must be a credit card.");
            }

            var role = kind == PaymentMethodKind.Pad ? RolePrimary : RoleBackup;
            var verificationStatus = MapVerificationStatus(setup.VerificationStatus);

            var entity = new PaymentMethod
            {
                PaymentMethodID = Guid.NewGuid(),
                TenantID = request.TenantID,
                PaymentMethodType = kind == PaymentMethodKind.Pad ? "PAD" : "CARD",
                DisplayName = kind == PaymentMethodKind.Pad
                    ? $"{setup.BankName} ••{setup.Last4}"
                    : $"{setup.Brand} ••{setup.Last4}",
                AccountLast4 = setup.Last4,
                CardBrand = setup.Brand,
                CardFunding = setup.Funding,
                BankName = setup.BankName,
                ExpiryMonth = (short?)setup.ExpiryMonth,
                ExpiryYear = (short?)setup.ExpiryYear,
                ProviderName = paymentProvider.ProviderName,
                ProviderCustomerID = customerId,
                ProviderPaymentMethodID = setup.PaymentMethodId,
                VerificationStatus = verificationStatus,
                VerifiedAt = verificationStatus == "VERIFIED" ? DateTime.UtcNow : null,
                MethodRole = role,
                IsDefault = kind == PaymentMethodKind.Pad,
                IsActive = true,
                CapturedDate = DateTime.UtcNow
            };

            await paymentMethodRepository.Create(entity);
            await paymentMethodRepository.Save();

            if (kind == PaymentMethodKind.Pad)
            {
                await CreateMandateAsync(tenant, entity, customerId!, cancellationToken);
                tenant.IsPADRegistered = true;
            }
            else
            {
                tenant.IsCardRegistered = true;
            }

            tenant.UpdatedDate = DateTime.UtcNow;
            await tenantRepository.Update(tenant);
            await tenantRepository.Save();

            return mapper.Map<PaymentMethodDto>(entity);
        }

        /// <inheritdoc/>
        public async Task<PaymentOnboardingStatusResponse> GetStatusAsync(Guid tenantId, CancellationToken cancellationToken = default)
        {
            var methods = await paymentMethodRepository.Find(x => x.TenantID == tenantId && x.IsActive);
            var mandates = await autopayMandateRepository.Find(x => x.TenantID == tenantId && x.Status == "ACTIVE");

            var list = methods?.ToList() ?? new List<PaymentMethod?>();

            return new PaymentOnboardingStatusResponse
            {
                TenantID = tenantId,
                HasVerifiedPad = list.Any(m => m!.MethodRole == RolePrimary && m.VerificationStatus == "VERIFIED"),
                HasVerifiedCard = list.Any(m => m!.MethodRole == RoleBackup && m.VerificationStatus == "VERIFIED"),
                PadMandateActive = mandates != null && mandates.Any()
            };
        }

        /// <inheritdoc/>
        public async Task UpdateVerificationStatusAsync(string? providerPaymentMethodId, string? providerStatus, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(providerPaymentMethodId))
            {
                logger.LogWarning("Verification webhook received without a provider payment method id; skipping.");
                return;
            }

            var methods = await paymentMethodRepository.Find(x => x.ProviderPaymentMethodID == providerPaymentMethodId);
            var method = methods?.FirstOrDefault();
            if (method == null)
            {
                logger.LogWarning("No payment method found for provider id {ProviderId}; verification update skipped.", providerPaymentMethodId);
                return;
            }

            var newStatus = MapVerificationStatus(providerStatus ?? string.Empty);

            // Idempotency: nothing to do if the status is unchanged.
            if (string.Equals(method.VerificationStatus, newStatus, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            method.VerificationStatus = newStatus;
            method.VerifiedAt = newStatus == "VERIFIED" ? DateTime.UtcNow : null;
            method.UpdatedDate = DateTime.UtcNow;
            method.UpdatedBy = "STRIPE_WEBHOOK";
            await paymentMethodRepository.Update(method);
            await paymentMethodRepository.Save();

            // For a verified PAD, activate the associated autopay mandate so autopay can proceed.
            if (newStatus == "VERIFIED" && method.MethodRole == RolePrimary)
            {
                var mandates = await autopayMandateRepository.Find(x => x.PaymentMethodID == method.PaymentMethodID);
                foreach (var mandate in (mandates ?? Enumerable.Empty<AutopayMandate>()).Where(m => m != null && m.Status != "ACTIVE"))
                {
                    mandate!.Status = "ACTIVE";
                    mandate.ActivatedAt = DateTime.UtcNow;
                    mandate.UpdatedDate = DateTime.UtcNow;
                    mandate.UpdatedBy = "STRIPE_WEBHOOK";
                    await autopayMandateRepository.Update(mandate);
                    await autopayMandateRepository.Save();
                }
            }

            logger.LogInformation("Payment method {PaymentMethodId} verification updated to {Status} via webhook.", method.PaymentMethodID, newStatus);
        }

        private async Task CreateMandateAsync(Tenant tenant, PaymentMethod padMethod, string customerId, CancellationToken cancellationToken)
        {
            var mandateResult = await paymentProvider.CreatePadMandateAsync(customerId, padMethod.ProviderPaymentMethodID!, cancellationToken: cancellationToken);

            var mandate = new AutopayMandate
            {
                AutopayMandateID = Guid.NewGuid(),
                LeaseID = null,
                TenantID = tenant.TenantID,
                PaymentMethodID = padMethod.PaymentMethodID,
                Status = MapMandateStatus(mandateResult.Status),
                MandateType = "VARIABLE",
                PaymentRail = "PAD",
                Currency = "CAD",
                Frequency = "MONTHLY",
                StartDate = DateTime.UtcNow,
                ProviderName = paymentProvider.ProviderName,
                ProviderMandateID = mandateResult.MandateId,
                ConsentVersion = "1.0",
                ConsentedAt = DateTime.UtcNow,
                ActivatedAt = MapMandateStatus(mandateResult.Status) == "ACTIVE" ? DateTime.UtcNow : null,
                CapturedDate = DateTime.UtcNow
            };

            await autopayMandateRepository.Create(mandate);
            await autopayMandateRepository.Save();
        }

        private async Task<string?> GetExistingCustomerIdAsync(Guid tenantId)
        {
            var methods = await paymentMethodRepository.Find(x => x.TenantID == tenantId && x.ProviderCustomerID != null);
            return methods?.FirstOrDefault()?.ProviderCustomerID;
        }

        private static PaymentMethodKind ParseKind(string methodKind) => methodKind?.Trim().ToUpperInvariant() switch
        {
            "PAD" => PaymentMethodKind.Pad,
            "CARD" => PaymentMethodKind.Card,
            _ => throw new ArgumentException($"Unsupported MethodKind '{methodKind}'. Expected 'PAD' or 'CARD'.")
        };

        // Stripe SetupIntent / mandate statuses -> Arcora verification statuses.
        private static string MapVerificationStatus(string providerStatus) => providerStatus?.ToLowerInvariant() switch
        {
            "succeeded" => "VERIFIED",
            "active" => "VERIFIED",
            "processing" => "PENDING",
            "pending" => "PENDING",
            "requires_action" => "PENDING",
            "requires_confirmation" => "PENDING",
            "requires_payment_method" => "FAILED",
            "canceled" => "FAILED",
            "inactive" => "FAILED",
            _ => "PENDING"
        };

        private static string MapMandateStatus(string providerStatus) => providerStatus?.ToLowerInvariant() switch
        {
            "active" => "ACTIVE",
            "succeeded" => "ACTIVE",
            "pending" => "PENDING",
            "processing" => "PENDING",
            "inactive" => "PENDING",
            _ => "PENDING"
        };
    }
}
