namespace Arcora.Payments.Abstractions;

public sealed record HostPayoutRequest(
    string Reference, string AccountId, string PaymentIntentId, long Amount, long CollectedAmount, string Currency);
public sealed record HostPayoutResult(
    string? TransferId, string? PayoutId, string Status, string? FailureReason = null);

public interface IHostPayoutProvider
{
    Task<HostPayoutResult> ProgressAsync(HostPayoutRequest request, bool allowTransferCreate, bool allowPayoutCreate,
        CancellationToken cancellationToken = default);
}
