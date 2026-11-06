using Arcora.Api;
using Arcora.Api.Controllers;
using Arcora.Api.Entities;
using Arcora.Api.Operations;
using Arcora.Api.Configurations;
using Arcora.Api.Email;
using Arcora.Api.Signing;
using Arcora.Api.Middleware;
using Azure.Storage.Blobs;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging.Abstractions;

namespace Arcora.Api.Tests;

public sealed class OperationsHealthTests
{
    private readonly Clock clock = new();
    private OperationsRegistry Registry() => new(Options.Create(new OperationsOptions { IntervalSeconds = 30, StuckMinutes = 2 }), clock);

    [Fact]
    public void Worker_failure_survives_success_until_incident_is_persisted()
    {
        var registry = Registry();
        registry.RegisterWorker("worker.test", "Test");
        using (var run = registry.Begin("worker.test")) run.Fail();
        using (var run = registry.Begin("worker.test")) run.Count();
        var failed = registry.Snapshot();
        Assert.Equal("Failed", Assert.Single(failed.Checks).Status);
        registry.AcknowledgeFailures(failed);
        var healthy = Assert.Single(registry.Snapshot().Checks);
        Assert.Equal("Healthy", healthy.Status);
        Assert.Equal(1, healthy.Failed);
        Assert.Equal(1, healthy.Processed);
    }

    [Fact]
    public void Worker_failure_after_snapshot_is_not_acknowledged()
    {
        var registry = Registry();
        registry.RegisterWorker("test", "Test");
        var snapshot = registry.Snapshot();
        using (var run = registry.Begin("test")) run.Fail();
        registry.AcknowledgeFailures(snapshot);
        Assert.Equal("Failed", Assert.Single(registry.Snapshot().Checks).Status);
    }

    [Fact]
    public void Idle_queue_is_healthy_but_processing_deadline_is_enforced()
    {
        var registry = Registry();
        registry.RegisterWorker("queue", "Queue");
        registry.Started("queue");
        clock.Advance(TimeSpan.FromDays(1));
        Assert.Equal("Healthy", Assert.Single(registry.Snapshot().Checks).Status);
        using var run = registry.Begin("queue");
        clock.Advance(TimeSpan.FromMinutes(3));
        Assert.Contains("deadline", Assert.Single(registry.Snapshot().Checks).Description);
    }

    [Fact]
    public void Disabled_scheduler_never_reports_missed_runs()
    {
        var registry = Registry();
        registry.RegisterWorker("rent", "Rent", TimeSpan.FromMinutes(1), false);
        clock.Advance(TimeSpan.FromDays(2));
        Assert.Equal("Disabled", Assert.Single(registry.Snapshot().Checks).Status);
    }

    [Fact]
    public void Missing_scheduler_startup_and_missed_next_run_are_detected()
    {
        var registry = Registry();
        registry.RegisterWorker("rent", "Rent", TimeSpan.FromMinutes(1));
        clock.Advance(TimeSpan.FromMinutes(2));
        Assert.Equal("Failed", Assert.Single(registry.Snapshot().Checks).Status);
        using (registry.Begin("rent")) { }
        clock.Advance(TimeSpan.FromMinutes(3));
        Assert.Contains("missed", Assert.Single(registry.Snapshot().Checks).Description);
    }

    [Fact]
    public void Canceled_work_is_not_reported_as_success_or_failure_and_dispose_is_idempotent()
    {
        var registry = Registry();
        registry.RegisterWorker("test", "Test");
        var run = registry.Begin("test");
        run.Cancel();
        run.Dispose();
        run.Dispose();
        Assert.Null(Assert.Single(registry.Snapshot().Checks).LastSuccessAt);
        Assert.Equal(0, Assert.Single(registry.Snapshot().Checks).Failed);
    }

    [Fact]
    public void Stale_probe_makes_readiness_fail_and_public_payload_hides_details()
    {
        var registry = Registry();
        var controller = new HealthProbeController(registry);
        Assert.Equal(503, Assert.IsType<ObjectResult>(controller.Ready()).StatusCode);
        registry.Report("dependency.database", "SQL", "Infrastructure", "Healthy", "private diagnostics");
        Assert.Equal(200, Assert.IsType<ObjectResult>(controller.Ready()).StatusCode);
        clock.Advance(TimeSpan.FromMinutes(2));
        var response = Assert.IsType<ObjectResult>(controller.Ready());
        Assert.Equal(503, response.StatusCode);
        Assert.DoesNotContain("private", System.Text.Json.JsonSerializer.Serialize(response.Value));
    }

