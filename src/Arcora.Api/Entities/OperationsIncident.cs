using System.ComponentModel.DataAnnotations;

namespace Arcora.Api.Entities;

public sealed class OperationsIncident
{
    public Guid Id { get; set; }
    [MaxLength(128)] public string InstanceId { get; set; } = "";
    [MaxLength(80)] public string CheckId { get; set; } = "";
    [MaxLength(32)] public string Status { get; set; } = "";
    [MaxLength(512)] public string Message { get; set; } = "";
    public DateTime OpenedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public DateTime? EmailSentAt { get; set; }
    public DateTime? RecoveryEmailSentAt { get; set; }
    public DateTime? NextEmailAttemptAt { get; set; }
    public int EmailAttempts { get; set; }
}
