using System.ComponentModel.DataAnnotations;

namespace Arcora.Api.DTOs;

public sealed class StartIdentityVerificationRequestDto
{
    [Required]
    public long UserID { get; set; }

    [MaxLength(50)]
    public string VerificationType { get; set; } = "IDENTITY";

    [MaxLength(100)]
    public string? ProviderName { get; set; }

    [MaxLength(500)]
    public string? RedirectUrl { get; set; }

    [MaxLength(100)]
    public string? CapturedBy { get; set; }
}

public sealed class StartIdentityVerificationResponseDto
{
    public Guid IdentityVerificationID { get; set; }
    public long UserID { get; set; }
    public string VerificationType { get; set; } = string.Empty;
    public string ProviderName { get; set; } = string.Empty;
    public string? ProviderReferenceID { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? VerificationUrl { get; set; }
    public string? ClientToken { get; set; }
    public string? FailureReason { get; set; }
    public DateTime? RequestedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
}
