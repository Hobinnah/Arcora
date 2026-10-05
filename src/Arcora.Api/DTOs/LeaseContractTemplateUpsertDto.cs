using System.ComponentModel.DataAnnotations;

namespace Arcora.Api.DTOs;

public class LeaseContractTemplateUpsertDto
{
    [Required]
    public Guid OrganizationID { get; set; }

    [Required]
    [MaxLength(150)]
    public string? TemplateName { get; set; }

    [MaxLength(1000)]
    public string? Description { get; set; }

    [Required]
    public string? HtmlContent { get; set; }

    public bool IsDefault { get; set; }
    public bool IsActive { get; set; } = true;

    [MaxLength(250)]
    public string? ChangeSummary { get; set; }

    /// <summary>
    /// Section numbers the landlord intends to edit. Allowed sections are 1-5 and 31-34.
    /// </summary>
    public List<int>? EditedSectionNumbers { get; set; }

    [MaxLength(100)]
    public string? UpdatedBy { get; set; }
}
