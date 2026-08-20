// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arcora.Api.Entities;
/// <summary>
/// Stores ratings and reviews related to leases, including various rating categories and review details.
/// </summary>
[Table("Rating")]
public class Rating
{
    /// <summary>
    /// Primary key
    /// </summary>
    [Key]
    public Guid RatingID { get; set; }

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
    [ForeignKey(nameof(LeaseID))]
    public Lease? Lease { get; set; }

    /// <summary>
    /// FK to User
    /// </summary>
    [ForeignKey(nameof(ReviewerUserID))]
    public User? ReviewerUser { get; set; }
}