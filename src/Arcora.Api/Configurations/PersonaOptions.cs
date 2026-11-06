namespace Arcora.Api.Configurations;

public sealed class PersonaOptions
{
    public string BaseUrl { get; set; } = "https://withpersona.com/api/v1/";
    public string ApiKey { get; set; } = string.Empty;
    public string TemplateId { get; set; } = string.Empty;
    public string HostedVerificationUrlTemplate { get; set; } = string.Empty;
    public string WebhookSecret { get; set; } = string.Empty;
}
