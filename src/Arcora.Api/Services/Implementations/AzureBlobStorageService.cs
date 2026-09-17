using Arcora.Api.Configurations;
using Arcora.Api.Enums;
using Arcora.Api.Models;
using Arcora.Api.Services.Interfaces;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Arcora.Api.Services.Implementations
{
    /// <summary>
    /// Azure Blob Storage implementation of <see cref="IFileStorageService"/>.
    /// Works against a real storage account, or against Azurite during local development.
    /// Containers are created as private; read access is granted via short-lived SAS URLs.
    /// </summary>
    public class AzureBlobStorageService : IFileStorageService
    {
        private readonly BlobServiceClient blobServiceClient;
        private readonly BlobStorageConfiguration options;
        private readonly ILogger<AzureBlobStorageService> logger;

        public AzureBlobStorageService(BlobServiceClient blobServiceClient, IOptions<BlobStorageConfiguration> options, ILogger<AzureBlobStorageService> logger)
        {
            this.blobServiceClient = blobServiceClient;
            this.options = options.Value;
            this.logger = logger;
        }

        /// <inheritdoc/>
        public async Task<BlobUploadResult> UploadAsync(StorageCategory category, string fileName, Stream content, string? contentType = null, CancellationToken cancellationToken = default)
        {
            if (content == null)
                throw new ArgumentNullException(nameof(content));

            var container = await GetContainerAsync(category, cancellationToken);
            var blobName = BuildBlobName(fileName);
            var blob = container.GetBlobClient(blobName);

            var headers = new BlobHttpHeaders
            {
                ContentType = string.IsNullOrWhiteSpace(contentType) ? "application/octet-stream" : contentType
            };

            await blob.UploadAsync(content, new BlobUploadOptions { HttpHeaders = headers }, cancellationToken);

            return new BlobUploadResult
            {
                Container = container.Name,
                BlobName = blobName,
                Uri = blob.Uri.ToString(),
                ContentType = headers.ContentType,
                Size = content.CanSeek ? content.Length : 0
            };
        }

        /// <inheritdoc/>
        public async Task<(Stream Content, string? ContentType)?> DownloadAsync(StorageCategory category, string blobName, CancellationToken cancellationToken = default)
        {
            var container = GetContainerClient(category);
            var blob = container.GetBlobClient(blobName);

            if (!await blob.ExistsAsync(cancellationToken))
                return null;

            var response = await blob.DownloadStreamingAsync(cancellationToken: cancellationToken);
            return (response.Value.Content, response.Value.Details.ContentType);
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteAsync(StorageCategory category, string blobName, CancellationToken cancellationToken = default)
        {
            var container = GetContainerClient(category);
            var blob = container.GetBlobClient(blobName);
            var response = await blob.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots, cancellationToken: cancellationToken);
            return response.Value;
        }

        /// <inheritdoc/>
        public async Task<string?> GetReadSasUrlAsync(StorageCategory category, string blobName, CancellationToken cancellationToken = default)
        {
            var container = GetContainerClient(category);
            var blob = container.GetBlobClient(blobName);

            if (!await blob.ExistsAsync(cancellationToken))
                return null;

            var expiresOn = DateTimeOffset.UtcNow.AddMinutes(options.SasExpiryMinutes <= 0 ? 43200 : options.SasExpiryMinutes);
            var builder = new BlobSasBuilder
            {
                BlobContainerName = container.Name,
                BlobName = blobName,
                Resource = "b",
                ExpiresOn = expiresOn
            };
            builder.SetPermissions(BlobSasPermissions.Read);

            // Account-key based SAS (works with connection string and Azurite).
            if (blob.CanGenerateSasUri)
            {
                var sasUri = blob.GenerateSasUri(builder).ToString();
                try
                {
                    logger.LogInformation("Generated blob SAS (masked): {Sas}", MaskSasInUrl(sasUri));
                }
                catch
                {
                    // ignore logging failures
                }
                return sasUri;
            }

            // Managed identity: fall back to a user delegation SAS.
            try
            {
                var start = DateTimeOffset.UtcNow.AddMinutes(-5);
                var userDelegationKey = await blobServiceClient.GetUserDelegationKeyAsync(start, expiresOn, cancellationToken);
                var sas = builder.ToSasQueryParameters(userDelegationKey.Value, blobServiceClient.AccountName).ToString();
                var full = $"{blob.Uri}?{sas}";
                try
                {
                    logger.LogInformation("Generated user-delegation blob SAS (masked): {Sas}", MaskSasInUrl(full));
                }
                catch
                {
                    // ignore logging failures
                }
                return full;
            }
            catch (Exception er)
            {
                logger.LogError(er, "Failed to generate a user delegation SAS for blob {BlobName}. Timestamp: {Timestamp}", blobName, DateTime.UtcNow);
                return null;
            }
        }

        private static string MaskSasInUrl(string url)
        {
            if (string.IsNullOrEmpty(url)) return url ?? string.Empty;
            try
            {
                var idx = url.IndexOf('?');
                if (idx == -1) return url;
                var baseUrl = url.Substring(0, idx + 1);
                var qs = url.Substring(idx + 1);
                // mask sig and se values
                var parts = qs.Split('&');
                for (int i = 0; i < parts.Length; i++)
                {
                    if (parts[i].StartsWith("sig=", StringComparison.OrdinalIgnoreCase))
                    {
                        parts[i] = "sig=****";
                    }
                    else if (parts[i].StartsWith("se=", StringComparison.OrdinalIgnoreCase))
                    {
                        parts[i] = "se=****";
                    }
                    else if (parts[i].StartsWith("sv=", StringComparison.OrdinalIgnoreCase))
                    {
                        parts[i] = "sv=****";
                    }
                }
                return baseUrl + string.Join('&', parts);
            }
            catch
            {
                return "****";
            }
        }

        private async Task<BlobContainerClient> GetContainerAsync(StorageCategory category, CancellationToken cancellationToken)
        {
            var container = GetContainerClient(category);
            // Private container (no public access).
            await container.CreateIfNotExistsAsync(PublicAccessType.None, cancellationToken: cancellationToken);
            return container;
        }

        private BlobContainerClient GetContainerClient(StorageCategory category)
        {
            var name = category switch
            {
                StorageCategory.Image => options.ImagesContainer,
                StorageCategory.Document => options.DocumentsContainer,
                _ => options.FilesContainer
            };
            return blobServiceClient.GetBlobContainerClient(name);
        }

        private static string BuildBlobName(string fileName)
        {
            var extension = Path.GetExtension(fileName);
            var datePrefix = DateTime.UtcNow.ToString("yyyy/MM/dd");
            return $"{datePrefix}/{Guid.NewGuid():N}{extension}";
        }
    }
}
