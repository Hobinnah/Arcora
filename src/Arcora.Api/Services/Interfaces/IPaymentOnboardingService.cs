using Arcora.Api.DTOs;

namespace Arcora.Api.Services.Interfaces
{
    /// <summary>
    /// Coordinates tenant payment onboarding: provider customer creation, saving the primary PAD
    /// (with ACSS mandate) and the mandatory backup credit card, and reporting readiness.
    ///
    /// Raw instrument details are collected client-side by the provider SDK; only opaque provider
    /// tokens ever reach this service.
    /// </summary>
    public interface IPaymentOnboardingService
    {
        /// <summary>
        /// Ensures a provider customer exists for the tenant and returns SDK initialization data.
        /// </summary>
        Task<PaymentOnboardingStartResponse> StartAsync(Guid tenantId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a provider SetupIntent for the given method kind and returns its client secret so
        /// the client SDK can collect and confirm instrument details (e.g. ACSS debit) directly with
        /// the provider. Only the client secret is returned - raw bank/card data never reaches Arcora.
        /// </summary>
        Task<CreateSetupIntentResponse> CreateSetupIntentAsync(CreateSetupIntentRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Saves a client-tokenized payment method. For PAD it also creates the ACSS mandate.
        /// Enforces that the backup method is a credit card.
        /// </summary>
        Task<PaymentMethodDto> SavePaymentMethodAsync(SavePaymentMethodRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns the tenant's onboarding readiness snapshot.
        /// </summary>
        Task<PaymentOnboardingStatusResponse> GetStatusAsync(Guid tenantId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Applies a provider verification outcome (from a SetupIntent/mandate webhook) to the stored
        /// payment method, transitioning its <c>VerificationStatus</c> to VERIFIED/PENDING/FAILED and,
        /// for PAD, activating the associated autopay mandate. Idempotent and safe to call repeatedly.
        /// </summary>
        /// <param name="providerPaymentMethodId">The provider payment method id (e.g. Stripe "pm_...").</param>
        /// <param name="providerStatus">The provider SetupIntent/mandate status (e.g. "succeeded", "active").</param>
        Task UpdateVerificationStatusAsync(string? providerPaymentMethodId, string? providerStatus, CancellationToken cancellationToken = default);
    }
}
