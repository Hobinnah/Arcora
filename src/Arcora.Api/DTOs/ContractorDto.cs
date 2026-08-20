// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;
using Arcora.Api.Entities;

namespace Arcora.Api.DTOs;
/// <summary>
/// Represents contractors providing services to properties or organizations.
/// </summary>
public class ContractorDto
{
    /// <summary>
    /// Key
    /// </summary>
    public Guid? ContractorID { get; set; }

    /// <summary>
    /// FK to Organization
    /// </summary>
    [Required]
    public Guid OrganizationID { get; set; }

    /// <summary>
    /// Name of the contractor company
    /// </summary>
    [MaxLength(100)]
    public string? CompanyName { get; set; }

    /// <summary>
    /// Contact person name
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string? ContactName { get; set; }

    /// <summary>
    /// Email address
    /// </summary>
    [MaxLength(255)]
    public string? Email { get; set; }

    /// <summary>
    /// Phone number
    /// </summary>
    [MaxLength(50)]
    public string? PhoneNumber { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int? CategoryID { get; set; }

    /// <summary>
    /// Contractor status
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string? Status { get; set; } = "ACTIVE";

    /// <summary>
    /// Additional notes
    /// </summary>
    [MaxLength(1000)]
    public string? Notes { get; set; }
    /// <summary>
    /// Date contractor was captured
    /// </summary>
    public DateTime? CapturedDate { get; set; }

    /// <summary>
    /// User ID who captured the contractor
    /// </summary>
    [MaxLength(100)]
    public string? CapturedBy { get; set; }
    /// <summary>
    /// Date contractor was last updated
    /// </summary>
    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// User ID who last updated the contractor
    /// </summary>
    [MaxLength(100)]
    public string? UpdatedBy { get; set; }
    /// <summary>
    /// FK to Organization
    /// </summary>
    public OrganizationDto? Organization { get; set; }
}