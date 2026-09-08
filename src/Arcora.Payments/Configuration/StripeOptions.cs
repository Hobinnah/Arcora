namespace Arcora.Payments.Configuration
{
    /// <summary>
    /// Strongly typed Stripe configuration. Bound from the "Payments:Stripe" configuration section.
    /// Secrets should be supplied via user-secrets, environment variables or a secure vault - never checked in.
    /// </summary>
    public class StripeOptions
    {
        /// <summary>
        /// Configuration section name used to bind these options.
        /// </summary>
        public const string SectionName = "Stripe";

        /// <summary>
        /// Secret API key used for server-side calls (sk_live_ / sk_test_).
        /// </summary>
        public string SecretKey { get; set; } = string.Empty;

        /// <summary>
        /// Publishable key handed to the client SDK (pk_live_ / pk_test_).
        /// </summary>
        public string PublishableKey { get; set; } = string.Empty;

        /// <summary>
        /// Signing secret used to verify inbound webhook signatures (whsec_...).
        /// </summary>
        public string WebhookSecret { get; set; } = string.Empty;

        /// <summary>
        /// Default ISO currency used when a request does not specify one.
        /// </summary>
        public string DefaultCurrency { get; set; } = "cad";

        /// <summary>
        /// Provider name persisted on Arcora entities (ProviderName columns).
        /// </summary>
        public string ProviderName { get; set; } = "STRIPE";

        /// <summary>
        /// Statement descriptor shown on the tenant's bank/card statement.
        /// </summary>
        public string? StatementDescriptor { get; set; }

        /// <summary>
        /// When true (default), webhook parsing throws if the event's Stripe API version does not match the
        /// version the installed Stripe.net SDK targets. Set to false to tolerate a mismatch, but be aware
        /// that objects may be deserialized incorrectly. The preferred fix is to align the Stripe.net package
        /// version with the API version configured on your Stripe webhook endpoint.
        /// </summary>
        public bool ThrowOnApiVersionMismatch { get; set; } = true;
    }
}
