// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arcora.Api.Entities;
/// <summary>
/// Details occupants associated with rental applications including personal info, occupant type, and status.
/// </summary>
[Table("ApplicationOccupant")]
public class ApplicationOccupant
{
    /// <summary>
    /// Primary key
    /// </summary>
    [Key]
    public Guid ApplicationOccupantID { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public Guid? RentalApplicationID { get; set; }
    /// <summary>
    /// FK to Tenant
    /// </summary>
    public Guid? TenantID { get; set; }
    /// <summary>
    /// FK to User
    /// </summary>
    public long? UserID { get; set; }

    /// <summary>
    /// Occupant first name
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string? FirstName { get; set; }

    /// <summary>
    /// Occupant last name
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string? LastName { get; set; }
    /// <summary>
    /// Occupant date of birth
    /// </summary>
    public DateTime? DateOfBirth { get; set; }

    /// <summary>
    /// Occupant email address
    /// </summary>
    [MaxLength(255)]
    public string? Email { get; set; }

    /// <summary>
    /// Occupant phone number
    /// </summary>
    [MaxLength(50)]
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Type of occupant
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string? OccupantType { get; set; } = "ADULT";

    /// <summary>
    /// Is primary applicant
    /// </summary>
    [Required]
    public bool IsPrimaryApplicant { get; set; }

    /// <summary>
    /// Occupant status
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string? Status { get; set; } = "PENDING";
    /// <summary>
    /// Record creation date
    /// </summary>
    public DateTime? CapturedDate { get; set; }

    /// <summary>
    /// Record created by
    /// </summary>
    [MaxLength(100)]
    public string? CapturedBy { get; set; }
    /// <summary>
    /// Record last update date
    /// </summary>
    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// Record last updated by
    /// </summary>
    [MaxLength(100)]
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// FK to Tenant
    /// </summary>
    [ForeignKey(nameof(TenantID))]
    public Tenant? Tenant { get; set; }

    /// <summary>
    /// FK to User
    /// </summary>
    [ForeignKey(nameof(UserID))]
    public User? User { get; set; }
}