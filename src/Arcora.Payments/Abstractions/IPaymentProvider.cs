namespace Arcora.Payments.Abstractions
{
    /// <summary>
    /// Request to create a saved payment method (SetupIntent) for off-session future charges.
    /// </summary>
    public sealed record SetupPaymentMethodRequest(
        string CustomerId,
        PaymentMethodKind Kind,
        string PaymentMethodId,
        string? IdempotencyKey = null);

    /// <summary>
    /// Request to charge a saved payment method off-session.
    /// </summary>
    public sealed record CreateChargeRequest(
        string CustomerId,
        string PaymentMethodId,
        PaymentMethodKind Kind,
        long AmountInMinorUnits,
        string Currency,
        string IdempotencyKey,
        string? MandateId = null,
        string? Description = null,
        IReadOnlyDictionary<string, string>? Metadata = null);

    /// <summary>
    /// Provider-agnostic gateway abstraction. A single implementation (Stripe) exists today, but the
    /// abstraction keeps the rest of Arcora free of any provider SDK types and allows the provider to
    /// be swapped or extended without touching orchestration/persistence code.
    ///
    /// SECURITY: implementations must only ever exchange opaque provider tokens/identifiers. Raw card
    /// numbers and bank account details are collected client-side by the provider SDK and never reach
    /// Arcora's servers.
    /// </summary>
    public interface IPaymentProvider
    {
        /// <summary>Provider name persisted to entity ProviderName columns (e.g. "STRIPE").</summary>
        string ProviderName { get; }

        /// <summary>Gets an existing customer or creates one for the tenant.</summary>
        Task<ProviderCustomer> EnsureCustomerAsync(
            string? existingCustomerId,
            string tenantReference,
            string? email,
            string? name,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a SetupIntent for the given method kind and returns its client secret so the
        /// client SDK can collect and confirm instrument details (e.g. ACSS debit) directly with the
        /// provider. Only the client secret leaves Arcora - raw bank/card data never reaches the server.
        /// </summary>
        Task<ProviderSetupIntent> CreateSetupIntentAsync(
            string customerId,
            PaymentMethodKind kind,
            CancellationToken cancellationToken = default);

        /// <summary>Attaches/confirms a saved card via a SetupIntent for off-session use.</summary>
        Task<ProviderPaymentMethodResult> SetupPaymentMethodAsync(
            SetupPaymentMethodRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>Creates/confirms a Canadian PAD (ACSS) mandate for recurring debits.</summary>
        Task<ProviderMandateResult> CreatePadMandateAsync(
            string customerId,
            string paymentMethodId,
            string? idempotencyKey = null,
            CancellationToken cancellationToken = default);

        /// <summary>Charges a saved payment method off-session (PAD primary or card fallback).</summary>
        Task<ProviderChargeResult> CreateChargeAsync(
            CreateChargeRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>Verifies the webhook signature and returns a normalized event.</summary>
        ProviderWebhookEvent ConstructWebhookEvent(string requestBody, string signatureHeader);
    }
}
