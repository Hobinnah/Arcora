// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arcora.Api.Entities;
/// <summary>
/// Stores guarantor details for tenants including contact info, relationship, income, and status.
/// </summary>
[Table("TenantGuarantor")]
public class TenantGuarantor
{
    /// <summary>
    /// Primary key
    /// </summary>
    [Key]
    public Guid TenantGuarantorID { get; set; }

    /// <summary>
    /// FK to Tenant
    /// </summary>
    [Required]
    public Guid TenantID { get; set; }
    /// <summary>
    /// FK to User
    /// </summary>
    public long? UserID { get; set; }

    /// <summary>
    /// Guarantor first name
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string? FirstName { get; set; }

    /// <summary>
    /// Guarantor last name
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string? LastName { get; set; }

    /// <summary>
    /// Guarantor email address
    /// </summary>
    [MaxLength(255)]
    public string? Email { get; set; }

    /// <summary>
    /// Guarantor phone number
    /// </summary>
    [MaxLength(50)]
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Relationship to tenant
    /// </summary>
    [MaxLength(100)]
    public string? Relationship { get; set; }
    /// <summary>
    /// Guarantor annual income
    /// </summary>
    public decimal? AnnualIncome { get; set; }

    /// <summary>
    /// Guarantor status
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string? Status { get; set; } = "PENDING";
    /// <summary>
    /// Date invited
    /// </summary>
    public DateTime? InvitedAt { get; set; }
    /// <summary>
    /// Date accepted
    /// </summary>
    public DateTime? AcceptedAt { get; set; }
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