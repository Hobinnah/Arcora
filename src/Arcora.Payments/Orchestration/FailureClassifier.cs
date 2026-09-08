using Arcora.Payments.Abstractions;

namespace Arcora.Payments.Orchestration
{
    /// <summary>
    /// Pure, side-effect free classification of provider failure codes into the categories that
    /// drive the rent-collection state machine. Kept separate so it can be exhaustively unit tested.
    ///
    /// Canadian PAD (ACSS) can take up to 5 business days to report a definitive result, so a
    /// "processing" style status MUST be treated as <see cref="FailureCategory.Pending"/> and must
    /// never trigger the card fallback (that would risk a double-charge).
    /// </summary>
    public static class FailureClassifier
    {
        // ACSS/bank decline codes that are transient - a short retry of the PAD may still succeed.
        private static readonly HashSet<string> TemporaryCodes = new(StringComparer.OrdinalIgnoreCase)
        {
            "insufficient_funds",
            "debit_not_authorized",
            "processing_error",
            "try_again_later",
            "issuer_unavailable",
            "temporary_hold"
        };

        // Codes for which retrying the PAD is pointless - go straight to the backup card.
        private static readonly HashSet<string> PermanentCodes = new(StringComparer.OrdinalIgnoreCase)
        {
            "account_closed",
            "no_account",
            "invalid_account_number",
            "bank_account_restricted",
            "account_frozen",
            "mandate_canceled",
            "payment_method_not_available",
            "currency_not_supported"
        };

        // Provider statuses that indicate the outcome is not yet final.
        private static readonly HashSet<string> PendingStatuses = new(StringComparer.OrdinalIgnoreCase)
        {
            "processing",
            "requires_action",
            "requires_confirmation",
            "requires_capture",
            "pending"
        };

        /// <summary>
        /// Classifies a provider status/failure code pair.
        /// </summary>
        public static FailureCategory Classify(string? status, string? failureCode)
        {
            if (!string.IsNullOrWhiteSpace(status))
            {
                if (status.Equals("succeeded", StringComparison.OrdinalIgnoreCase) ||
                    status.Equals("paid", StringComparison.OrdinalIgnoreCase))
                {
                    return FailureCategory.None;
                }

                if (PendingStatuses.Contains(status))
                {
                    return FailureCategory.Pending;
                }
            }

            if (!string.IsNullOrWhiteSpace(failureCode))
            {
                if (PermanentCodes.Contains(failureCode))
                {
                    return FailureCategory.Permanent;
                }

                if (TemporaryCodes.Contains(failureCode))
                {
                    return FailureCategory.Temporary;
                }
            }

            // Unknown failures are treated as permanent so we do not loop retrying the PAD forever;
            // the backup card becomes the safety net.
            return string.IsNullOrWhiteSpace(failureCode) && string.IsNullOrWhiteSpace(status)
                ? FailureCategory.None
                : FailureCategory.Permanent;
        }
    }
}
