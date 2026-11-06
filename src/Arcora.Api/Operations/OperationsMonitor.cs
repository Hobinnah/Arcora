using System.Net.Mail;
using Arcora.Api.Configurations;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Arcora.Api.Operations;

public sealed class OperationsMonitor(
    OperationsRegistry registry, OperationsProbes probes,
    IOptions<OperationsOptions> options, ILogger<OperationsMonitor> logger, TimeProvider time) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!options.Value.Enabled)
        {
            registry.Report("monitor.cycle", "Operations monitor", "Monitoring", "Disabled", "Health probing and alert delivery are disabled.");
            return;
        }
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(options.Value.IntervalSeconds), time);
        do
        {
            try
            {
                await probes.RunAsync(stoppingToken);
                registry.Report("monitor.cycle", "Operations monitor", "Monitoring", "Healthy", "Monitoring cycle completed.");
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) { break; }
            catch (Exception ex)
            {
                logger.LogError("Operations monitor cycle failed ({ExceptionType}).", ex.GetType().Name);
                registry.Report("monitor.cycle", "Operations monitor", "Monitoring", "Failed", "Monitoring cycle failed; inspect application logs.");
            }
        }
        while (await timer.WaitForNextTickAsync(stoppingToken));
    }
}

public sealed class OperationsAlertDispatcher(
    OperationsAlerts alerts, IOptions<OperationsOptions> options, ILogger<OperationsAlertDispatcher> logger, TimeProvider time) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!options.Value.Enabled) return;
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(options.Value.IntervalSeconds), time);
        do
        {
            try
            {
                await alerts.RunAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) { break; }
            catch (Exception ex)
            {
                logger.LogError("Operations alert dispatch failed ({ExceptionType}); the next cycle will retry.", ex.GetType().Name);
            }
        }
        while (await timer.WaitForNextTickAsync(stoppingToken));
    }
}

public static class OperationsRegistration
{
    public static IServiceCollection AddOperationsMonitoring(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<OperationsOptions>().Bind(configuration.GetSection("OperationsHealth"))
            .Validate(x => x.IntervalSeconds is >= 30 and <= 3600 && x.TimeoutSeconds is >= 1 and <= 30 &&
                x.PdfTimeoutSeconds is >= 5 and <= 120 &&
                x.FailureThreshold is >= 1 and <= 20 && x.StuckMinutes is >= 1 and <= 1440 &&
                x.PaymentPendingHours is >= 1 and <= 720 && x.VerificationPendingHours is >= 1 and <= 720 &&
                x.PayoutPendingHours is >= 1 and <= 720, "Operations health intervals and thresholds are outside supported limits.")
            .Validate(x => x.InstanceId == null || !string.IsNullOrWhiteSpace(x.InstanceId) && x.InstanceId.Length <= 128,
                "OperationsHealth:InstanceId must have 1-128 characters when configured.")
            .Validate(x => x.WebUrl == null || Uri.TryCreate(x.WebUrl, UriKind.Absolute, out var uri) &&
                uri.Scheme is "http" or "https", "OperationsHealth:WebUrl must be an absolute HTTP(S) URL.")
            .Validate(x => x.EmailRecipients != null && (!x.EmailEnabled || x.EmailRecipients.Length > 0 &&
                x.EmailRecipients.All(address => MailAddress.TryCreate(address, out _))),
                "Enabled operational email alerts require valid OperationsHealth:EmailRecipients.")
            .ValidateOnStart();
        services.AddSingleton(TimeProvider.System);
        services.TryAddSingleton(KeyVaultStartupState.Disabled);
        services.AddSingleton<KeyVaultConnectivity>();
        services.AddSingleton<OperationsRequestMetrics>();
        services.AddSingleton<OperationsRegistry>(sp =>
        {
            var registry = new OperationsRegistry(sp.GetRequiredService<IOptions<OperationsOptions>>(), sp.GetRequiredService<TimeProvider>());
            var rent = configuration.GetSection("RentCollection").Get<RentCollectionOptions>() ?? new();
            registry.RegisterWorker(WorkerIds.Rent, "Recurring rent collection",
                TimeSpan.FromMinutes(Math.Max(1, rent.PollingIntervalMinutes)), rent.Enabled);
            registry.RegisterWorker(WorkerIds.Invitations, "Tenant invitation email dispatcher", TimeSpan.FromSeconds(15));
            registry.RegisterWorker(WorkerIds.Email, "General email dispatcher");
            registry.RegisterWorker(WorkerIds.Signing, "Lease signing processor");
            registry.RegisterWorker(WorkerIds.CheckIn, "Move-in details email scheduler",
                TimeSpan.FromHours(Math.Max(1, configuration.GetValue<int?>("LeaseCheckIn:NotificationPollingHours") ?? 6)));
            registry.RegisterWorker(WorkerIds.Cleanup, "Move-in draft evidence cleanup", TimeSpan.FromHours(1));
            registry.Report("monitor.email", "Operations alert delivery", "Monitoring",
                registry.EmailConfigured ? "Unknown" : "NotConfigured",
                registry.EmailConfigured ? "Waiting for the first operational email; no test email is sent by probes." :
                    "In-app incidents are enabled. Configure email recipients and enable email alerts for notifications.");
            return registry;
        });
        services.AddHttpClient("Operations", client => client.Timeout = Timeout.InfiniteTimeSpan)
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { AllowAutoRedirect = false });
        services.AddTransient<EmailSender>();
        services.AddSingleton<OperationsProbes>();
        services.AddSingleton<OperationsAlerts>();
        services.AddHostedService<OperationsMonitor>();
        services.AddHostedService<OperationsAlertDispatcher>();
        return services;
    }
}
