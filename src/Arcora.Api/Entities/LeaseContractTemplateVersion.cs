using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arcora.Api.Entities;

/// <summary>
/// Stores immutable versions for lease contract template revisions.
/// </summary>
[Table("LeaseContractTemplateVersion")]
public class LeaseContractTemplateVersion
{
    [Key]
    public Guid LeaseContractTemplateVersionID { get; set; }

    [Required]
    public Guid LeaseContractTemplateID { get; set; }

    [Required]
    public int VersionNumber { get; set; }

    [Required]
    [Column(TypeName = "nvarchar(max)")]
    public string? HtmlContent { get; set; }

    [MaxLength(250)]
    public string? ChangeSummary { get; set; }

    public DateTime? CapturedDate { get; set; }

    [MaxLength(100)]
    public string? CapturedBy { get; set; }

    [ForeignKey(nameof(LeaseContractTemplateID))]
    public LeaseContractTemplate? LeaseContractTemplate { get; set; }
}
