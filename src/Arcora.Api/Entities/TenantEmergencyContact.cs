// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arcora.Api.Entities;
/// <summary>
/// Stores emergency contact details for tenants including name, relationship, and contact info.
/// </summary>
[Table("TenantEmergencyContact")]
public class TenantEmergencyContact
{
    /// <summary>
    /// Primary key
    /// </summary>
    [Key]
    public Guid TenantEmergencyContactID { get; set; }

    /// <summary>
    /// FK to Tenant
    /// </summary>
    [Required]
    public Guid TenantID { get; set; }

    /// <summary>
    /// Emergency contact name
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string? Name { get; set; }

    /// <summary>
    /// Relationship to tenant
    /// </summary>
    [MaxLength(100)]
    public string? Relationship { get; set; }

    /// <summary>
    /// Emergency contact phone number
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Emergency contact email
    /// </summary>
    [MaxLength(255)]
    public string? Email { get; set; }

    /// <summary>
    /// Is primary emergency contact
    /// </summary>
    [Required]
    public bool IsPrimary { get; set; }
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
}