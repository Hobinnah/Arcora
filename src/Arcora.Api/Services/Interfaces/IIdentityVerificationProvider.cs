namespace Arcora.Api.Services.Interfaces;

public sealed record IdentityVerificationProviderRequest(
    long UserId,
    string VerificationType,
    string ReferenceId,
    string? Email,
    string? FirstName,
    string? LastName,
    string? RedirectUrl);

public sealed record IdentityVerificationProviderResult(
    string ProviderName,
    string Status,
    string? ProviderReferenceId,
    string? VerificationUrl,
    string? ClientToken,
    DateTime? ExpiresAt = null,
    string? FailureReason = null);

public interface IIdentityVerificationProvider
{
    string ProviderName { get; }

    Task<IdentityVerificationProviderResult> StartVerificationAsync(
        IdentityVerificationProviderRequest request,
        CancellationToken cancellationToken = default);

    Task<IdentityVerificationProviderResult> GetVerificationStatusAsync(
        string providerReferenceId,
        CancellationToken cancellationToken = default);
}
