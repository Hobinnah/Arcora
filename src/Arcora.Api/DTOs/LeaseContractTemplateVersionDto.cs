namespace Arcora.Api.DTOs;

public class LeaseContractTemplateVersionDto
{
    public Guid? LeaseContractTemplateVersionID { get; set; }
    public Guid LeaseContractTemplateID { get; set; }
    public int VersionNumber { get; set; }
    public string? HtmlContent { get; set; }
    public string? ChangeSummary { get; set; }
    public DateTime? CapturedDate { get; set; }
    public string? CapturedBy { get; set; }
}
