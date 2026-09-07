// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;
using Arcora.Api.Entities;

namespace Arcora.Api.DTOs;
/// <summary>
/// Stores tenant personal and contact information.
/// </summary>
public class TenantDto
{
    /// <summary>
    /// Key
    /// </summary>
    public Guid? TenantID { get; set; }

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
    public long? UserID { get; set; }

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
    public bool? IsActive { get; set; }

    /// <summary>
    /// Indicates if the tenant has a registered pre-authorized debit (PAD)
    /// </summary>
    public bool? IsPADRegistered { get; set; }

    /// <summary>
    /// Indicates if the tenant has a registered card
    /// </summary>
    public bool? IsCardRegistered { get; set; }

    /// <summary>
    /// Indicates whether the lease contract has been reviewed
    /// </summary>
    public bool? LeaseContractReviewed { get; set; } = false;

    /// <summary>
    /// Indicates whether the applicant authorized verification
    /// </summary>
    public bool? VerificationAuthorization { get; set; } = false;

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
    public User? User { get; set; }
}