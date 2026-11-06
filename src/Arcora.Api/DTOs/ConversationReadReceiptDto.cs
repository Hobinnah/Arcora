namespace Arcora.Api.DTOs
{
    /// <summary>
    /// How far one participant has read a thread. Exactly one of <see cref="TenantID"/> or
    /// <see cref="OrganizationMemberID"/> identifies the reader's side.
    /// </summary>
    public sealed class ConversationReadReceiptDto
    {
        public Guid ConversationID { get; set; }
        public Guid? TenantID { get; set; }
        public Guid? OrganizationMemberID { get; set; }
        public string? FirstName { get; set; }
        public DateTime LastReadAt { get; set; }
    }
}
