namespace Arcora.Api.DTOs;

public class LeaseContractRenderResultDto
{
    public Guid? LeaseContractTemplateID { get; set; }
    public string? TemplateName { get; set; }
    public bool IsFallbackTemplate { get; set; }
    public string? RawHtml { get; set; }
    public string? RenderedHtml { get; set; }
}
