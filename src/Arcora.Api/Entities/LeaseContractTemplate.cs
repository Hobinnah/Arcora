using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arcora.Api.Entities;

/// <summary>
/// Stores customizable lease contract templates per landlord organization.
/// </summary>
[Table("LeaseContractTemplate")]
public class LeaseContractTemplate
{
    [Key]
    public Guid LeaseContractTemplateID { get; set; }

    [Required]
    public Guid OrganizationID { get; set; }

    [Required]
    [MaxLength(150)]
    public string? TemplateName { get; set; }

    [MaxLength(1000)]
    public string? Description { get; set; }

    [Required]
    [Column(TypeName = "nvarchar(max)")]
    public string? HtmlContent { get; set; }

    [Required]
    public bool IsDefault { get; set; }

    [Required]
    public bool IsActive { get; set; } = true;

    [Required]
    public bool IsSystemGenerated { get; set; }

    public DateTime? CapturedDate { get; set; }

    [MaxLength(100)]
    public string? CapturedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    [MaxLength(100)]
    public string? UpdatedBy { get; set; }

    [ForeignKey(nameof(OrganizationID))]
    public Organization? Organization { get; set; }

    public List<LeaseContractTemplateVersion>? Versions { get; set; }
}
