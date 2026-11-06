namespace Arcora.Api.DTOs;

public sealed record LeaseFirstPaymentDto(
    decimal RentAmount,
    decimal SecurityDepositAmount,
    decimal PlatformFeeAmount,
    decimal Amount,
    string Currency,
    string Status,
    Guid? PaymentIntentID,
    string? FailureReason);

public sealed record CollectLeaseFirstPaymentRequest(decimal ExpectedAmount);
