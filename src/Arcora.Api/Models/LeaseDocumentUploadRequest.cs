using Microsoft.AspNetCore.Http;

namespace Arcora.Api.Models
{
    /// <summary>
    /// Multipart form payload for uploading a lease document (PDF) to blob storage
    /// together with its metadata.
    /// </summary>
    public class LeaseDocumentUploadRequest
    {
        /// <summary>
        /// The PDF file to upload.
        /// </summary>
        public IFormFile? File { get; set; }

        /// <summary>
        /// FK to Lease.
        /// </summary>
        public Guid? LeaseID { get; set; }

        /// <summary>
        /// FK to LeaseRenewals.
        /// </summary>
        public Guid? LeaseRenewalID { get; set; }

        /// <summary>
        /// FK to RentalApplication.
        /// </summary>
        public Guid? RentalApplicationID { get; set; }

        /// <summary>
        /// FK to Listing.
        /// </summary>
        public Guid? ListingID { get; set; }

        /// <summary>
        /// FK to Tenant.
        /// </summary>
        public Guid? TenantID { get; set; }

        /// <summary>
        /// Type of document.
        /// </summary>
        public string? DocumentType { get; set; }

        /// <summary>
        /// Status of the document.
        /// </summary>
        public string? DocumentStatus { get; set; }

        /// <summary>
        /// Indicates if this is the primary document.
        /// </summary>
        public bool IsPrimary { get; set; }

        /// <summary>
        /// Identifier of the user who captured the document.
        /// </summary>
        public string? CapturedBy { get; set; }
    }
}
