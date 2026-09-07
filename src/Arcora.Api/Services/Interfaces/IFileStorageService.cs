using Arcora.Api.Enums;
using Arcora.Api.Models;

namespace Arcora.Api.Services.Interfaces
{
    /// <summary>
    /// Abstraction over the underlying blob storage provider (Azure Blob Storage / Azurite).
    /// Handles files, documents and images.
    /// </summary>
    public interface IFileStorageService
    {
        /// <summary>
        /// Uploads content to the container associated with the given category.
        /// </summary>
        /// <param name="category">Logical category that determines the target container.</param>
        /// <param name="fileName">Original file name. A unique blob name is generated from it.</param>
        /// <param name="content">The content stream to upload.</param>
        /// <param name="contentType">The MIME type to store with the blob.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The upload result including the blob name and URI.</returns>
        Task<BlobUploadResult> UploadAsync(StorageCategory category, string fileName, Stream content, string? contentType = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Downloads a blob's content.
        /// </summary>
        /// <returns>The content stream and its content type, or null when the blob does not exist.</returns>
        Task<(Stream Content, string? ContentType)?> DownloadAsync(StorageCategory category, string blobName, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a blob if it exists.
        /// </summary>
        /// <returns><c>true</c> when a blob was deleted; otherwise <c>false</c>.</returns>
        Task<bool> DeleteAsync(StorageCategory category, string blobName, CancellationToken cancellationToken = default);

        /// <summary>
        /// Generates a time-limited read-only SAS URL for the given blob.
        /// </summary>
        /// <returns>A SAS URI string, or null when the blob does not exist.</returns>
        Task<string?> GetReadSasUrlAsync(StorageCategory category, string blobName, CancellationToken cancellationToken = default);
    }
}
