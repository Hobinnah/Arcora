namespace Arcora.Api.Configurations
{
    /// <summary>
    /// Configuration for Azure Blob Storage used to store files, documents and images.
    /// </summary>
    public class BlobStorageConfiguration
    {
        /// <summary>
        /// Connection string to the storage account.
        /// For local development with Azurite this can be "UseDevelopmentStorage=true".
        /// Leave empty in production to authenticate with a managed identity via <see cref="AccountName"/>.
        /// </summary>
        public string? ConnectionString { get; set; }

        /// <summary>
        /// Storage account name. Used together with managed identity (DefaultAzureCredential)
        /// when <see cref="ConnectionString"/> is not provided.
        /// </summary>
        public string? AccountName { get; set; }

        /// <summary>
        /// Container used for image blobs.
        /// </summary>
        public string ImagesContainer { get; set; } = "images";

        /// <summary>
        /// Container used for document blobs.
        /// </summary>
        public string DocumentsContainer { get; set; } = "documents";

        /// <summary>
        /// Container used for general file blobs.
        /// </summary>
        public string FilesContainer { get; set; } = "files";

        /// <summary>
        /// Lifetime, in minutes, of generated read SAS URLs.
        /// </summary>
        public int SasExpiryMinutes { get; set; } = 43200;
    }
}
