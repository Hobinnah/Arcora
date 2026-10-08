using System.ComponentModel.DataAnnotations;

namespace Arcora.Api.DTOs;

public sealed class EligibleLeaseReviewDto
{
    public Guid LeaseID { get; set; }
    public string ListingTitle { get; set; } = string.Empty;
    public DateTime LeaseEndedAt { get; set; }
    public string ReviewMode { get; set; } = string.Empty;
    public string SubjectName { get; set; } = string.Empty;
    public bool HasSubmitted { get; set; }
}

public sealed class SubmitLeaseReviewDto
{
    [Required]
    public Guid LeaseID { get; set; }

    [Range(1, 5)]
    public short OverallRating { get; set; }

    [Range(1, 5)]
    public short? PaymentRating { get; set; }

    [Range(1, 5)]
    public short? CommunicationRating { get; set; }

    [Range(1, 5)]
    public short? PropertyCareRating { get; set; }

    [Range(1, 5)]
    public short? ResponsivenessRating { get; set; }

    [Range(1, 5)]
    public short? AccuracyRating { get; set; }

    [Range(1, 5)]
    public short? CleanlinessRating { get; set; }

    [Required]
    [StringLength(256, MinimumLength = 1)]
    public string ReviewBody { get; set; } = string.Empty;
}

public sealed class ReviewWorkflowResultDto
{
    public Guid RatingID { get; set; }
    public Guid LeaseID { get; set; }
    public bool IsPublic { get; set; }
    public DateTime? PublishedAt { get; set; }
    public DateTime? ReleaseAt { get; set; }
}
