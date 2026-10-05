using System.ComponentModel.DataAnnotations;

namespace Arcora.Api.DTOs;

public class LeaseContractTemplateDto
{
    public Guid? LeaseContractTemplateID { get; set; }

    [Required]
    public Guid OrganizationID { get; set; }

    [Required]
    [MaxLength(150)]
    public string? TemplateName { get; set; }

    [MaxLength(1000)]
    public string? Description { get; set; }

    [Required]
    public string? HtmlContent { get; set; }

    [Required]
    public bool IsDefault { get; set; }

    [Required]
    public bool IsActive { get; set; } = true;

    [Required]
    public bool IsSystemGenerated { get; set; }

    public int CurrentVersionNumber { get; set; }

    public DateTime? CapturedDate { get; set; }

    public string? CapturedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }
}
