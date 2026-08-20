// ===================================THIS FILE WAS AUTO GENERATED===================================
namespace Arcora.Api.Configurations
{
    /// <summary>
    /// Represents OIDC settings for authentication.
    /// </summary>
    public class OIDCConfiguration
    {
        /// <summary>
        /// The ClientId used for signing oidc tokens.
        /// </summary>
        public string? ClientId { get; set; }
        /// <summary>
        /// The ClientSecret for the oidc token.
        /// </summary>
        public string? ClientSecret { get; set; }
        /// <summary>
        /// The authority for the oidc token.
        /// </summary>
        public string? Authority { get; set; }
    }
}