namespace Arcora.Api.Services.Interfaces
{
    public class SignwellCreateResult
    {
        public string? ProviderRequestId { get; set; }
        public string? ProviderDocumentId { get; set; }
        public Dictionary<string, string>? SignerIdsByEmail { get; set; }
    }

    public class SignwellSignerStatusResult
    {
        public string? ProviderSignerId { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Status { get; set; }
        public DateTimeOffset? ViewedAt { get; set; }
        public DateTimeOffset? SignedAt { get; set; }
        public DateTimeOffset? DeclinedAt { get; set; }
    }

    public class SignwellDocumentStatusResult
    {
        public string? ProviderRequestId { get; set; }
        public string? ProviderDocumentId { get; set; }
        public string? Status { get; set; }
        public string? DocumentUrl { get; set; }
        public DateTimeOffset? CompletedAt { get; set; }
        public List<SignwellSignerStatusResult> Signers { get; set; } = new();
    }

    public class SignwellSignerRequest
    {
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public int? Order { get; set; }
        public int Page { get; set; } = 1;
        public decimal X { get; set; }
        public decimal Y { get; set; }
        public decimal Width { get; set; } = 180;
        public decimal Height { get; set; } = 45;
    }

    public interface ISignwellClient
    {
        /// <summary>
        /// Creates a signing request on the provider using the provided PDF bytes and signers.
        /// </summary>
        Task<SignwellCreateResult> CreateSigningRequestAsync(
            Guid leaseId,
            byte[] pdfBytes,
            string fileName,
            IEnumerable<SignwellSignerRequest> signers,
            bool sequential,
            string? subject = null,
            string? message = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves the current provider-side status for a signing document.
        /// </summary>
        Task<SignwellDocumentStatusResult> GetDocumentStatusAsync(string providerDocumentId, CancellationToken cancellationToken = default);
    }
}
