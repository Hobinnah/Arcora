namespace Arcora.Api.Services.Interfaces;

public interface IIdentityVerificationProviderFactory
{
    IIdentityVerificationProvider GetProvider(string? providerName = null);
    IReadOnlyCollection<string> GetAvailableProviders();
}
