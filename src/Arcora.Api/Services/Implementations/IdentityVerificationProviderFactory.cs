using Arcora.Api.Configurations;
using Arcora.Api.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace Arcora.Api.Services.Implementations;

public sealed class IdentityVerificationProviderFactory : IIdentityVerificationProviderFactory
{
    private readonly IReadOnlyDictionary<string, IIdentityVerificationProvider> _providers;
    private readonly IdentityVerificationOptions _options;

    public IdentityVerificationProviderFactory(IEnumerable<IIdentityVerificationProvider> providers, IOptions<IdentityVerificationOptions> options)
    {
        _providers = providers.ToDictionary(provider => provider.ProviderName, StringComparer.OrdinalIgnoreCase);
        _options = options.Value ?? new IdentityVerificationOptions();
    }

    public IIdentityVerificationProvider GetProvider(string? providerName = null)
    {
        var resolvedProviderName = string.IsNullOrWhiteSpace(providerName)
            ? _options.DefaultProvider
            : providerName.Trim();

        if (_providers.TryGetValue(resolvedProviderName, out var provider))
            return provider;

        throw new InvalidOperationException($"Identity verification provider '{resolvedProviderName}' is not registered.");
    }

    public IReadOnlyCollection<string> GetAvailableProviders()
        => _providers.Keys.OrderBy(key => key, StringComparer.OrdinalIgnoreCase).ToArray();
}
