// ===================================THIS FILE WAS AUTO GENERATED===================================
namespace Arcora.Api.Configurations
{
    /// <summary>
    /// Represents the configuration for caching in the application.
    /// </summary>
    public class CacheConfiguration
    {
        /// <summary>
        /// Gets or sets the name of the cache.
        /// </summary>
        public string CacheName { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the expiration time for the cache in seconds.
        /// </summary>
        public int ExpirationTimeInMinutes { get; set; } = 60;
    }
}