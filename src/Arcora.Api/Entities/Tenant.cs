// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arcora.Api.Entities;
/// <summary>
/// Stores tenant personal and contact information.
/// </summary>
[Table("Tenant")]
public class Tenant
{
    /// <summary>
    /// Primary key
    /// </summary>
    [Key]
    public Guid TenantID { get; set; }

    /// <summary>
    /// Tenant code
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string? Code { get; set; }

    /// <summary>
    /// Tenant UserID
    /// </summary>
    [Required]
    [MaxLength(100)]
    public long UserID { get; set; }

    /// <summary>
    /// Tenant description
    /// </summary>
    [Required]
    [MaxLength(256)]
    public string? Description { get; set; }

    /// <summary>
    /// Tenant phone number
    /// </summary>
    [MaxLength(100)]
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Tenant photo URL
    /// </summary>
    [MaxLength(256)]
    public string? PhotoUrl { get; set; }
    /// <summary>
    /// Tenant date of birth
    /// </summary>
    public DateTime? DateOfBirth { get; set; }

    /// <summary>
    /// Tenant profile status
    /// </summary>
    [MaxLength(100)]
    public string? ProfileStatus { get; set; }

    /// <summary>
    /// Tenant active status
    /// </summary>
    [MaxLength(20)]
    public bool? IsActive { get; set; }

    /// <summary>
    /// Record captured by
    /// </summary>
    [MaxLength(100)]
    public string? CapturedBy { get; set; }
    /// <summary>
    /// Record captured date
    /// </summary>
    public DateTime? CapturedDate { get; set; }

    /// <summary>
    /// Record updated by
    /// </summary>
    [MaxLength(100)]
    public string? UpdatedBy { get; set; }
    /// <summary>
    /// Record updated date
    /// </summary>
    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// Navigation property for User entity.
    /// </summary>
    [ForeignKey(nameof(UserID))]
    public User? User { get; set; }
}