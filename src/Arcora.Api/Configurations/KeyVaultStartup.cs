using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using Azure;
using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace Arcora.Api.Configurations;

public sealed class KeyVaultStartupOptions
{
    public bool Enabled { get; set; }
    public string? VaultUri { get; set; }
    public string? ManagedIdentityClientId { get; set; }
    public bool AllowEnvironmentFallback { get; set; }
    public int StartupTimeoutSeconds { get; set; } = 30;
    public int ConnectivityIntervalSeconds { get; set; } = 300;
    public List<KeyVaultSecretMapping> Secrets { get; set; } = [];
}

public sealed class KeyVaultSecretMapping
{
    public string ConfigurationKey { get; set; } = "";
    public string SecretName { get; set; } = "";
}

public sealed record KeyVaultStartupState(
    bool Enabled, string Source, Uri? VaultUri, DateTime LoadedAt, DateTimeOffset? EarliestExpiry,
    int SecretCount, int ConnectivityIntervalSeconds)
{
    public static KeyVaultStartupState Disabled => new(false, "ExistingConfiguration", null, DateTime.UtcNow, null, 0, 300);
}

public static class KeyVaultStartup
{
    public const string FallbackPrefix = "KeyVaultFallback__";
    public const string ReferencePrefix = "akv:";

    public static async Task<KeyVaultStartupState> LoadAsync(
    ConfigurationManager configuration, IHostEnvironment environment, ILogger logger,
    CancellationToken cancellationToken = default)
    {
        var options = configuration.GetSection("Azure:KeyVault").Get<KeyVaultStartupOptions>()
            ?? configuration.GetSection("KeyVault").Get<KeyVaultStartupOptions>()
            ?? new();

        return await LoadAsync(configuration, options, environment, logger,
            (uri, settings) => CreateClient(uri, settings, environment), Environment.GetEnvironmentVariable, cancellationToken);
    }

    public static async Task<KeyVaultStartupState> LoadAsync(
        ConfigurationManager configuration, KeyVaultStartupOptions options, IHostEnvironment environment, ILogger logger,
        Func<Uri, KeyVaultStartupOptions, SecretClient> clientFactory, Func<string, string?> fallbackValue,
        CancellationToken cancellationToken = default)
    {
        var references = configuration.AsEnumerable()
            .Where(entry => entry.Value?.StartsWith(ReferencePrefix, StringComparison.OrdinalIgnoreCase) == true)
            .Select(entry => new KeyVaultSecretMapping
            {
                ConfigurationKey = entry.Key,
                SecretName = entry.Value![ReferencePrefix.Length..]
            }).ToList();
        if (!options.Enabled)
        {
            if (references.Count > 0)
                throw new InvalidOperationException("Key Vault references require KeyVault:Enabled to be true. Startup refused.");
            return KeyVaultStartupState.Disabled;
        }

        var mappings = (options.Secrets ?? []).Concat(references).ToList();
        var uri = Validate(options, mappings);
        cancellationToken.ThrowIfCancellationRequested();
        var client = clientFactory(uri, options);
        var values = new ConcurrentDictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
        var failures = new ConcurrentQueue<Exception>();
        DateTimeOffset? earliestExpiry = null;
        var expiryGate = new object();
        var now = DateTimeOffset.UtcNow;
        using var deadline = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        deadline.CancelAfter(TimeSpan.FromSeconds(options.StartupTimeoutSeconds));
        try
        {
            await Parallel.ForEachAsync(mappings,
                new ParallelOptions { MaxDegreeOfParallelism = 4, CancellationToken = deadline.Token },
                async (mapping, ct) =>
                {
                    try
                    {
                        var secret = (await client.GetSecretAsync(mapping.SecretName, cancellationToken: ct)).Value;
                        if (string.IsNullOrWhiteSpace(secret.Value) || secret.Properties.Enabled == false ||
                            secret.Properties.ExpiresOn <= now || secret.Properties.NotBefore > now)
                            throw new InvalidOperationException("A mapped Key Vault secret is empty, disabled, expired or not yet valid. Startup refused.");
                        values[mapping.ConfigurationKey] = secret.Value;
                        lock (expiryGate)
                        {
                            if (secret.Properties.ExpiresOn.HasValue &&
                                (!earliestExpiry.HasValue || secret.Properties.ExpiresOn < earliestExpiry))
                                earliestExpiry = secret.Properties.ExpiresOn;
                        }
                    }
                    catch (Exception ex) when (ex is RequestFailedException or AuthenticationFailedException or
                        HttpRequestException or OperationCanceledException or InvalidOperationException)
                    {
                        failures.Enqueue(ex);
                    }
                });
            var rejected = failures.FirstOrDefault(failure => !IsUnavailable(failure));
            if (rejected != null)
                throw rejected;
            if (failures.TryPeek(out var failure))
                throw failure;
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex) when (IsUnavailable(ex) && failures.All(IsUnavailable))
        {
            if (!options.AllowEnvironmentFallback)
            {
                logger.LogCritical("Key Vault startup retrieval unavailable ({FailureKind}). Environment fallback is disabled; startup refused.", FailureKind(ex));
                throw new InvalidOperationException("Key Vault is unavailable and fallback is disabled. Startup refused.");
            }
            values.Clear();
            foreach (var key in mappings.Select(mapping => mapping.ConfigurationKey))
            {
                var value = fallbackValue(FallbackPrefix + key.Replace(":", "__", StringComparison.Ordinal));
                if (string.IsNullOrWhiteSpace(value))
                {
                    logger.LogCritical("Key Vault is unavailable and the explicit environment fallback set is incomplete. Startup refused.");
                    throw new InvalidOperationException("Key Vault is unavailable and required explicit environment fallback secrets are missing. Startup refused.");
                }
                values[key] = value;
            }
            cancellationToken.ThrowIfCancellationRequested();
            configuration.AddInMemoryCollection(values);
            logger.LogWarning("Key Vault is unavailable ({FailureKind}). Using the complete explicit environment fallback secret set in process memory. No secrets were written to disk.", FailureKind(ex));
            return new(true, "EnvironmentFallback", uri, DateTime.UtcNow, null, values.Count, options.ConnectivityIntervalSeconds);
        }
        catch (Exception ex) when (ex is RequestFailedException or AuthenticationFailedException or InvalidOperationException or OperationCanceledException)
        {
            logger.LogCritical("Key Vault startup retrieval rejected ({FailureKind}). No secrets were applied; startup refused.", FailureKind(ex));
            // Do not retain the provider exception: it may contain request URLs, secret names or response bodies.
            throw new InvalidOperationException("Key Vault secret retrieval or validation failed. Check identity permissions, mappings and secret validity. Startup refused.");
        }
        cancellationToken.ThrowIfCancellationRequested();
        if (earliestExpiry <= DateTimeOffset.UtcNow)
            throw new InvalidOperationException("A mapped Key Vault secret expired during startup. No secrets were applied; startup refused.");
        configuration.AddInMemoryCollection(values);
        logger.LogInformation("Loaded {SecretCount} mapped secrets from Key Vault into process memory at startup. Runtime requests do not retrieve secrets.", values.Count);
        return new(true, "KeyVault", uri, DateTime.UtcNow, earliestExpiry, values.Count, options.ConnectivityIntervalSeconds);
    }

