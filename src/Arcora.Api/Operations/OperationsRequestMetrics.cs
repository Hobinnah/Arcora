using System.Diagnostics;

namespace Arcora.Api.Operations;

public sealed class OperationsRequestMetrics(TimeProvider time)
{
    private readonly object gate = new();
    private readonly Dictionary<long, Bucket> buckets = [];
    private sealed class Bucket
    {
        public long Count;
        public long Errors;
        public double Milliseconds;
    }

    public void Record(int status, double milliseconds)
    {
        lock (gate)
        {
            var minute = time.GetUtcNow().ToUnixTimeSeconds() / 60;
            Prune(minute);
            if (!buckets.TryGetValue(minute, out var bucket))
                buckets[minute] = bucket = new();
            bucket.Count++;
            if (status >= 500) bucket.Errors++;
            bucket.Milliseconds += milliseconds;
        }
    }

    public (string Status, string Description) Snapshot()
    {
        lock (gate)
        {
            Prune(time.GetUtcNow().ToUnixTimeSeconds() / 60);
            var total = buckets.Values.Sum(x => x.Count);
            var errors = buckets.Values.Sum(x => x.Errors);
            var average = total > 0 ? buckets.Values.Sum(x => x.Milliseconds) / total : 0;
            var rate = total > 0 ? errors * 100d / total : 0;
            var status = errors >= 5 && rate >= 50 ? "Failed" :
                errors >= 5 && rate >= 5 || total >= 20 && average > 2000 ? "Degraded" : "Healthy";
            return (status, FormattableString.Invariant(
                $"Recent API traffic (current plus previous four UTC minutes): {total} requests, {errors} server errors ({rate:F1}%), mean {average:F0} ms. Health probes are excluded."));
        }
    }

    private void Prune(long minute)
    {
        foreach (var key in buckets.Keys.Where(x => x < minute - 4).ToArray())
            buckets.Remove(key);
    }
}

public sealed class OperationsRequestMetricsMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, OperationsRequestMetrics metrics)
    {
        if (!context.Request.Path.StartsWithSegments("/api") ||
            context.Request.Path.StartsWithSegments("/api/operations/health"))
        {
            await next(context);
            return;
        }
        var started = Stopwatch.GetTimestamp();
        var failed = false;
        try { await next(context); }
        catch
        {
            failed = true;
            throw;
        }
        finally
        {
            metrics.Record(failed ? 500 : context.Response.StatusCode, Stopwatch.GetElapsedTime(started).TotalMilliseconds);
        }
    }
}
