using System.Data;
using System.Net;
using Arcora.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Arcora.Api.Operations;

public sealed class OperationsAlerts(
    IServiceScopeFactory scopes, OperationsRegistry registry, IOptions<OperationsOptions> options,
    ILogger<OperationsAlerts> logger)
{
    private OperationsIncident? storageOutage;

    public async Task RunAsync(CancellationToken ct)
    {
        try
        {
            using var scope = scopes.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ArcoraDbContext>();
            await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable, ct);
            if (db.Database.IsSqlServer())
            {
                var resource = $"operations-alerts:{registry.InstanceId}";
                await db.Database.ExecuteSqlInterpolatedAsync($"""
                    DECLARE @result int;
                    EXEC @result = sys.sp_getapplock @Resource={resource}, @LockMode='Exclusive',
                        @LockOwner='Transaction', @LockTimeout=5000;
                    IF @result < 0 THROW 51002, 'Unable to acquire operations alert lock.', 1;
                    """, ct);
            }
            var snapshot = registry.Snapshot();
            await ReconcileAsync(db, snapshot, options.Value.FailureThreshold, ct);
            await tx.CommitAsync(ct);
            registry.AcknowledgeFailures(snapshot);

            var due = await db.OperationsIncidents.Where(x => x.InstanceId == registry.InstanceId &&
                (x.EmailSentAt == null ||
                 (x.ResolvedAt != null && x.RecoveryEmailSentAt == null)) &&
                (x.NextEmailAttemptAt == null || x.NextEmailAttemptAt <= registry.Now))
                .OrderBy(x => x.OpenedAt).Take(10).ToListAsync(ct);
            foreach (var incident in due)
            {
                if (!registry.EmailConfigured) break;
                await DeliverAsync(incident, ct);
                await db.SaveChangesAsync(ct);
            }
            var history = await db.OperationsIncidents.AsNoTracking().Where(x => x.InstanceId == registry.InstanceId)
                .OrderBy(x => x.ResolvedAt != null).ThenByDescending(x => x.OpenedAt).Take(100).ToListAsync(ct);
            registry.SetAlerts(history.Select(ToAlert));
            registry.Report("monitor.alert-store", "Incident persistence", "Monitoring", "Healthy",
                "Incident transitions are persisted; dashboard shows the latest 100 incidents for this instance.");
            if (storageOutage != null)
            {
                storageOutage.ResolvedAt ??= registry.Now;
                if (!storageOutage.RecoveryEmailSentAt.HasValue)
                    await DeliverAsync(storageOutage, ct);
                if (!registry.EmailConfigured || storageOutage.RecoveryEmailSentAt.HasValue)
                    storageOutage = null;
            }
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested) { throw; }
        catch (Exception ex)
        {
            logger.LogError("Operations incident persistence failed ({ExceptionType}); cached incidents remain visible.", ex.GetType().Name);
            registry.Report("monitor.alert-store", "Incident persistence", "Monitoring", "Failed",
                "Incident persistence is unavailable. Cached state is shown; external uptime monitoring is required.");
            storageOutage ??= new OperationsIncident
            {
                Id = Guid.NewGuid(), InstanceId = registry.InstanceId, CheckId = "monitor.alert-store",
                Status = "Failed", Message = "Health incident persistence is unavailable; database or monitoring storage may be down.",
                OpenedAt = registry.Now
            };
            if (registry.EmailConfigured && !storageOutage.EmailSentAt.HasValue)
                await DeliverAsync(storageOutage, ct);
            registry.SetAlerts(registry.Snapshot().Alerts.Where(x => x.CheckId != "monitor.alert-store")
                .Append(ToAlert(storageOutage)));
        }
    }

    public static async Task ReconcileAsync(ArcoraDbContext db, OperationsSnapshot snapshot, int threshold, CancellationToken ct)
    {
        var active = await db.OperationsIncidents.Where(x => x.InstanceId == snapshot.InstanceId && x.ResolvedAt == null)
            .ToListAsync(ct);
        foreach (var check in snapshot.Checks)
        {
            var incident = active.SingleOrDefault(x => x.CheckId == check.Id);
            if (check.Status is "Failed" or "Degraded" && check.ConsecutiveFailures >= threshold)
            {
                if (incident == null)
                {
                    incident = new()
                    {
                        Id = Guid.NewGuid(), InstanceId = snapshot.InstanceId, CheckId = check.Id,
                        OpenedAt = snapshot.CheckedAt
                    };
                    db.OperationsIncidents.Add(incident);
                }
                incident.Status = check.Status;
                incident.Message = check.Description;
            }
            else if (incident != null && check.Status == "Healthy")
            {
                incident.ResolvedAt = snapshot.CheckedAt;
                incident.NextEmailAttemptAt = null;
                incident.EmailAttempts = 0;
            }
        }
        await db.SaveChangesAsync(ct);
    }

    private async Task DeliverAsync(OperationsIncident incident, CancellationToken ct)
    {
        if (!registry.EmailConfigured || incident.NextEmailAttemptAt > registry.Now) return;
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(ct);
        timeout.CancelAfter(TimeSpan.FromSeconds(options.Value.TimeoutSeconds));
        try
        {
            using var scope = scopes.CreateScope();
            var recovery = incident.ResolvedAt.HasValue;
            var subject = $"Arcora operations {(recovery ? "recovered" : "alert")}: {incident.CheckId}";
            var body = $"<p>{WebUtility.HtmlEncode(subject)}</p><p>Instance: {WebUtility.HtmlEncode(incident.InstanceId)}</p>" +
                $"<p>{WebUtility.HtmlEncode(incident.Message)}</p><p>Opened: {incident.OpenedAt:O}</p>" +
                (recovery ? $"<p>Recovered: {incident.ResolvedAt:O}</p>" : "");
            await scope.ServiceProvider.GetRequiredService<EmailSender>()
                .SendEmailAsync(string.Join(",", options.Value.EmailRecipients), subject, body, timeout.Token);
            if (recovery)
            {
                incident.EmailSentAt ??= registry.Now;
                incident.RecoveryEmailSentAt = registry.Now;
            }
            else incident.EmailSentAt = registry.Now;
            incident.EmailAttempts = 0;
            incident.NextEmailAttemptAt = null;
            registry.Report("monitor.email", "Operations alert delivery", "Monitoring", "Healthy", "Latest operational alert email was accepted by SMTP.");
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested) { throw; }
        catch (Exception ex)
        {
            incident.EmailAttempts++;
            incident.NextEmailAttemptAt = registry.Now.AddMinutes(Math.Min(60, Math.Pow(2, Math.Min(incident.EmailAttempts, 6))));
            logger.LogError("Operations alert email failed ({ExceptionType}); retry is scheduled.", ex.GetType().Name);
            registry.Report("monitor.email", "Operations alert delivery", "Monitoring", "Failed",
                "Operational alert email delivery failed; retry is scheduled. In-app incidents remain available.");
        }
    }

    private static OperationsAlert ToAlert(OperationsIncident x) =>
        new(x.Id, x.CheckId, x.Status, x.Message, x.OpenedAt, x.ResolvedAt, x.EmailSentAt);
}
