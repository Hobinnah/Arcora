// ===================================THIS FILE WAS AUTO GENERATED===================================
using Microsoft.AspNetCore.Identity;

namespace Arcora.Api.Entities
{
    public class Role : IdentityRole<long>
    {
        /// <summary>
        /// Optional description for the role's purpose.
        /// </summary>
        public string? Description { get; set; }
        /// <summary>
        /// Whether the role is active or has been soft-deleted/disabled.
        /// </summary>
        public bool Enabled { get; set; } = true;
    }
}