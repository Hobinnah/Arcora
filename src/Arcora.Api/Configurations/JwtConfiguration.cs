// ===================================THIS FILE WAS AUTO GENERATED===================================
namespace Arcora.Api.Configurations
{
    /// <summary>
    /// Represents JWT settings for authentication.
    /// </summary>
    public class JwtConfiguration
    {
        /// <summary>
        /// The secret key used for signing JWT tokens.
        /// </summary>
        public string? Key { get; set; }
        /// <summary>
        /// The audience for the JWT token.
        /// </summary>
        public string? Audience { get; set; }
        /// <summary>
        /// The issuer of the JWT token.
        /// </summary>
        public string? Issuer { get; set; }
        /// <summary>
        /// The lifespan (in minutes) of the JWT token.
        /// </summary>
        public int Lifespan { get; set; } = 15;
    }
}