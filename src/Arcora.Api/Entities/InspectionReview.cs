using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arcora.Api.Entities;

/// <summary>
/// Stores landlord review history for tenant move-in check-ins.
/// </summary>
[Table("InspectionReview")]
public class InspectionReview
{
    [Key]
    public Guid InspectionReviewID { get; set; }

    [Required]
    public Guid InspectionID { get; set; }

    [Required]
    [MaxLength(50)]
    public string? Status { get; set; }

    [MaxLength(1000)]
    public string? Comments { get; set; }

    [Required]
    public long ReviewedByUserID { get; set; }

    [Required]
    public DateTime ReviewedAt { get; set; }

    public DateTime? CapturedDate { get; set; }

    [MaxLength(100)]
    public string? CapturedBy { get; set; }

    [ForeignKey(nameof(InspectionID))]
    public Inspection? Inspection { get; set; }

    [ForeignKey(nameof(ReviewedByUserID))]
    public User? ReviewedByUser { get; set; }
}
