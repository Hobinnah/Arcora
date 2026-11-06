namespace Arcora.Api.Configurations;

public sealed class PhoneVerificationOptions
{
    public string Provider { get; set; } = "INFOBIP";
    public bool EnableDevelopmentOtp { get; set; }
    public int InfobipCodeLength { get; set; } = 5;
    public int InfobipCodeExpiryMinutes { get; set; } = 5;
}