    [Fact]
    public void Detailed_dashboard_is_admin_only_and_read_requests_do_not_run_probes()
    {
        var attribute = Assert.Single(typeof(OperationsHealthController).GetCustomAttributes(typeof(AuthorizeAttribute), false).Cast<AuthorizeAttribute>());
        Assert.Equal("Admin", attribute.Roles);
        var registry = Registry();
        registry.RegisterWorker("test", "Test");
        var controller = new OperationsHealthController(registry);
        Assert.Equal("Unknown", Assert.Single(controller.Get().Checks).Status);
        Assert.Equal("Unknown", Assert.Single(controller.Get().Checks).Status);
        Assert.Empty(controller.Get().Alerts);
    }

    [Theory]
    [InlineData(0, 10, false)]
    [InlineData(30, 0, false)]
    [InlineData(30, 10, true)]
    public void Invalid_monitor_intervals_fail_configuration_validation(int interval, int timeout, bool valid)
    {
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["OperationsHealth:IntervalSeconds"] = interval.ToString(),
            ["OperationsHealth:TimeoutSeconds"] = timeout.ToString()
        }).Build();
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddOperationsMonitoring(config);
        Assert.Contains(services, descriptor => descriptor.ServiceType == typeof(IHostedService) &&
            descriptor.ImplementationType == typeof(OperationsMonitor));
        Assert.Contains(services, descriptor => descriptor.ServiceType == typeof(IHostedService) &&
            descriptor.ImplementationType == typeof(OperationsAlertDispatcher));
        using var provider = services.BuildServiceProvider();
        var accessor = provider.GetRequiredService<IOptions<OperationsOptions>>();
        if (valid) Assert.Equal(interval, accessor.Value.IntervalSeconds);
        else Assert.Throws<OptionsValidationException>(() => accessor.Value);
    }

    [Fact]
    public async Task Incidents_are_durable_deduplicated_recover_and_reopen_without_cross_instance_leaks()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:;Foreign Keys=False");
        await connection.OpenAsync();
        var options = new DbContextOptionsBuilder<ArcoraDbContext>().UseSqlite(connection).Options;
        await using var db = new TestContext(options);
        await db.Database.EnsureCreatedAsync();
        var registry = Registry();
        registry.Report("stripe", "Stripe", "Provider", "Failed", "Provider connection failed.");
        await OperationsAlerts.ReconcileAsync(db, registry.Snapshot(), 2, default);
        Assert.Empty(await db.OperationsIncidents.ToListAsync());
        registry.Report("stripe", "Stripe", "Provider", "Failed", "Provider connection failed.");
        await OperationsAlerts.ReconcileAsync(db, registry.Snapshot(), 2, default);
        await OperationsAlerts.ReconcileAsync(db, registry.Snapshot(), 2, default);
        var first = Assert.Single(await db.OperationsIncidents.ToListAsync());
        Assert.Null(first.ResolvedAt);
        registry.Report("stripe", "Stripe", "Provider", "Unknown", "Not checked.");
        await OperationsAlerts.ReconcileAsync(db, registry.Snapshot(), 2, default);
        Assert.Null(first.ResolvedAt);
        registry.Report("stripe", "Stripe", "Provider", "Healthy", "Recovered.");
        await OperationsAlerts.ReconcileAsync(db, registry.Snapshot(), 2, default);
        Assert.NotNull(first.ResolvedAt);
        registry.Report("stripe", "Stripe", "Provider", "Failed", "Failed again.");
        registry.Report("stripe", "Stripe", "Provider", "Failed", "Failed again.");
        await OperationsAlerts.ReconcileAsync(db, registry.Snapshot(), 2, default);
        Assert.Equal(2, await db.OperationsIncidents.CountAsync());
        await OperationsAlerts.ReconcileAsync(db, registry.Snapshot() with { InstanceId = "other" }, 2, default);
        Assert.Equal(3, await db.OperationsIncidents.CountAsync());
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(true, false)]
    [InlineData(true, true)]
    public async Task Operational_email_sends_once_per_incident_and_once_on_recovery(bool failFirst, bool recoverBeforeRetry)
    {
        await using var connection = new SqliteConnection("Data Source=:memory:;Foreign Keys=False");
        await connection.OpenAsync();
        await using var db = new TestContext(new DbContextOptionsBuilder<ArcoraDbContext>().UseSqlite(connection).Options);
        await db.Database.EnsureCreatedAsync();
        await using var smtp = new SmtpServer();
        smtp.Reject = failFirst;
        var settings = Options.Create(new OperationsOptions
        {
            EmailEnabled = true, EmailRecipients = ["operations@example.test"], TimeoutSeconds = 5
        });
        var registry = new OperationsRegistry(settings, clock);
        var services = new ServiceCollection().AddLogging().AddSingleton<ArcoraDbContext>(db)
            .AddSingleton(Options.Create(new EmailSettings
            {
                SmtpHost = "127.0.0.1", SmtpPort = smtp.Port, EnableSsl = false,
                SenderEmail = "health@example.test", SenderName = "Health"
            })).AddTransient<EmailSender>();
        await using var provider = services.BuildServiceProvider();
        var alerts = new OperationsAlerts(provider.GetRequiredService<IServiceScopeFactory>(), registry, settings,
            NullLogger<OperationsAlerts>.Instance);
        registry.Report("test", "Test", "Provider", "Failed", "Unavailable.");
        registry.Report("test", "Test", "Provider", "Failed", "Unavailable.");
        await alerts.RunAsync(default);
        if (failFirst)
        {
            Assert.Equal(0, smtp.Messages);
            Assert.NotNull(Assert.Single(await db.OperationsIncidents.ToListAsync()).NextEmailAttemptAt);
            await alerts.RunAsync(default);
            Assert.Equal(0, smtp.Messages);
            if (recoverBeforeRetry)
                registry.Report("test", "Test", "Provider", "Healthy", "Recovered before delivery.");
            smtp.Reject = false;
            clock.Advance(TimeSpan.FromMinutes(3));
        }
        alerts = new OperationsAlerts(provider.GetRequiredService<IServiceScopeFactory>(), registry, settings,
            NullLogger<OperationsAlerts>.Instance);
        await alerts.RunAsync(default);
        Assert.Equal(1, smtp.Messages);
        Assert.NotNull(Assert.Single(await db.OperationsIncidents.ToListAsync()).EmailSentAt);
        registry.Report("test", "Test", "Provider", "Healthy", "Recovered.");
        await alerts.RunAsync(default);
        await alerts.RunAsync(default);
        Assert.Equal(recoverBeforeRetry ? 1 : 2, smtp.Messages);
        Assert.NotNull(Assert.Single(await db.OperationsIncidents.ToListAsync()).RecoveryEmailSentAt);
    }

    [Theory]
    [InlineData(400)]
    [InlineData(401)]
    [InlineData(404)]
    [InlineData(409)]
    [InlineData(500)]
    public async Task Api_metrics_record_final_exception_mapped_status(int status)
    {
        var metrics = new OperationsRequestMetrics(clock);
        var services = new ServiceCollection().AddLogging().AddSingleton(metrics);
        using var provider = services.BuildServiceProvider();
        var pipeline = new ApplicationBuilder(provider);
        pipeline.UseMiddleware<OperationsRequestMetricsMiddleware>();
        pipeline.UseMiddleware<GlobalExceptionMiddleware>();
        pipeline.Run(_ => throw (status switch
        {
            400 => new ArgumentException("Synthetic invalid input"),
            401 => new UnauthorizedAccessException("Synthetic denied"),
            404 => new KeyNotFoundException("Synthetic missing"),
            409 => new Arcora.Api.Exceptions.ApiProblemException(409, "Conflict", "Synthetic conflict"),
            _ => new Exception("Synthetic server failure")
        }));
        var app = pipeline.Build();
        for (var i = 0; i < 5; i++)
        {
            var context = new DefaultHttpContext { RequestServices = provider };
            context.Request.Path = "/api/synthetic";
            context.Response.Body = new MemoryStream();
            await app(context);
            Assert.Equal(status, context.Response.StatusCode);
        }
        Assert.Equal(status == 500 ? "Failed" : "Healthy", metrics.Snapshot().Status);
        Assert.Contains(status == 500 ? "5 server errors" : "0 server errors", metrics.Snapshot().Description);
    }

    [Fact]
    public async Task Blocked_alert_delivery_does_not_block_monitor_refresh()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:;Foreign Keys=False");
        await connection.OpenAsync();
        await using var db = new TestContext(new DbContextOptionsBuilder<ArcoraDbContext>().UseSqlite(connection).Options);
        await db.Database.EnsureCreatedAsync();
        await db.Database.ExecuteSqlRawAsync(
            "CREATE TABLE __EFMigrationsHistory (MigrationId TEXT NOT NULL PRIMARY KEY, ProductVersion TEXT NOT NULL)");
        foreach (var migration in db.Database.GetMigrations())
            await db.Database.ExecuteSqlInterpolatedAsync(
                $"INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion) VALUES ({migration}, {"8.0.19"})");
        await using var smtp = new SmtpServer { Stall = true };
        var settings = Options.Create(new OperationsOptions
        {
            IntervalSeconds = 30, EmailEnabled = true, EmailRecipients = ["operations@example.test"], TimeoutSeconds = 30
        });
        var registry = new OperationsRegistry(settings, clock);
        var alertServices = new ServiceCollection().AddLogging().AddSingleton<ArcoraDbContext>(db)
            .AddSingleton(Options.Create(new EmailSettings
            {
                SmtpHost = "127.0.0.1", SmtpPort = smtp.Port, EnableSsl = false,
                SenderEmail = "health@example.test", SenderName = "Health"
            })).AddTransient<EmailSender>();
        await using var alertProvider = alertServices.BuildServiceProvider();
        var alerts = new OperationsAlerts(alertProvider.GetRequiredService<IServiceScopeFactory>(), registry, settings,
            NullLogger<OperationsAlerts>.Instance);
        registry.Report("test", "Test", "Provider", "Failed", "Unavailable.");
        registry.Report("test", "Test", "Provider", "Failed", "Unavailable.");
        using var dispatcher = new OperationsAlertDispatcher(alerts, settings,
            NullLogger<OperationsAlertDispatcher>.Instance, clock);
        await dispatcher.StartAsync(default);
        await smtp.Connected.Task.WaitAsync(TimeSpan.FromSeconds(5));

        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["PhoneVerification:Provider"] = "LOCAL"
        }).Build();
        var probeServices = new ServiceCollection().AddLogging().AddSignalR().Services;
        probeServices.AddScoped<ArcoraDbContext>(_ =>
            new TestContext(new DbContextOptionsBuilder<ArcoraDbContext>().UseSqlite(connection).Options));
        using var probeProvider = probeServices.BuildServiceProvider();
        var probes = new OperationsProbes(probeProvider.GetRequiredService<IServiceScopeFactory>(), registry, config,
            Options.Create(new OperationsOptions { TimeoutSeconds = 1, PdfTimeoutSeconds = 5 }), new Clients(new ReadOnlyHandler()),
            new OfflineStorage(), new EmailQueue(), new SigningQueue(), NullLogger<OperationsProbes>.Instance);
        using var monitor = new OperationsMonitor(registry, probes, settings, NullLogger<OperationsMonitor>.Instance, clock);
        try
        {
            await monitor.StartAsync(default);
            await WaitForCycleAsync(registry, clock.GetUtcNow().UtcDateTime);
            Assert.Equal(200, Assert.IsType<ObjectResult>(new HealthProbeController(registry).Ready()).StatusCode);
            var first = registry.Snapshot().Checks.Single(x => x.Id == "monitor.cycle").CheckedAt;
            clock.Advance(TimeSpan.FromSeconds(121));
            clock.TickTimers();
            await WaitForCycleAsync(registry, first.AddSeconds(1));
            Assert.Equal(200, Assert.IsType<ObjectResult>(new HealthProbeController(registry).Ready()).StatusCode);
            Assert.False(dispatcher.ExecuteTask!.IsCompleted);
            Assert.Equal(0, smtp.Messages);
        }
        finally
        {
            using var stop = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            await monitor.StopAsync(stop.Token);
            await dispatcher.StopAsync(stop.Token);
        }
    }

    private static async Task WaitForCycleAsync(OperationsRegistry registry, DateTime atLeast)
    {
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(15));
        while (!registry.Snapshot().Checks.Any(x => x.Id == "monitor.cycle" && x.Status == "Healthy" && x.CheckedAt >= atLeast))
            await Task.Delay(20, timeout.Token);
    }

    [Fact]
    public async Task Probes_only_issue_read_requests_and_do_not_expose_provider_bodies_or_credentials()
    {
        var handler = new ReadOnlyHandler();
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Stripe:SecretKey"] = "sk_test_never_expose",
            ["IdentityVerification:DefaultProvider"] = "PERSONA",
            ["Persona:ApiKey"] = "persona_never_expose",
            ["Persona:BaseUrl"] = "https://provider.example.test/api/v1/",
            ["PhoneVerification:Provider"] = "LOCAL",
            ["FrontendUrl"] = "https://web.example.test"
        }).Build();
        var services = new ServiceCollection().AddLogging().AddSignalR().Services;
        using var provider = services.BuildServiceProvider();
        var registry = Registry();
        var probes = new OperationsProbes(provider.GetRequiredService<IServiceScopeFactory>(), registry, config,
            Options.Create(new OperationsOptions { TimeoutSeconds = 1 }), new Clients(handler), new OfflineStorage(),
            new EmailQueue(), new SigningQueue(), NullLogger<OperationsProbes>.Instance);
        await probes.RunAsync(default);
        Assert.NotEmpty(handler.Methods);
        Assert.All(handler.Methods, method => Assert.Equal(HttpMethod.Get, method));
        var snapshot = registry.Snapshot();
        var json = System.Text.Json.JsonSerializer.Serialize(snapshot);
        Assert.DoesNotContain("never_expose", json);
        Assert.DoesNotContain("private-provider-body", json);
        Assert.Equal("Failed", snapshot.Checks.Single(x => x.Id == "dependency.stripe").Status);
        Assert.Equal("Disabled", snapshot.Checks.Single(x => x.Id == "dependency.phone").Status);
    }

    [Fact]
    public async Task Production_queues_expose_counts_and_oldest_items_without_dequeueing()
    {
        var emails = new EmailQueue();
        await emails.EnqueueAsync(new("synthetic@example.test", "Test", "Test"));
        Assert.True(emails.Reader.CanCount);
        Assert.Equal(1, emails.Reader.Count);
        Assert.True(emails.Reader.TryPeek(out var email));
        Assert.True(email.EnqueuedAt <= DateTime.UtcNow);
        Assert.Equal(1, emails.Reader.Count);
        var signing = new SigningQueue();
        await signing.EnqueueAsync(new());
        Assert.True(signing.Reader.CanCount);
        Assert.Equal(1, signing.Reader.Count);
        Assert.True(signing.Reader.TryPeek(out _));
        Assert.Equal(1, signing.Reader.Count);
    }

    [Fact]
    public async Task Api_metrics_capture_exceptions_exclude_probes_and_expire_old_samples()
    {
        var metrics = new OperationsRequestMetrics(clock);
        var middleware = new OperationsRequestMetricsMiddleware(_ => throw new InvalidOperationException("Synthetic"));
        for (var i = 0; i < 5; i++)
        {
            var context = new DefaultHttpContext();
            context.Request.Path = "/api/synthetic";
            await Assert.ThrowsAsync<InvalidOperationException>(() => middleware.InvokeAsync(context, metrics));
        }
        Assert.Equal("Failed", metrics.Snapshot().Status);
        clock.Advance(TimeSpan.FromMinutes(5));
        Assert.Equal("Healthy", metrics.Snapshot().Status);
        var healthy = new OperationsRequestMetricsMiddleware(_ => Task.CompletedTask);
        var probe = new DefaultHttpContext();
        probe.Request.Path = "/health/ready";
        await healthy.InvokeAsync(probe, metrics);
        Assert.Contains("0 requests", metrics.Snapshot().Description);
        for (var i = 0; i < 20; i++) metrics.Record(200, 3000);
        Assert.Equal("Degraded", metrics.Snapshot().Status);
    }

    private sealed class OfflineStorage : BlobServiceClient
    {
        public override BlobContainerClient GetBlobContainerClient(string name) => throw new IOException("Synthetic offline storage");
    }

    private sealed class Clients(HttpMessageHandler handler) : IHttpClientFactory
    {
        public HttpClient CreateClient(string name) => new(handler, disposeHandler: false);
    }

    private sealed class ReadOnlyHandler : HttpMessageHandler
    {
        public System.Collections.Concurrent.ConcurrentBag<HttpMethod> Methods { get; } = [];
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Methods.Add(request.Method);
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.Unauthorized)
            {
                Content = new StringContent("private-provider-body")
            });
        }
    }

    private sealed class SmtpServer : IAsyncDisposable
    {
        private readonly TcpListener listener = new(IPAddress.Loopback, 0);
        private readonly CancellationTokenSource stop = new();
        private readonly Task loop;
        private int messages;
        public bool Reject { get; set; }
        public bool Stall { get; init; }
        public TaskCompletionSource<bool> Connected { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
        public int Messages => Volatile.Read(ref messages);
        public int Port => ((IPEndPoint)listener.LocalEndpoint).Port;
        public SmtpServer()
        {
            listener.Start();
            loop = RunAsync();
        }
        private async Task RunAsync()
        {
            try
            {
                while (!stop.IsCancellationRequested)
                {
                    using var client = await listener.AcceptTcpClientAsync(stop.Token);
                    Connected.TrySetResult(true);
                    if (Stall) await Task.Delay(Timeout.InfiniteTimeSpan, stop.Token);
                    using var stream = client.GetStream();
                    using var reader = new StreamReader(stream);
                    using var writer = new StreamWriter(stream, Encoding.ASCII) { AutoFlush = true, NewLine = "\r\n" };
                    await writer.WriteLineAsync("220 localhost");
                    while (await reader.ReadLineAsync(stop.Token) is { } line)
                    {
                        if (line == "DATA")
                        {
                            await writer.WriteLineAsync("354 continue");
                            while (await reader.ReadLineAsync(stop.Token) is { } data && data != ".") { }
                            Interlocked.Increment(ref messages);
                            await writer.WriteLineAsync("250 accepted");
                        }
                        else if (line == "QUIT")
                        {
                            await writer.WriteLineAsync("221 bye");
                            break;
                        }
                        else if (Reject && line.StartsWith("RCPT", StringComparison.Ordinal))
                            await writer.WriteLineAsync("550 rejected");
                        else await writer.WriteLineAsync("250 localhost");
                    }
                }
            }
            catch (OperationCanceledException) when (stop.IsCancellationRequested) { }
        }
        public async ValueTask DisposeAsync()
        {
            stop.Cancel();
            await loop;
            listener.Stop();
            stop.Dispose();
        }
    }

    private sealed class Clock : TimeProvider
    {
        private DateTimeOffset now = new(2026, 10, 29, 0, 0, 0, TimeSpan.Zero);
        private readonly List<ManualTimer> timers = [];
        public override DateTimeOffset GetUtcNow() => now;
        public void Advance(TimeSpan value) => now += value;
        public override ITimer CreateTimer(TimerCallback callback, object? state, TimeSpan dueTime, TimeSpan period)
        {
            var timer = new ManualTimer(callback, state);
            lock (timers) timers.Add(timer);
            return timer;
        }
        public void TickTimers()
        {
            ManualTimer[] snapshot;
            lock (timers) snapshot = timers.ToArray();
            foreach (var timer in snapshot) timer.Tick();
        }
        private sealed class ManualTimer(TimerCallback callback, object? state) : ITimer
        {
            private bool disposed;
            public bool Change(TimeSpan dueTime, TimeSpan period) => !disposed;
            public void Tick() { if (!disposed) callback(state); }
            public void Dispose() => disposed = true;
            public ValueTask DisposeAsync() { Dispose(); return ValueTask.CompletedTask; }
        }
    }

    private sealed class TestContext(DbContextOptions<ArcoraDbContext> options)
        : ArcoraDbContext(options, new ConfigurationBuilder().Build())
    {
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            foreach (var property in builder.Model.GetEntityTypes().SelectMany(entity => entity.GetProperties()))
                property.SetColumnType(null);
            foreach (var property in builder.Entity<RentPaymentAccounting>().Metadata.GetProperties().Where(x => x.ClrType == typeof(decimal)))
                builder.Entity<RentPaymentAccounting>().Property<decimal>(property.Name).HasConversion<double>();
        }
    }
}
