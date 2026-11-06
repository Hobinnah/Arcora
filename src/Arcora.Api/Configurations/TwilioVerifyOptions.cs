namespace Arcora.Api.Configurations;

public sealed class TwilioVerifyOptions
{
    public string AccountSid { get; set; } = string.Empty;
    public string AuthToken { get; set; } = string.Empty;
    public string ServiceSid { get; set; } = string.Empty;
    public string Channel { get; set; } = "sms";
}
