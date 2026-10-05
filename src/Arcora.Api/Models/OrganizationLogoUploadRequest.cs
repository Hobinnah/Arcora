using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Arcora.Api.Models
{
    /// <summary>
    /// Multipart payload used to upload an organization brand logo.
    /// </summary>
    public class OrganizationLogoUploadRequest
    {
        /// <summary>
        /// The logo image file to upload.
        /// </summary>
        [Required]
        public IFormFile? File { get; set; }

        /// <summary>
        /// Optional user/display name for audit metadata.
        /// </summary>
        [MaxLength(100)]
        public string? UpdatedBy { get; set; }
    }
}
