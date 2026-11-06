namespace Arcora.Api.Operations;

public sealed class OperationsOptions
{
    public bool Enabled { get; set; } = true;
    public string? InstanceId { get; set; }
    public string? WebUrl { get; set; }
    public int IntervalSeconds { get; set; } = 60;
    public int TimeoutSeconds { get; set; } = 10;
    public int PdfTimeoutSeconds { get; set; } = 60;
    public int FailureThreshold { get; set; } = 2;
    public int StuckMinutes { get; set; } = 30;
    public int PaymentPendingHours { get; set; } = 120;
    public int VerificationPendingHours { get; set; } = 48;
    public int PayoutPendingHours { get; set; } = 240;
    public bool EmailEnabled { get; set; }
    public string[] EmailRecipients { get; set; } = [];
}

public static class WorkerIds
{
    public const string Rent = "worker.rent";
    public const string Invitations = "worker.invitations";
    public const string Email = "worker.email";
    public const string Signing = "worker.signing";
    public const string CheckIn = "worker.check-in";
    public const string Cleanup = "worker.cleanup";
}

public sealed record OperationsCheck(
    string Id, string Name, string Category, string Status, string Description, DateTime CheckedAt,
    DateTime? LastSuccessAt = null, DateTime? LastStartedAt = null, DateTime? NextExpectedAt = null,
    double? DurationMs = null, int ConsecutiveFailures = 0, long Processed = 0, long Failed = 0, int? Backlog = null);

public sealed record OperationsAlert(
    Guid Id, string CheckId, string Status, string Message, DateTime OpenedAt,
    DateTime? ResolvedAt, DateTime? EmailSentAt);

public sealed record OperationsSnapshot(
    DateTime CheckedAt, string InstanceId, string OverallStatus, IReadOnlyList<OperationsCheck> Checks,
    IReadOnlyList<OperationsAlert> Alerts, bool EmailAlertsConfigured);
