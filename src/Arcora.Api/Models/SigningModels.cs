using System.ComponentModel.DataAnnotations;

namespace Arcora.Api.Models
{
    public class SigningPartyDto
    {
        [Required]
        public string Name { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        public string? Role { get; set; }
        public long? UserId { get; set; }
        public Guid? TenantId { get; set; }
        public Guid? OrganizationMemberId { get; set; }
        public int? SignatureOrder { get; set; }

        // Optional explicit signature placement (PDF coordinate space)
        public int? SignaturePage { get; set; }
        public decimal? SignatureX { get; set; }
        public decimal? SignatureY { get; set; }
        public decimal? SignatureWidth { get; set; }
        public decimal? SignatureHeight { get; set; }
    }

    public class StartSigningRequestDto
    {
        public Guid? LeaseContractTemplateId { get; set; }
        public List<SigningPartyDto> Signatories { get; set; } = new();
        public bool Sequential { get; set; } = false;
        public string? Subject { get; set; }
        public string? Message { get; set; }
        public string? ReturnUrl { get; set; }
        public string? WebhookCallbackUrl { get; set; }
        public string? CapturedBy { get; set; }
        public bool ForceRegeneratePdf { get; set; } = false;
    }

    public class StartSigningResponseDto
    {
        public Guid SigningRequestId { get; set; }
        public string Status { get; set; } = "Queued";
        public Guid LeaseId { get; set; }
    }

    // Internal queue work item
    public class SigningWorkItem
    {
        public Guid SigningRequestId { get; set; }
        public Guid LeaseId { get; set; }
        public Guid? LeaseDocumentId { get; set; }
        public Guid? LeaseContractTemplateId { get; set; }
        public List<SigningPartyDto> Signatories { get; set; } = new();
        public bool Sequential { get; set; }
        public string? Subject { get; set; }
        public string? Message { get; set; }
        public string? ReturnUrl { get; set; }
        public string? WebhookCallbackUrl { get; set; }
        public string? CapturedBy { get; set; }
    }
}
