using System.Net.Security;
using System.Net.Sockets;
using Arcora.Api.Configurations;

namespace Arcora.Api.Operations;

public sealed record KeyVaultHealthResult(string Status, string Description);

public sealed class KeyVaultConnectivity
{
    private readonly KeyVaultStartupState startup;
    private readonly TimeProvider time;
    private readonly ILogger<KeyVaultConnectivity> logger;
    private readonly Func<Uri, CancellationToken, Task> connect;
    private DateTimeOffset nextCheck;
    private DateTimeOffset? lastTransportCheck;
    private bool reachable;
    private readonly SemaphoreSlim gate = new(1, 1);

    public KeyVaultConnectivity(KeyVaultStartupState startup, TimeProvider time, ILogger<KeyVaultConnectivity> logger)
        : this(startup, time, logger, ConnectAsync) { }

    public KeyVaultConnectivity(KeyVaultStartupState startup, TimeProvider time, ILogger<KeyVaultConnectivity> logger,
        Func<Uri, CancellationToken, Task> connect)
    {
        this.startup = startup;
        this.time = time;
        this.logger = logger;
        this.connect = connect;
    }

    public async Task<KeyVaultHealthResult> CheckAsync(CancellationToken ct)
    {
        if (!startup.Enabled)
            return new("Disabled", "Key Vault startup loading is not enabled. Existing deployment configuration remains in use.");
        await gate.WaitAsync(ct);
        try
        {
            if (startup.EarliestExpiry <= time.GetUtcNow())
                return new("Failed", "At least one startup-loaded secret has expired. Rotate credentials and restart with valid secrets.");
            if (time.GetUtcNow() >= nextCheck)
            {
                try
                {
                    await connect(startup.VaultUri!, ct);
                    reachable = true;
                }
                catch (Exception ex) when (ex is SocketException or IOException or
                    System.Security.Authentication.AuthenticationException or OperationCanceledException)
                {
                    reachable = false;
                    logger.LogWarning("Key Vault transport check unavailable ({FailureKind}); startup-loaded secrets remain in memory.", ex.GetType().Name);
                }
                lastTransportCheck = time.GetUtcNow();
                nextCheck = lastTransportCheck.Value.AddSeconds(startup.ConnectivityIntervalSeconds);
            }
            if (!reachable)
                return new("Degraded", $"Key Vault transport is unavailable; using {startup.SecretCount} secrets loaded at {startup.LoadedAt:O}. Existing credentials remain in memory; no secret retrieval was attempted. Last transport check: {lastTransportCheck:O}.");
            if (startup.Source == "EnvironmentFallback")
                return new("Degraded", $"Key Vault transport is reachable, but this instance started using explicit environment fallback secrets at {startup.LoadedAt:O}. Validate access and restart to load vault values. No runtime rotation is performed.");
            return new("Healthy", $"{startup.SecretCount} secrets loaded from Key Vault at startup ({startup.LoadedAt:O}). Last DNS/TCP/certificate-validated TLS check: {lastTransportCheck:O}. Authentication is not rechecked; no runtime secret reads.");
        }
        finally
        {
            gate.Release();
        }
    }

    private static async Task ConnectAsync(Uri uri, CancellationToken ct)
    {
        using var tcp = new TcpClient();
        await tcp.ConnectAsync(uri.Host, uri.Port, ct);
        await using var ssl = new SslStream(tcp.GetStream());
        await ssl.AuthenticateAsClientAsync(new SslClientAuthenticationOptions { TargetHost = uri.Host }, ct);
    }
}
