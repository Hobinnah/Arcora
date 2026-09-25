using System.ComponentModel.DataAnnotations;

using System.ComponentModel.DataAnnotations;

namespace Arcora.Api.DTOs;

/// <summary>
/// Represents a request to invite a cohost to help manage an organization's listings.
/// </summary>
public class CohostInvitationDto
{
    /// <summary>
    /// The organization the cohost is being invited to cohost listings for.
    /// </summary>
    [Required]
    public Guid OrganizationID { get; set; }

    /// <summary>
    /// Email address of the person being invited to cohost.
    /// </summary>
    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public string? Email { get; set; }

    /// <summary>
    /// Full name of the person being invited to cohost.
    /// </summary>
    [Required]
    [MaxLength(150)]
    public string? CohostName { get; set; }

    /// <summary>
    /// Phone number of the person being invited to cohost.
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// The level of access granted to the cohost. Allowed values:
    /// "Full access", "Calendar and message access", "Calendar access".
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string? CohostAccess { get; set; }
}

/// <summary>
/// Represents the result of a cohost invitation request.
/// </summary>
public class CohostInvitationResultDto
{
    /// <summary>
    /// Identifier of the persisted cohost invitation record.
    /// </summary>
    public Guid CohostInvitationID { get; set; }

    /// <summary>
    /// Email address that received the invitation.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Invitee full name.
    /// </summary>
    public string? CohostName { get; set; }

    /// <summary>
    /// Phone number included in the invitation.
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Normalized cohost access level applied to the invitation.
    /// </summary>
    public string? CohostAccess { get; set; }

    /// <summary>
    /// Signed invitation token.
    /// </summary>
    public string? Token { get; set; }

    /// <summary>
    /// UTC date/time when the invitation expires.
    /// </summary>
    public DateTime ExpiresAtUtc { get; set; }

    /// <summary>
    /// Current status of the invitation.
    /// </summary>
    public string? Status { get; set; }
}

/// <summary>
/// Cohost invitation record data used by host dashboards.
/// </summary>
public class CohostInvitationRecordDto
{
    public Guid CohostInvitationID { get; set; }
    public Guid OrganizationID { get; set; }
    public string? Email { get; set; }
    public string? CohostName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? CohostAccess { get; set; }
    public string? Status { get; set; }
    public DateTime ExpiresAtUtc { get; set; }
    public DateTime? AcceptedAtUtc { get; set; }
    public DateTime? DeclinedAtUtc { get; set; }
    public DateTime? CapturedDateUtc { get; set; }
    public DateTime? UpdatedDateUtc { get; set; }
}
