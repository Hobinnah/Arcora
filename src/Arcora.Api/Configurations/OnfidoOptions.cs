namespace Arcora.Api.Configurations;

public sealed class OnfidoOptions
{
    public string BaseUrl { get; set; } = "https://api.onfido.com/v3.6/";
    public string ApiKey { get; set; } = string.Empty;
    public string WorkflowRunId { get; set; } = string.Empty;
    public string HostedVerificationUrlTemplate { get; set; } = string.Empty;
    public string WebhookSecret { get; set; } = string.Empty;
}
