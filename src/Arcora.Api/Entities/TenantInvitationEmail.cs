using System.ComponentModel.DataAnnotations;

namespace Arcora.Api.Entities;

public sealed class TenantInvitationEmail
{
    [Key]
    public Guid TenantInvitationID { get; set; }
    [Required, MaxLength(30)]
    public string Status { get; set; } = "PENDING";
    [Required]
    public string ProtectedMessage { get; set; } = string.Empty;
    public int Attempts { get; set; }
    public DateTime NextAttemptAt { get; set; }
    public DateTime? LastAttemptAt { get; set; }
    public DateTime? SentAt { get; set; }
    public TenantInvitation? TenantInvitation { get; set; }
}
