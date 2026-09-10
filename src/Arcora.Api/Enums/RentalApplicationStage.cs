using System.ComponentModel.DataAnnotations;

namespace Arcora.Api.Enums
{
    /// <summary>
    /// The lifecycle stages a rental application moves through, from creation to a final outcome.
    /// The enum member name (e.g. "SUBMITTED") is the canonical value stored in
    /// <c>RentalApplication.Status</c> and shared with the frontend so both stay in sync.
    /// </summary>
    public enum RentalApplicationStage
    {
        /// <summary>
        /// Application has been started by the tenant but not yet submitted for review.
        /// </summary>
        [Display(Name = "Draft", Description = "The application has been started but not yet submitted.")]
        DRAFT = 0,

        /// <summary>
        /// Application has been submitted by the tenant and is awaiting the host's attention.
        /// </summary>
        [Display(Name = "Submitted", Description = "The application has been submitted and is awaiting review.")]
        SUBMITTED = 1,

        /// <summary>
        /// The host/landlord is actively reviewing the application.
        /// </summary>
        [Display(Name = "Under Review", Description = "The host is reviewing the application.")]
        UNDER_REVIEW = 2,

        /// <summary>
        /// Background/credit screening is in progress for the applicant.
        /// </summary>
        [Display(Name = "Screening", Description = "Background and credit screening is in progress.")]
        SCREENING = 3,

        /// <summary>
        /// The host has approved the application; the lease and first charge follow.
        /// </summary>
        [Display(Name = "Approved", Description = "The host has approved the application.")]
        APPROVED = 4,

        /// <summary>
        /// The host has declined the application.
        /// </summary>
        [Display(Name = "Rejected", Description = "The host has declined the application.")]
        REJECTED = 5,

        /// <summary>
        /// The tenant has withdrawn the application before a decision was made.
        /// </summary>
        [Display(Name = "Withdrawn", Description = "The tenant withdrew the application.")]
        WITHDRAWN = 6,

        /// <summary>
        /// The application has been converted into an active lease.
        /// </summary>
        [Display(Name = "Leased", Description = "The application has been converted into a lease.")]
        LEASED = 7,

        /// <summary>
        /// The application expired without progressing to a decision.
        /// </summary>
        [Display(Name = "Expired", Description = "The application expired without a decision.")]
        EXPIRED = 8
    }
}
