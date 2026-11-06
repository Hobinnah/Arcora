using System.Collections.Concurrent;
using Microsoft.Extensions.Options;

namespace Arcora.Api.Operations;

public sealed class OperationsRegistry(IOptions<OperationsOptions> options, TimeProvider time)
{
    private readonly ConcurrentDictionary<string, OperationsCheck> checks = new();
    private readonly ConcurrentDictionary<string, TimeSpan> periods = new();
    private readonly ConcurrentDictionary<string, byte> running = new();
    private readonly ConcurrentDictionary<string, OperationsCheck> pendingFailures = new();
    private OperationsAlert[] alerts = [];
    public string InstanceId { get; } = options.Value.InstanceId ?? Environment.MachineName;
    public DateTime Now => time.GetUtcNow().UtcDateTime;
    public bool EmailConfigured => options.Value.EmailEnabled && options.Value.EmailRecipients.Length > 0;

    public void RegisterWorker(string id, string name, TimeSpan? period = null, bool enabled = true)
    {
        if (period.HasValue) periods[id] = period.Value;
        checks[id] = new(id, name, "Worker", enabled ? "Unknown" : "Disabled",
            enabled ? "Waiting for worker startup." : "Disabled by configuration.", Now,
            NextExpectedAt: enabled ? Now.AddSeconds(options.Value.IntervalSeconds) : null);
    }

    public void Started(string id)
    {
        checks.AddOrUpdate(id, _ => new(id, id, "Worker", "Healthy", "Worker is listening.", Now, Now),
            (_, old) => old with { Status = "Healthy", Description = "Worker is listening.", CheckedAt = Now, LastSuccessAt = Now,
                NextExpectedAt = null });
    }

    public Run Begin(string id)
    {
        running[id] = 0;
        checks.AddOrUpdate(id, _ => new(id, id, "Worker", "Unknown", "Processing.", Now, LastStartedAt: Now),
            (_, old) => old with { LastStartedAt = Now, CheckedAt = Now, Description = "Processing." });
        return new Run(this, id, time.GetTimestamp());
    }

    private void Complete(string id, long started, bool failed, bool canceled, long processed)
    {
        running.TryRemove(id, out _);
        if (canceled) return;
        var now = Now;
        var completed = checks.AddOrUpdate(id, _ => throw new InvalidOperationException("Worker was not registered."),
            (_, old) => old with
            {
                Status = failed ? "Failed" : "Healthy",
                Description = failed ? "Work failed. Consult application logs; failed work may require retry or review." : "Last work completed successfully.",
                CheckedAt = now, LastSuccessAt = failed ? old.LastSuccessAt : now,
                NextExpectedAt = periods.TryGetValue(id, out var period) ? now + period : null,
                DurationMs = time.GetElapsedTime(started).TotalMilliseconds,
                ConsecutiveFailures = failed ? old.ConsecutiveFailures + 1 : 0,
                Processed = old.Processed + processed, Failed = old.Failed + (failed ? 1 : 0)
            });
        if (failed) pendingFailures[id] = completed;
    }

    public void Report(string id, string name, string category, string status, string description, double? duration = null, int? backlog = null)
    {
        var now = Now;
        checks.AddOrUpdate(id, _ => new(id, name, category, status, description, now,
                status == "Healthy" ? now : null, DurationMs: duration,
                ConsecutiveFailures: status is "Failed" or "Degraded" ? 1 : 0, Backlog: backlog),
            (_, old) => old with
            {
                Status = status, Description = description, CheckedAt = now, DurationMs = duration, Backlog = backlog,
                LastSuccessAt = status == "Healthy" ? now : old.LastSuccessAt,
                ConsecutiveFailures = status is "Failed" or "Degraded" ? old.ConsecutiveFailures + 1 : 0
            });
    }

    public void SetAlerts(IEnumerable<OperationsAlert> value) => Volatile.Write(ref alerts, value.ToArray());

    public OperationsSnapshot Snapshot()
    {
        var now = Now;
        var rows = checks.Values.Select(row =>
        {
            if (pendingFailures.TryGetValue(row.Id, out var failure))
                return failure with { ConsecutiveFailures = Math.Max(failure.ConsecutiveFailures, options.Value.FailureThreshold) };
            if (row.Status == "Disabled") return row;
            if (row.Category != "Worker")
                return now - row.CheckedAt > (row.Id == "dependency.pdf" ? TimeSpan.FromMinutes(20) :
                    TimeSpan.FromSeconds(options.Value.IntervalSeconds * 3)) && row.Id != "monitor.email"
                    ? row with { Status = "Unknown", Description = "Health result is stale; monitor may have stopped." } : row;
            var stuck = running.ContainsKey(row.Id) && row.LastStartedAt.HasValue &&
                now - row.LastStartedAt.Value > TimeSpan.FromMinutes(options.Value.StuckMinutes);
            var missed = !running.ContainsKey(row.Id) && row.NextExpectedAt.HasValue &&
                now > row.NextExpectedAt.Value.AddSeconds(options.Value.IntervalSeconds * 2);
            return stuck || missed
                ? row with { Status = "Failed", Description = stuck ? "Worker processing exceeded its deadline." : "Expected worker run was missed.",
                    ConsecutiveFailures = Math.Max(row.ConsecutiveFailures, options.Value.FailureThreshold) }
                : row;
        }).OrderBy(x => x.Category).ThenBy(x => x.Name).ToArray();
        var overall = rows.Any(x => x.Status == "Failed") ? "Failed" :
            rows.Any(x => x.Status is "Degraded" or "Unknown") ? "Degraded" : "Healthy";
        return new(now, InstanceId, overall, rows, Volatile.Read(ref alerts), EmailConfigured);
    }

    public void AcknowledgeFailures(OperationsSnapshot snapshot)
    {
        foreach (var pair in pendingFailures)
            if (pair.Value.CheckedAt <= snapshot.CheckedAt &&
                snapshot.Checks.Any(check => check.Id == pair.Key && check.Failed >= pair.Value.Failed))
                ((ICollection<KeyValuePair<string, OperationsCheck>>)pendingFailures).Remove(pair);
    }

    public sealed class Run(OperationsRegistry owner, string id, long started) : IDisposable
    {
        private bool failed;
        private bool canceled;
        private long processed;
        private int disposed;
        public void Fail() => failed = true;
        public void Cancel() => canceled = true;
        public void Count(long count = 1) => processed += count;
        public void Dispose()
        {
            if (Interlocked.Exchange(ref disposed, 1) == 0)
                owner.Complete(id, started, failed, canceled, processed);
        }
    }
}
