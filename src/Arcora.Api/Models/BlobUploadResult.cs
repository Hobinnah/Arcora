namespace Arcora.Api.Models
{
    /// <summary>
    /// Result of a blob upload operation.
    /// </summary>
    public class BlobUploadResult
    {
        /// <summary>
        /// The name of the container the blob was stored in.
        /// </summary>
        public string Container { get; set; } = string.Empty;

        /// <summary>
        /// The unique blob name (key) within the container.
        /// </summary>
        public string BlobName { get; set; } = string.Empty;

        /// <summary>
        /// The canonical (non-SAS) blob URI.
        /// </summary>
        public string Uri { get; set; } = string.Empty;

        /// <summary>
        /// The content type stored with the blob.
        /// </summary>
        public string? ContentType { get; set; }

        /// <summary>
        /// The size, in bytes, of the uploaded content.
        /// </summary>
        public long Size { get; set; }
    }
}
