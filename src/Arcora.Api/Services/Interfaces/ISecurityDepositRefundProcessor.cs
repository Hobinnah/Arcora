namespace Arcora.Api.Services.Interfaces;

public sealed record SecurityDepositProcessorRefund(string ProviderRefundID, string? Status, string? FailureReason);

public interface ISecurityDepositRefundProcessor
{
    Task<SecurityDepositProcessorRefund> CreateRefundAsync(
        string providerPaymentReference,
        long amountInMinorUnits,
        Guid securityDepositID,
        string idempotencyKey,
        CancellationToken cancellationToken = default);
}