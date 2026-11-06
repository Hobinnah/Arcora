using System.ComponentModel.DataAnnotations;

namespace Arcora.Api.DTOs;

public sealed class ConfirmPhoneNumberRequest
{
    [Required]
    [StringLength(10, MinimumLength = 4)]
    public string Code { get; set; } = string.Empty;
}

public sealed class PhoneVerificationStatusDto
{
    public string Phone { get; set; } = string.Empty;
    public bool PhoneNumberConfirmed { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