    private static SecretClient CreateClient(Uri uri, KeyVaultStartupOptions options, IHostEnvironment environment)
    {
        TokenCredential credential = environment.IsDevelopment()
            ? new DefaultAzureCredential(new DefaultAzureCredentialOptions
            {
                ManagedIdentityClientId = options.ManagedIdentityClientId,
                ExcludeInteractiveBrowserCredential = true
            })
            : string.IsNullOrWhiteSpace(options.ManagedIdentityClientId)
                ? new ManagedIdentityCredential()
                : new ManagedIdentityCredential(options.ManagedIdentityClientId);
        var clientOptions = new SecretClientOptions();
        clientOptions.Retry.MaxRetries = 2;
        clientOptions.Retry.Delay = TimeSpan.FromMilliseconds(250);
        clientOptions.Retry.MaxDelay = TimeSpan.FromSeconds(2);
        clientOptions.Retry.NetworkTimeout = TimeSpan.FromSeconds(5);
        clientOptions.Diagnostics.IsLoggingEnabled = false;
        clientOptions.Diagnostics.IsLoggingContentEnabled = false;
        return new SecretClient(uri, credential, clientOptions);
    }

    private static Uri Validate(KeyVaultStartupOptions options, List<KeyVaultSecretMapping> mappings)
    {
        if (!Uri.TryCreate(options.VaultUri, UriKind.Absolute, out var uri) || uri.Scheme != "https" ||
            !uri.IsDefaultPort || !string.IsNullOrEmpty(uri.UserInfo) || !string.IsNullOrEmpty(uri.Query) ||
            !string.IsNullOrEmpty(uri.Fragment) || uri.AbsolutePath != "/" ||
            !new[] { ".vault.azure.net", ".vault.azure.cn", ".vault.usgovcloudapi.net" }
                .Any(suffix => uri.Host.EndsWith(suffix, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException("KeyVault:VaultUri must be an HTTPS Azure Key Vault root URI.");
        if (options.StartupTimeoutSeconds is < 1 or > 120 || options.ConnectivityIntervalSeconds is < 60 or > 3600)
            throw new InvalidOperationException("Key Vault timeout must be 1-120 seconds and connectivity interval 60-3600 seconds.");
        if (options.ManagedIdentityClientId != null && !Guid.TryParse(options.ManagedIdentityClientId, out _))
            throw new InvalidOperationException("KeyVault:ManagedIdentityClientId must be a client ID GUID.");
        if (mappings.Count is < 1 or > 100)
            throw new InvalidOperationException("Key Vault requires 1-100 configuration references or explicit secret mappings.");
        var keys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var mapping in mappings)
        {
            if (mapping == null || !Regex.IsMatch(mapping.ConfigurationKey ?? "", @"^[A-Za-z0-9]+(?::[A-Za-z0-9]+)+$") ||
                mapping.ConfigurationKey!.StartsWith("KeyVault:", StringComparison.OrdinalIgnoreCase) ||
                mapping.ConfigurationKey.StartsWith("KeyVaultFallback:", StringComparison.OrdinalIgnoreCase) ||
                !keys.Add(mapping.ConfigurationKey) || !Regex.IsMatch(mapping.SecretName ?? "", "^[A-Za-z0-9-]{1,127}$"))
                throw new InvalidOperationException("Key Vault mappings contain an invalid or duplicate configuration key or secret name.");
        }
        return uri;
    }

    private static bool IsUnavailable(Exception ex) => ex is HttpRequestException or OperationCanceledException ||
        ex is RequestFailedException request && (request.Status is 0 or 408 or 429 || request.Status >= 500);

    private static string FailureKind(Exception ex) =>
        ex is RequestFailedException request ? $"HTTP {request.Status}" : ex.GetType().Name;
}
