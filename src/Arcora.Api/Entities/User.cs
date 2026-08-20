// ===================================THIS FILE WAS AUTO GENERATED===================================
using Microsoft.AspNetCore.Identity;

namespace Arcora.Api.Entities
{
    public class User : IdentityUser<long>
    {
        /// <summary>
        /// Full name of the user.
        /// </summary>
        public string DisplayName { get; set; } = string.Empty;
        /// <summary>
        /// First name of the user.
        /// </summary>
        public string FirstName { get; set; } = string.Empty;
        /// <summary>
        /// Last name of the user.
        /// </summary>
        public string LastName { get; set; } = string.Empty;
        /// <summary>
        /// Client application name.
        /// </summary>
        public string? ClientName { get; set; }
        /// <summary>
        /// Client application version.
        /// </summary>
        public string? BranchName { get; set; }
    }
}