namespace Arcora.Payments.Abstractions
{
    /// <summary>
    /// Logical payment method kinds Arcora supports. PAD is the Canadian pre-authorized debit
    /// (Stripe "acss_debit"); Card is the mandatory backup credit card.
    /// </summary>
    public enum PaymentMethodKind
    {
        /// <summary>Canadian pre-authorized debit (ACSS).</summary>
        Pad,

        /// <summary>Credit card.</summary>
        Card
    }

    /// <summary>
    /// Classification of a failed collection attempt. Drives the retry/fallback state machine.
    /// </summary>
    public enum FailureCategory
    {
        /// <summary>No failure.</summary>
        None,

        /// <summary>Result is not yet final (e.g. ACSS still processing). Do NOT fall back yet.</summary>
        Pending,

        /// <summary>Transient failure (e.g. insufficient funds). A short PAD retry may succeed.</summary>
        Temporary,

        /// <summary>Definitive failure (e.g. account closed). Go straight to the backup card.</summary>
        Permanent
    }

    /// <summary>
    /// Represents a provider customer reference (Stripe Customer).
    /// </summary>
    public sealed record ProviderCustomer(string CustomerId);

    /// <summary>
    /// A SetupIntent created server-side. The client secret is handed to the client SDK to collect
    /// and confirm instrument details (e.g. ACSS debit) directly with the provider.
    /// </summary>
    public sealed record ProviderSetupIntent(
        string SetupIntentId,
        string ClientSecret,
        string Status);

    /// <summary>
    /// Result of setting up a saved payment method via a SetupIntent.
    /// Only opaque provider tokens are returned - Arcora never receives raw PAN/bank numbers.
    /// </summary>
    public sealed record ProviderPaymentMethodResult(
        string PaymentMethodId,
        PaymentMethodKind Kind,
        string? Brand,
        string? Last4,
        int? ExpiryMonth,
        int? ExpiryYear,
        string? BankName,
        string VerificationStatus,
        string? Funding);

    /// <summary>
    /// Result of creating/confirming a PAD (ACSS) mandate.
    /// </summary>
    public sealed record ProviderMandateResult(
        string MandateId,
        string PaymentMethodId,
        string Status);

    /// <summary>
    /// Result of a charge (PaymentIntent) attempt against the provider.
    /// </summary>
    public sealed record ProviderChargeResult(
        string PaymentIntentId,
        string? ChargeId,
        string Status,
        FailureCategory FailureCategory,
        string? FailureCode,
        string? FailureMessage);

    /// <summary>
    /// A normalized, provider-agnostic view of an inbound webhook event.
    /// </summary>
    public sealed record ProviderWebhookEvent(
        string EventId,
        string EventType,
        string RawPayload,
        string? PaymentIntentId,
        string? MandateId,
        string? ChargeId,
        string? Status,
        FailureCategory FailureCategory,
        string? FailureCode,
        string? FailureMessage,
        string? SetupIntentId = null,
        string? PaymentMethodId = null);
}
