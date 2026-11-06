using System.Collections.Concurrent;
using System.Net;
using Arcora.Api.Configurations;
using Arcora.Api.Controllers;
using Arcora.Api.Operations;
using Azure;
using Azure.Core;
using Azure.Core.Pipeline;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Arcora.Api.Tests;

public sealed class KeyVaultStartupTests
{
    private static KeyVaultStartupOptions Settings() => new()
    {
        Enabled = true, VaultUri = "https://synthetic.vault.azure.net", AllowEnvironmentFallback = true,
        Secrets =
        [
            new() { ConfigurationKey = "Stripe:SecretKey", SecretName = "synthetic-stripe" },
            new() { ConfigurationKey = "ConnectionStrings:DefaultAppConnection", SecretName = "synthetic-sql" }
        ]
    };

    private static Task<KeyVaultStartupState> Load(
        ConfigurationManager config, KeyVaultStartupOptions settings, SecretClient client,
        Func<string, string?>? fallback = null, ILogger? logger = null, CancellationToken ct = default) =>
        KeyVaultStartup.LoadAsync(config, settings, new TestEnvironment(), logger ?? NullLogger.Instance,
            (_, _) => client, fallback ?? (_ => null), ct);

    [Fact]
    public async Task Disabled_feature_leaves_existing_configuration_untouched_and_never_creates_client()
    {
        var config = new ConfigurationManager();
        config["Stripe:SecretKey"] = "existing-synthetic";
        var result = await KeyVaultStartup.LoadAsync(config, new(), new TestEnvironment(), NullLogger.Instance,
            (_, _) => throw new InvalidOperationException("Client should never be created"), _ => throw new InvalidOperationException());
        Assert.False(result.Enabled);
        Assert.Equal("existing-synthetic", config["Stripe:SecretKey"]);
    }

    [Fact]
    public async Task Inline_references_resolve_at_startup_without_a_mapping_array_and_preserve_literals()
    {
        var config = new ConfigurationManager();
        config["ConnectionStrings:DefaultAppConnection"] = "akv:synthetic-sql";
        config["Stripe:SecretKey"] = "AKV:synthetic-stripe";
        config["Stripe:Currency"] = "cad";
        var settings = Settings();
        settings.Secrets.Clear();
        var client = new TestClient((name, _) =>
        {
            Assert.Equal("akv:synthetic-sql", config["ConnectionStrings:DefaultAppConnection"]);
            return Task.FromResult(new KeyVaultSecret(name, $"private-{name}"));
        });

        var state = await Load(config, settings, client);

        Assert.Equal(2, state.SecretCount);
        Assert.Equal("private-synthetic-sql", config["ConnectionStrings:DefaultAppConnection"]);
        Assert.Equal("private-synthetic-stripe", config["Stripe:SecretKey"]);
        Assert.Equal("cad", config["Stripe:Currency"]);
        Assert.Empty(settings.Secrets);
        for (var i = 0; i < 100; i++) Assert.NotNull(config["Stripe:SecretKey"]);
        Assert.Equal(2, client.Reads);
    }

