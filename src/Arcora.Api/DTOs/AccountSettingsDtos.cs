using System.ComponentModel.DataAnnotations;

namespace Arcora.Api.DTOs;

public sealed class AccountProfileUpdateDto
{
    [MaxLength(100)]
    public string? FirstName { get; set; }

    [MaxLength(100)]
    public string? LastName { get; set; }

    [Phone]
    [MaxLength(30)]
    public string? Phone { get; set; }
}

public sealed class AccountSettingsDto
{
    public bool EmailNotifications { get; set; } = true;
    public bool SmsNotifications { get; set; }

    [Required]
    [MaxLength(20)]
    public string Language { get; set; } = "English";

    [Required]
    [MaxLength(100)]
    public string Timezone { get; set; } = "America/Regina";

    [Required]
    [StringLength(3, MinimumLength = 3)]
    public string Currency { get; set; } = "CAD";
}
