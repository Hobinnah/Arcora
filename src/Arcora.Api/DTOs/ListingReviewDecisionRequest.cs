using System.ComponentModel.DataAnnotations;

namespace Arcora.Api.DTOs;

public sealed class ListingReviewDecisionRequest
{
    [Required]
    [RegularExpression("^(APPROVED|REJECTED)$", ErrorMessage = "Decision must be APPROVED or REJECTED.")]
    public string Decision { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Message { get; set; }
}