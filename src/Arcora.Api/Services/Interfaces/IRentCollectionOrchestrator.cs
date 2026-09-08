using Arcora.Api.DTOs;

namespace Arcora.Api.Services.Interfaces
{
    /// <summary>
    /// Orchestrates rent collection using the PAD-primary / credit-card-backup strategy.
    ///
    /// The flow deliberately never charges the backup card while a PAD (ACSS) charge is still
    /// processing - ACSS results can take up to 5 business days, and charging the card early risks a
    /// double-charge. The card fallback is only triggered on a definitive PAD failure.
    /// </summary>
    public interface IRentCollectionOrchestrator
    {
        /// <summary>
        /// Initiates collection for a due invoice: creates a PaymentIntent and attempts the PAD.
        /// Returns the resulting PaymentIntent. <paramref name="leaseId"/> may be null for the first
        /// charge taken at application approval, before a lease has been created.
        /// </summary>
        Task<PaymentIntentDto> InitiateCollectionAsync(Guid? leaseId, Guid tenantId, Guid invoiceMasterId, decimal amount, string? idempotencyKey = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Handles a definitive PAD result for an intent, applying the retry/fallback state machine.
        /// Called from webhook processing when the ACSS outcome becomes final.
        /// </summary>
        Task HandlePadResultAsync(Guid paymentIntentId, string providerStatus, string? failureCode, string? failureMessage, CancellationToken cancellationToken = default);

        /// <summary>
        /// Charges the backup credit card for an intent whose PAD has definitively failed.
        /// </summary>
        Task<bool> ChargeBackupCardAsync(Guid paymentIntentId, CancellationToken cancellationToken = default);
    }
}