    [Fact]
    public async Task Effective_provider_override_replaces_a_reference_before_discovery()
    {
        var config = new ConfigurationManager();
        config.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Stripe:SecretKey"] = "akv:unused-secret",
            ["Authentication:JwtSettings:Key"] = "akv:synthetic-jwt"
        });
        config.AddInMemoryCollection(new Dictionary<string, string?> { ["Stripe:SecretKey"] = "literal-override" });
        var settings = Settings();
        settings.Secrets.Clear();
        var client = new TestClient((name, _) =>
        {
            Assert.Equal("synthetic-jwt", name);
            return Task.FromResult(new KeyVaultSecret(name, "private-jwt"));
        });

        await Load(config, settings, client);

        Assert.Equal("literal-override", config["Stripe:SecretKey"]);
        Assert.Equal(1, client.Reads);
    }

    [Fact]
    public async Task Disabled_feature_rejects_unresolved_references_before_creating_a_client()
    {
        var config = new ConfigurationManager();
        config["Stripe:SecretKey"] = "akv:synthetic-stripe";
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            KeyVaultStartup.LoadAsync(config, new(), new TestEnvironment(), NullLogger.Instance,
                (_, _) => throw new InvalidOperationException("Client should never be created"), _ => null));
        Assert.Equal("akv:synthetic-stripe", config["Stripe:SecretKey"]);
    }

    [Theory]
    [InlineData("Stripe:SecretKey", "akv:")]
    [InlineData("Stripe:SecretKey", "akv:invalid_name")]
    [InlineData("Stripe:SecretKey", "akv:secret/version")]
    [InlineData("KeyVault:ManagedIdentityClientId", "akv:synthetic-client")]
    [InlineData("KeyVaultFallback:Stripe:SecretKey", "akv:synthetic-fallback")]
    public async Task Invalid_or_bootstrap_references_fail_before_creating_a_client(string key, string value)
    {
        var config = new ConfigurationManager();
        config[key] = value;
        var settings = Settings();
        settings.Secrets.Clear();
        var created = false;
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            KeyVaultStartup.LoadAsync(config, settings, new TestEnvironment(), NullLogger.Instance,
                (_, _) =>
                {
                    created = true;
                    throw new InvalidOperationException("Must not create a client");
                }, _ => null));
        Assert.False(created);
        Assert.Equal(value, config[key]);
    }

    [Fact]
    public async Task Inline_references_and_legacy_mappings_share_atomic_fallback()
    {
        var config = new ConfigurationManager();
        config["Authentication:JwtSettings:Key"] = "akv:synthetic-jwt";
        var client = new TestClient((name, _) => name == "synthetic-jwt"
            ? throw new RequestFailedException(503, "outage")
            : Task.FromResult(new KeyVaultSecret(name, "partial-vault-value")));
        var keys = new List<string>();
        var state = await Load(config, Settings(), client, key =>
        {
            keys.Add(key);
            return "explicit-fallback";
        });

        Assert.Equal("EnvironmentFallback", state.Source);
        Assert.Equal(3, state.SecretCount);
        Assert.Contains("KeyVaultFallback__Authentication__JwtSettings__Key", keys);
        Assert.Equal("explicit-fallback", config["Authentication:JwtSettings:Key"]);
        Assert.Equal("explicit-fallback", config["Stripe:SecretKey"]);
        Assert.Equal("explicit-fallback", config["ConnectionStrings:DefaultAppConnection"]);
    }

    [Fact]
    public async Task Inline_reference_cannot_conflict_with_a_legacy_mapping()
    {
        var config = new ConfigurationManager();
        config["stripe:secretkey"] = "akv:another-secret";
        var client = new TestClient((name, _) => Task.FromResult(new KeyVaultSecret(name, "private-value")));
        await Assert.ThrowsAsync<InvalidOperationException>(() => Load(config, Settings(), client));
        Assert.Equal(0, client.Reads);
    }

    [Fact]
    public async Task Loads_only_explicit_mappings_once_and_publishes_all_values_atomically()
    {
        var config = new ConfigurationManager();
        config["Stripe:SecretKey"] = "old";
        var client = new TestClient((name, _) =>
        {
            Assert.Equal("old", config["Stripe:SecretKey"]);
            return Task.FromResult(new KeyVaultSecret(name, $"private-{name}"));
        });
        var result = await Load(config, Settings(), client, _ => throw new InvalidOperationException("Must not use fallback"));
        Assert.Equal("KeyVault", result.Source);
        Assert.Equal(2, result.SecretCount);
        Assert.Equal(2, client.Reads);
        Assert.Equal("private-synthetic-stripe", config["Stripe:SecretKey"]);
        Assert.Equal("private-synthetic-sql", config["ConnectionStrings:DefaultAppConnection"]);
        for (var i = 0; i < 100; i++) Assert.NotNull(config["Stripe:SecretKey"]);
        Assert.Equal(2, client.Reads);
        Assert.DoesNotContain("private-", System.Text.Json.JsonSerializer.Serialize(result));
    }

    [Fact]
    public async Task Real_sdk_issues_authenticated_secret_gets_only_and_deserializes_expiry_metadata()
    {
        using var handler = new VaultHttp();
        using var http = new HttpClient(handler);
        var sdkOptions = new SecretClientOptions { Transport = new HttpClientTransport(http) };
        sdkOptions.Retry.MaxRetries = 0;
        var sdk = new SecretClient(new("https://synthetic.vault.azure.net"), new SyntheticCredential(), sdkOptions);
        var config = new ConfigurationManager();
        var state = await Load(config, Settings(), sdk);
        Assert.Equal("KeyVault", state.Source);
        Assert.NotNull(state.EarliestExpiry);
        Assert.InRange(handler.Requests.Count, 3, 4);
        Assert.Equal(2, handler.AuthenticatedReads);
        Assert.All(handler.Requests, request =>
        {
            Assert.Equal(HttpMethod.Get, request.Method);
            Assert.StartsWith("/secrets/synthetic-", request.Path);
        });
        Assert.Equal("private-sdk-value", config["Stripe:SecretKey"]);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(408)]
    [InlineData(429)]
    [InlineData(500)]
    [InlineData(503)]
    public async Task Outage_uses_complete_explicit_environment_fallback_and_never_partial_vault_values(int status)
    {
        var config = new ConfigurationManager();
        var keys = new ConcurrentBag<string>();
        var client = new TestClient((name, _) => name == "synthetic-stripe"
            ? Task.FromResult(new KeyVaultSecret(name, "partial-private-vault-value"))
            : throw new RequestFailedException(status, "private-provider-body"));
        var logs = new CaptureLogger();
        var result = await Load(config, Settings(), client, name =>
        {
            keys.Add(name);
            return "explicit-fallback";
        }, logs);
        Assert.Equal("EnvironmentFallback", result.Source);
        Assert.Equal("explicit-fallback", config["Stripe:SecretKey"]);
        Assert.Equal("explicit-fallback", config["ConnectionStrings:DefaultAppConnection"]);
        Assert.Contains("KeyVaultFallback__Stripe__SecretKey", keys);
        Assert.Contains("KeyVaultFallback__ConnectionStrings__DefaultAppConnection", keys);
        Assert.DoesNotContain("private-", string.Join("", logs.Messages));
        Assert.Contains(logs.Messages, message => message.Contains("environment fallback"));
    }

    [Theory]
    [InlineData(401)]
    [InlineData(403)]
    [InlineData(404)]
    [InlineData(400)]
    public async Task Rejected_vault_requests_fail_closed_instead_of_using_fallback(int status)
    {
        var config = new ConfigurationManager();
        config["Stripe:SecretKey"] = "unchanged";
        var logs = new CaptureLogger();
        var client = new TestClient((_, _) => throw new RequestFailedException(status, "private-provider-body"));
        var error = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            Load(config, Settings(), client, _ => throw new InvalidOperationException("Fallback must not be read"), logs));
        Assert.Equal("unchanged", config["Stripe:SecretKey"]);
        Assert.DoesNotContain("private-", error.ToString());
        Assert.DoesNotContain("private-", string.Join("", logs.Messages));
    }

    [Theory]
    [InlineData("expired")]
    [InlineData("disabled")]
    [InlineData("future")]
    [InlineData("blank")]
    public async Task Invalid_secret_metadata_or_value_is_not_cached_or_replaced_with_fallback(string kind)
    {
        var config = new ConfigurationManager();
        config["Stripe:SecretKey"] = "unchanged";
        var client = new TestClient((name, _) =>
        {
            var value = new KeyVaultSecret(name, kind == "blank" ? " " : "private-synthetic");
            if (kind == "expired") value.Properties.ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(-1);
            if (kind == "future") value.Properties.NotBefore = DateTimeOffset.UtcNow.AddHours(1);
            if (kind == "disabled") value.Properties.Enabled = false;
            return Task.FromResult(value);
        });
        await Assert.ThrowsAsync<InvalidOperationException>(() => Load(config, Settings(), client, _ => "fallback"));
        Assert.Equal("unchanged", config["Stripe:SecretKey"]);
    }

    [Fact]
    public async Task Missing_fallback_does_not_use_normal_appsettings_as_an_implicit_backup()
    {
        var config = new ConfigurationManager();
        config["Stripe:SecretKey"] = "appsettings-existing";
        config["ConnectionStrings:DefaultAppConnection"] = "appsettings-existing";
        var client = new TestClient((_, _) => throw new HttpRequestException("private-provider-body"));
        await Assert.ThrowsAsync<InvalidOperationException>(() => Load(config, Settings(), client,
            key => key.Contains("Stripe") ? "partial-fallback" : null));
        Assert.Equal("appsettings-existing", config["Stripe:SecretKey"]);
        Assert.Equal("appsettings-existing", config["ConnectionStrings:DefaultAppConnection"]);
    }

    [Fact]
    public async Task Fallback_can_be_disabled_and_cancellation_is_never_converted_to_success()
    {
        var config = new ConfigurationManager();
        var settings = Settings();
        settings.AllowEnvironmentFallback = false;
        var client = new TestClient((_, _) => throw new HttpRequestException("Synthetic outage"));
        await Assert.ThrowsAsync<InvalidOperationException>(() => Load(config, settings, client, _ => "fallback"));
        settings.AllowEnvironmentFallback = true;
        using var cancel = new CancellationTokenSource();
        cancel.Cancel();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => Load(config, settings, client, _ => "fallback", ct: cancel.Token));
        Assert.Null(config["Stripe:SecretKey"]);
    }

    [Fact]
    public async Task Bounded_startup_timeout_can_fall_back_without_waiting_indefinitely()
    {
        var settings = Settings();
        settings.StartupTimeoutSeconds = 1;
        var config = new ConfigurationManager();
        var client = new TestClient(async (name, ct) =>
        {
            await Task.Delay(Timeout.InfiniteTimeSpan, ct);
            return new(name, "never-returned");
        });
        var result = await Load(config, settings, client, _ => "explicit-fallback").WaitAsync(TimeSpan.FromSeconds(5));
        Assert.Equal("EnvironmentFallback", result.Source);
    }

    [Fact]
    public async Task Credential_failure_and_concurrent_permission_failure_cannot_be_masked_as_network_outage()
    {
        var config = new ConfigurationManager();
        var client = new TestClient((_, _) => throw new AuthenticationFailedException("private-identity-detail"));
        var error = await Assert.ThrowsAsync<InvalidOperationException>(() => Load(config, Settings(), client, _ => "fallback"));
        Assert.DoesNotContain("private-", error.ToString());
        client = new TestClient((name, _) => name == "synthetic-stripe"
            ? throw new RequestFailedException(503, "outage")
            : throw new RequestFailedException(403, "private-permission-detail"));
        await Assert.ThrowsAsync<InvalidOperationException>(() => Load(config, Settings(), client, _ => "fallback"));
        Assert.Null(config["Stripe:SecretKey"]);
    }

    [Theory]
    [InlineData("http://synthetic.vault.azure.net")]
    [InlineData("https://untrusted.example.test")]
    [InlineData("https://synthetic.vault.azure.net/path")]
    [InlineData("https://synthetic.vault.azure.net?secret=private")]
    [InlineData("https://user:password@synthetic.vault.azure.net")]
    public async Task Invalid_vault_endpoints_are_rejected_before_credentials_are_requested(string url)
    {
        var settings = Settings();
        settings.VaultUri = url;
        var client = new TestClient((_, _) => throw new InvalidOperationException("Must never fetch"));
        await Assert.ThrowsAsync<InvalidOperationException>(() => Load(new(), settings, client));
        Assert.Equal(0, client.Reads);
    }

    [Fact]
    public async Task Options_bind_real_nested_configuration_keys_and_reject_duplicate_mappings()
    {
        var config = new ConfigurationManager();
        config.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["KeyVault:Enabled"] = "true",
            ["KeyVault:VaultUri"] = "https://synthetic.vault.azure.net",
            ["KeyVault:Secrets:0:ConfigurationKey"] = "Stripe:SecretKey",
            ["KeyVault:Secrets:0:SecretName"] = "synthetic-stripe"
        });
        var settings = config.GetSection("KeyVault").Get<KeyVaultStartupOptions>()!;
        var client = new TestClient((name, _) => Task.FromResult(new KeyVaultSecret(name, "private-synthetic")));
        await Load(config, settings, client);
        Assert.Equal("private-synthetic", config["Stripe:SecretKey"]);
        settings.Secrets.Add(new() { ConfigurationKey = "stripe:secretkey", SecretName = "duplicate" });
        await Assert.ThrowsAsync<InvalidOperationException>(() => Load(config, settings, client));
        Assert.Equal(1, client.Reads);
    }

    [Fact]
    public async Task Connectivity_checks_are_cached_and_never_refetch_or_replace_loaded_secrets()
    {
        var clock = new Clock();
        var config = new ConfigurationManager();
        var client = new TestClient((name, _) => Task.FromResult(new KeyVaultSecret(name, "private-value")));
        var state = await Load(config, Settings(), client);
        var connections = 0;
        var reachable = true;
        var health = new KeyVaultConnectivity(state, clock, NullLogger<KeyVaultConnectivity>.Instance, (_, _) =>
        {
            connections++;
            if (!reachable) throw new IOException("private-transport");
            return Task.CompletedTask;
        });
        Assert.Equal("Healthy", (await health.CheckAsync(default)).Status);
        for (var i = 0; i < 100; i++) await health.CheckAsync(default);
        Assert.Equal(1, connections);
        reachable = false;
        clock.Advance(TimeSpan.FromMinutes(6));
        Assert.Equal("Degraded", (await health.CheckAsync(default)).Status);
        Assert.Equal("private-value", config["Stripe:SecretKey"]);
        Assert.Equal(2, client.Reads);
        reachable = true;
        clock.Advance(TimeSpan.FromMinutes(6));
        Assert.Equal("Healthy", (await health.CheckAsync(default)).Status);
        Assert.Equal(2, client.Reads);
    }

    [Fact]
    public async Task Fallback_remains_degraded_after_transport_recovers_until_an_explicit_restart()
    {
        var state = new KeyVaultStartupState(true, "EnvironmentFallback", new("https://synthetic.vault.azure.net"),
            DateTime.UtcNow, null, 2, 300);
        var health = new KeyVaultConnectivity(state, TimeProvider.System, NullLogger<KeyVaultConnectivity>.Instance, (_, _) => Task.CompletedTask);
        var result = await health.CheckAsync(default);
        Assert.Equal("Degraded", result.Status);
        Assert.Contains("restart", result.Description);
    }

    [Fact]
    public async Task Expired_cached_secret_fails_health_and_readiness_but_outage_with_valid_cache_does_not()
    {
        var clock = new Clock();
        var state = new KeyVaultStartupState(true, "KeyVault", new("https://synthetic.vault.azure.net"),
            DateTime.UtcNow, clock.GetUtcNow().AddMinutes(1), 2, 300);
        var health = new KeyVaultConnectivity(state, clock, NullLogger<KeyVaultConnectivity>.Instance,
            (_, _) => throw new IOException("Synthetic offline"));
        var registry = new OperationsRegistry(Options.Create(new OperationsOptions()), clock);
        registry.Report("dependency.database", "SQL", "Infrastructure", "Healthy", "Ready.");
        var result = await health.CheckAsync(default);
        registry.Report("dependency.key-vault", "Key Vault", "Infrastructure", result.Status, result.Description);
        Assert.Equal(200, Assert.IsType<ObjectResult>(new HealthProbeController(registry).Ready()).StatusCode);
        clock.Advance(TimeSpan.FromMinutes(2));
        registry.Report("dependency.database", "SQL", "Infrastructure", "Healthy", "Ready.");
        result = await health.CheckAsync(default);
        registry.Report("dependency.key-vault", "Key Vault", "Infrastructure", result.Status, result.Description);
        Assert.Equal("Failed", result.Status);
        Assert.Equal(503, Assert.IsType<ObjectResult>(new HealthProbeController(registry).Ready()).StatusCode);
    }

    private sealed class TestClient(Func<string, CancellationToken, Task<KeyVaultSecret>> read) : SecretClient
    {
        private int reads;
        public int Reads => Volatile.Read(ref reads);
        public override async Task<Response<KeyVaultSecret>> GetSecretAsync(string name, string? version = null, CancellationToken cancellationToken = default)
        {
            Interlocked.Increment(ref reads);
            return Response.FromValue(await read(name, cancellationToken), new TestResponse());
        }
    }

    private sealed class SyntheticCredential : TokenCredential
    {
        public override AccessToken GetToken(TokenRequestContext requestContext, CancellationToken cancellationToken) =>
            new("synthetic-token", DateTimeOffset.UtcNow.AddHours(1));
        public override ValueTask<AccessToken> GetTokenAsync(TokenRequestContext requestContext, CancellationToken cancellationToken) =>
            ValueTask.FromResult(GetToken(requestContext, cancellationToken));
    }

    private sealed class VaultHttp : HttpMessageHandler
    {
        public ConcurrentBag<(HttpMethod Method, string Path)> Requests { get; } = [];
        private int authenticatedReads;
        public int AuthenticatedReads => Volatile.Read(ref authenticatedReads);
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Requests.Add((request.Method, request.RequestUri!.AbsolutePath));
            if (request.Headers.Authorization?.Scheme != "Bearer")
            {
                var challenge = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                challenge.Headers.TryAddWithoutValidation("WWW-Authenticate",
                    "Bearer authorization=\"https://login.windows.net/synthetic\", resource=\"https://vault.azure.net\"");
                return Task.FromResult(challenge);
            }
            Interlocked.Increment(ref authenticatedReads);
            var name = request.RequestUri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries)[1];
            var payload = System.Text.Json.JsonSerializer.Serialize(new
            {
                id = $"https://synthetic.vault.azure.net/secrets/{name}/version1",
                value = "private-sdk-value",
                attributes = new { enabled = true, exp = DateTimeOffset.UtcNow.AddHours(2).ToUnixTimeSeconds() }
            });
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(payload, System.Text.Encoding.UTF8, "application/json")
            });
        }
    }

    private sealed class TestResponse : Response
    {
        public override int Status => 200;
        public override string ReasonPhrase => "OK";
        public override Stream? ContentStream { get; set; }
        public override string ClientRequestId { get; set; } = "";
        public override void Dispose() { }
        protected override bool ContainsHeader(string name) => false;
        protected override IEnumerable<HttpHeader> EnumerateHeaders() => [];
        protected override bool TryGetHeader(string name, out string value) { value = ""; return false; }
        protected override bool TryGetHeaderValues(string name, out IEnumerable<string> values) { values = []; return false; }
    }

    private sealed class CaptureLogger : ILogger
    {
        public ConcurrentBag<string> Messages { get; } = [];
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
        public bool IsEnabled(LogLevel logLevel) => true;
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
            Func<TState, Exception?, string> formatter) => Messages.Add(formatter(state, exception));
    }

    private sealed class TestEnvironment : IWebHostEnvironment
    {
        public string EnvironmentName { get; set; } = "Production";
        public string ApplicationName { get; set; } = "Synthetic";
        public string ContentRootPath { get; set; } = "";
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
        public string WebRootPath { get; set; } = "";
        public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();
    }

    private sealed class Clock : TimeProvider
    {
        private DateTimeOffset now = DateTimeOffset.UtcNow;
        public override DateTimeOffset GetUtcNow() => now;
        public void Advance(TimeSpan value) => now += value;
    }
}
