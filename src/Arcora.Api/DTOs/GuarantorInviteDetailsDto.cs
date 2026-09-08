namespace Arcora.Api.DTOs
{
    /// <summary>
    /// Details shown on the public guarantor invitation landing page (/guarantor-invite/:token).
    /// Returned from a signed, self-expiring invite token so the page can render before the guarantor
    /// chooses to accept or decline.
    /// </summary>
    public class GuarantorInviteDetailsDto
    {
        /// <summary>
        /// The guarantor's full name.
        /// </summary>
        public string? GuarantorName { get; set; }

        /// <summary>
        /// The applicant/tenant's full name.
        /// </summary>
        public string? TenantName { get; set; }

        /// <summary>
        /// The applicant/tenant's phone number.
        /// </summary>
        public string? TenantPhone { get; set; }

        /// <summary>
        /// The title of the listing being applied for.
        /// </summary>
        public string? ListingTitle { get; set; }

        /// <summary>
        /// The formatted monthly rent (including currency), e.g. "2,400.00 CAD".
        /// </summary>
        public string? MonthlyRent { get; set; }

        /// <summary>
        /// The formatted security deposit (including currency).
        /// </summary>
        public string? SecurityDeposit { get; set; }

        /// <summary>
        /// An optional cover image URL for the listing.
        /// </summary>
        public string? ListingImageUrl { get; set; }

        /// <summary>
        /// The guarantor's current status (e.g. PENDING, ACCEPTED, DECLINED). Lets the page show a
        /// "you've already responded" state when applicable.
        /// </summary>
        public string? Status { get; set; }
    }
}
