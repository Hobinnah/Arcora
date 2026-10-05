namespace Arcora.Api.DTOs
{
    public class SigningRequestStatusDto
    {
        public Guid SigningRequestId { get; set; }
        public Guid LeaseId { get; set; }
        public Guid? LeaseDocumentId { get; set; }
        public string Status { get; set; } = "Unknown";
        public string? ProviderRequestId { get; set; }
        public string? ProviderDocumentId { get; set; }
        public string? ProviderStatus { get; set; }
        public string? DocumentUrl { get; set; }
        public DateTime? SentForSignatureAt { get; set; }
        public DateTime? FullySignedAt { get; set; }
        public bool RefreshedFromProvider { get; set; }
        public List<SigningRequestSignerStatusDto> Signatories { get; set; } = new();
    }

    public class SigningRequestSignerStatusDto
    {
        public Guid LeaseSignatoryId { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
        public string? Status { get; set; }
        public int? SignatureOrder { get; set; }
        public string? ProviderSignerId { get; set; }
        public DateTime? ViewedAt { get; set; }
        public DateTime? SignedAt { get; set; }
        public DateTime? DeclinedAt { get; set; }
    }
}
