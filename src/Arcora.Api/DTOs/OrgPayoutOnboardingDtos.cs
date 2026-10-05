using System.ComponentModel.DataAnnotations;

namespace Arcora.Api.DTOs;

public class OrgPayoutOnboardingLinkRequestDto
{
    [Required]
    public Guid OrganizationID { get; set; }

    [Required]
    [MaxLength(2048)]
    public string ReturnUrl { get; set; } = string.Empty;

    [Required]
    [MaxLength(2048)]
    public string RefreshUrl { get; set; } = string.Empty;
}

public class OrgPayoutOnboardingLinkResponseDto
{
    public Guid OrganizationID { get; set; }
    public string StripeAccountID { get; set; } = string.Empty;
    public string OnboardingUrl { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}
