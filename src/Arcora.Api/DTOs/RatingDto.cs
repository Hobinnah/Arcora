// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Arcora.Api.Entities;

namespace Arcora.Api.DTOs;
/// <summary>
/// Stores ratings and reviews related to leases, including various rating categories and review details.
/// </summary>
public class RatingDto
{
    /// <summary>
    /// Key
    /// </summary>
    public Guid? RatingID { get; set; }

    /// <summary>
    /// FK to Lease
    /// </summary>
    [Required]
    public Guid LeaseID { get; set; }

    /// <summary>
    /// FK to User
    /// </summary>
    [Required]
    public long ReviewerUserID { get; set; }

    /// <summary>
    /// Type of subject being rated
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string? SubjectType { get; set; }

    /// <summary>
    /// Reference ID of the subject
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string? SubjectReferenceID { get; set; }

    /// <summary>
    /// Overall rating score
    /// </summary>
    [Required]
    public short OverallRating { get; set; }
    /// <summary>
    /// Rating for payment
    /// </summary>
    public short? PaymentRating { get; set; }
    /// <summary>
    /// Rating for communication
    /// </summary>
    public short? CommunicationRating { get; set; }
    /// <summary>
    /// Rating for property care
    /// </summary>
    public short? PropertyCareRating { get; set; }
    /// <summary>
    /// Rating for responsiveness
    /// </summary>
    public short? ResponsivenessRating { get; set; }
    /// <summary>
    /// Rating for accuracy
    /// </summary>
    public short? AccuracyRating { get; set; }
    /// <summary>
    /// Rating for cleanliness
    /// </summary>
    public short? CleanlinessRating { get; set; }

    /// <summary>
    /// Review text body
    /// </summary>
    [MaxLength(256)]
    public string? ReviewBody { get; set; }

    /// <summary>
    /// Is review public
    /// </summary>
    [Required]
    public bool IsPublic { get; set; }
    /// <summary>
    /// Review published date
    /// </summary>
    public DateTime? PublishedAt { get; set; }
    /// <summary>
    /// Record captured date
    /// </summary>
    public DateTime? CapturedDate { get; set; }

    /// <summary>
    /// User who captured the record
    /// </summary>
    [MaxLength(256)]
    public string? CapturedBy { get; set; }
    /// <summary>
    /// Record last updated date
    /// </summary>
    public DateTime? UpdatedDate { get; set; }
    /// <summary>
    /// FK to Lease
    /// </summary>
    [JsonIgnore]
    public LeaseDto? Lease { get; set; }
    /// <summary>
    /// FK to User
    /// </summary>
    public User? ReviewerUser { get; set; }

    /// <summary>
    /// Reviewer's first name, derived from the associated user. Presentation-only.
    /// </summary>
    public string? ReviewerFirstName { get; set; }

    /// <summary>
    /// Reviewer's location (e.g. "Seattle, WA"), derived from the lease property address.
    /// Presentation-only.
    /// </summary>
    public string? ReviewerLocation { get; set; }

    /// <summary>
    /// Reviewer's profile image URL, derived from the tenant on the associated lease.
    /// Presentation-only.
    /// </summary>
    public string? ReviewerPhotoUrl { get; set; }
}