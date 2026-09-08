using System.ComponentModel.DataAnnotations;

namespace Arcora.Api.DTOs
{
    /// <summary>
    /// Request to begin payment onboarding for a tenant. Ensures a provider customer exists so the
    /// client SDK can collect instrument details directly with the provider (Arcora never sees raw
    /// card/bank numbers).
    /// </summary>
    public class PaymentOnboardingStartRequest
    {
        [Required]
        public Guid TenantID { get; set; }
    }

    /// <summary>
    /// Response returned to the client to initialize the provider SDK.
    /// </summary>
    public class PaymentOnboardingStartResponse
    {
        public Guid TenantID { get; set; }
        public string? ProviderCustomerID { get; set; }
        public string? PublishableKey { get; set; }
        public string? ProviderName { get; set; }
    }

    /// <summary>
    /// Request to create a provider SetupIntent so the client SDK can collect and confirm instrument
    /// details (e.g. ACSS debit) directly with the provider. Returns a client secret only.
    /// </summary>
    public class CreateSetupIntentRequest
    {
        [Required]
        public Guid TenantID { get; set; }

        /// <summary>
        /// "PAD" for the primary Canadian pre-authorized debit, or "CARD" for the backup credit card.
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string MethodKind { get; set; } = string.Empty;
    }

    /// <summary>
    /// Response containing the SetupIntent client secret needed by the client SDK to confirm the setup.
    /// </summary>
    public class CreateSetupIntentResponse
    {
        public Guid TenantID { get; set; }
        public string? SetupIntentID { get; set; }
        public string? ClientSecret { get; set; }
        public string? PublishableKey { get; set; }
        public string? Status { get; set; }
    }

    /// <summary>
    /// Request to save a client-tokenized payment method (card backup or PAD primary).
    /// Only the opaque provider payment method token is transmitted.
    /// </summary>
    public class SavePaymentMethodRequest
    {
        [Required]
        public Guid TenantID { get; set; }

        /// <summary>
        /// Opaque provider payment method id (e.g. Stripe "pm_..."), tokenized client-side.
        /// </summary>
        [Required]
        [MaxLength(255)]
        public string ProviderPaymentMethodID { get; set; } = string.Empty;

        /// <summary>
        /// "PAD" for the primary Canadian pre-authorized debit, or "CARD" for the backup credit card.
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string MethodKind { get; set; } = string.Empty;
    }

    /// <summary>
    /// Snapshot of a tenant's payment onboarding readiness.
    /// </summary>
    public class PaymentOnboardingStatusResponse
    {
        public Guid TenantID { get; set; }
        public bool HasVerifiedPad { get; set; }
        public bool HasVerifiedCard { get; set; }
        public bool PadMandateActive { get; set; }

        /// <summary>
        /// True only when the tenant has a verified PAD, an active mandate and a verified backup credit card.
        /// </summary>
        public bool IsReady => HasVerifiedPad && HasVerifiedCard && PadMandateActive;
    }
}
