namespace Arcora.Api.Enums
{
    /// <summary>
    /// Logical category of a stored blob, used to resolve the target container.
    /// </summary>
    public enum StorageCategory
    {
        /// <summary>General files.</summary>
        File,

        /// <summary>Documents (PDFs, leases, etc.).</summary>
        Document,

        /// <summary>Images.</summary>
        Image
    }
}
