using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Arcora.Api.DTOs;

public class HostSecurityDepositListItemDto
{
    public Guid SecurityDepositID { get; set; }
    public Guid? LeaseID { get; set; }
    public Guid TenantID { get; set; }
    public Guid OrganizationID { get; set; }
    public decimal RequiredAmount { get; set; }
    public decimal ReceivedAmount { get; set; }
    public decimal AppliedAmount { get; set; }
    public decimal ReturnedAmount { get; set; }
    public decimal HeldAmount { get; set; }
    public string? Currency { get; set; }
    public string? Status { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? HeldAt { get; set; }
    public DateTime? ClosedAt { get; set; }
    public string? TenantDisplayName { get; set; }
    public string? TenantEmail { get; set; }
    public string? LeaseDisplay { get; set; }
    public string? ListingDisplay { get; set; }
}

public class HostSecurityDepositSummaryDto
{
    public decimal HeldAmountTotal { get; set; }
    public decimal ReturnsDueAmountTotal { get; set; }
    public int ReviewNeededCount { get; set; }
}

public class HostSecurityDepositListResponseDto
{
    public IEnumerable<HostSecurityDepositListItemDto> Data { get; set; } = Enumerable.Empty<HostSecurityDepositListItemDto>();
    public int TotalCount { get; set; }
    public HostSecurityDepositSummaryDto Summary { get; set; } = new();
}

public class SecurityDepositEvidenceUploadResponseDto
{
    public Guid AttachmentID { get; set; }
    public string? FileName { get; set; }
    public string? MimeType { get; set; }
    public long? FileSizeBytes { get; set; }
    public string? StorageReference { get; set; }
}

public class SecurityDepositEvidenceUploadRequestDto
{
    [Required]
    public IFormFile? File { get; set; }
}

public class SecurityDepositSettlementNoticeRequestDto
{
    [Required]
    public List<SecurityDepositDeductionDto> Deductions { get; set; } = new();

    [Required]
    [MaxLength(3)]
    public string Currency { get; set; } = "CAD";
}

public class SecurityDepositDeductionDto
{
    [Required]
    [MaxLength(255)]
    public string Description { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue)]
    public decimal Amount { get; set; }

    public List<Guid> EvidenceAttachmentIDs { get; set; } = new();
}

public class SecurityDepositSettlementNoticeResponseDto
{
    public Guid SecurityDepositID { get; set; }
    public decimal DeductionsTotal { get; set; }
    public decimal ReturnAmount { get; set; }
    public string? Currency { get; set; }
    public DateTime SentAtUtc { get; set; }
    public string? RecipientEmail { get; set; }
    public string? Status { get; set; }
}

public class SecurityDepositReturnRequestDto
{
    [Range(0.01, double.MaxValue)]
    public decimal Amount { get; set; }

    [Required]
    [MaxLength(3)]
    public string Currency { get; set; } = "CAD";

    [Required]
    [MaxLength(200)]
    public string IdempotencyKey { get; set; } = string.Empty;
}

public class SecurityDepositReturnResponseDto
{
    public Guid SecurityDepositID { get; set; }
    public Guid RefundID { get; set; }
    public Guid SecurityDepositTransactionID { get; set; }
    public decimal ReturnedAmount { get; set; }
    public string? Status { get; set; }
    public bool IdempotentReplay { get; set; }
}
