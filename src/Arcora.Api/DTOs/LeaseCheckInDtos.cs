using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Arcora.Api.DTOs;

public class LeaseCheckInResponseDto
{
    public Guid LeaseID { get; set; }
    public LeaseCheckInEligibilityDto Eligibility { get; set; } = new();
    public LeaseCheckInInspectionDto? Inspection { get; set; }
}

public class LeaseCheckInEligibilityDto
{
    public bool CanSubmit { get; set; }
    public string? ReasonCode { get; set; }
    public string? Message { get; set; }
    public DateOnly MoveInDate { get; set; }
    public string? TimeZone { get; set; }
}

public class LeaseCheckInDraftResponseDto
{
    public Guid InspectionID { get; set; }
    public string Version { get; set; } = string.Empty;
}

public class LeaseCheckInInspectionDto
{
    public Guid InspectionID { get; set; }
    public string InspectionType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? OccupancyReadiness { get; set; }
    public DateTime? ArrivalConfirmedAt { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public string? Notes { get; set; }
    public string Version { get; set; } = string.Empty;
    public bool RequiresFollowUp { get; set; }
    public List<LeaseCheckInItemDto> Items { get; set; } = new();
    public List<LeaseCheckInAttachmentDto> Attachments { get; set; } = new();
    public List<InspectionReviewRecordDto> Reviews { get; set; } = new();
}

public class LeaseCheckInItemDto
{
    public Guid InspectionItemID { get; set; }
    public string Area { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public string? Condition { get; set; }
    public string? Notes { get; set; }
    public bool? RequiresRepair { get; set; }
    public List<LeaseCheckInAttachmentDto> Attachments { get; set; } = new();
}

public class LeaseCheckInAttachmentDto
{
    public Guid AttachmentID { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string? MimeType { get; set; }
    public long? FileSizeBytes { get; set; }
    public string? Description { get; set; }
    public string? DownloadUrl { get; set; }
    public DateTime? ExpiresAt { get; set; }
}

public class LeaseCheckInPhotoUploadRequestDto
{
    [Required]
    public Guid InspectionID { get; set; }

    [Required]
    public IFormFile? File { get; set; }

    [MaxLength(1000)]
    public string? Description { get; set; }

    [Required]
    [MaxLength(100)]
    public string ClientPhotoID { get; set; } = string.Empty;
}

public class LeaseCheckInPhotoUploadResponseDto
{
    public Guid AttachmentID { get; set; }
    public string? FileName { get; set; }
    public string? MimeType { get; set; }
    public long? FileSizeBytes { get; set; }
    public string? Description { get; set; }
    public string? DownloadUrl { get; set; }
    public DateTime? ExpiresAt { get; set; }
}

public class SubmitLeaseCheckInRequestDto
{
    [Required]
    public Guid InspectionID { get; set; }

    [Required]
    [MaxLength(64)]
    public string Version { get; set; } = string.Empty;

    public bool ArrivalConfirmed { get; set; }

    [Required]
    [MaxLength(50)]
    public string OccupancyReadiness { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Notes { get; set; }

    public List<SubmitLeaseCheckInPhotoDto> Photos { get; set; } = new();

    public List<SubmitLeaseCheckInItemDto> Items { get; set; } = new();
}

public class SubmitLeaseCheckInPhotoDto
{
    [Required]
    public Guid AttachmentID { get; set; }

    [MaxLength(1000)]
    public string? Description { get; set; }
}

public class SubmitLeaseCheckInItemDto
{
    [Required]
    [MaxLength(100)]
    public string ClientItemID { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Area { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string ItemName { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? Condition { get; set; }

    [MaxLength(1000)]
    public string? Notes { get; set; }

    public List<Guid> AttachmentIDs { get; set; } = new();
}

public class InspectionReviewRequestDto
{
    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Comments { get; set; }
}

public class InspectionReviewRecordDto
{
    public Guid InspectionReviewID { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Comments { get; set; }
    public long ReviewedByUserID { get; set; }
    public DateTime ReviewedAt { get; set; }
}
