namespace Arcora.Api.Models
{
    /// <summary>
    /// Identifies who is acting in a conversation: a tenant/prospective tenant (by <see cref="TenantID"/>)
    /// or a host organization member (by <see cref="OrganizationMemberID"/> within <see cref="OrganizationID"/>).
    /// Exactly one side should be populated.
    /// </summary>
    public sealed class ConversationActor
    {
        /// <summary>The acting application user id, when known.</summary>
        public long? UserID { get; set; }

        /// <summary>Set when the actor is the tenant/prospective tenant.</summary>
        public Guid? TenantID { get; set; }

        /// <summary>Set when the actor is a host organization member.</summary>
        public Guid? OrganizationMemberID { get; set; }

        /// <summary>The host organization the acting member belongs to.</summary>
        public Guid? OrganizationID { get; set; }

        /// <summary>True when the actor represents the tenant side.</summary>
        public bool IsTenant => TenantID.HasValue && TenantID != Guid.Empty;

        /// <summary>True when the actor represents the host side.</summary>
        public bool IsHost => OrganizationMemberID.HasValue && OrganizationMemberID != Guid.Empty;
    }
}
