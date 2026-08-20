// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;
using Arcora.Api.Entities;

namespace Arcora.Api.DTOs;
/// <summary>
/// Contains access instructions for listings, such as door codes or special instructions.
/// </summary>
public class ListingAccessInstructionDto
{
    /// <summary>
    /// Key
    /// </summary>
    public Guid? ListingAccessInstructionID { get; set; }

    /// <summary>
    /// FK to Listing
    /// </summary>
    [Required]
    public Guid ListingID { get; set; }
    /// <summary>
    /// FK to Lease
    /// </summary>
    public Guid? LeaseID { get; set; }

    /// <summary>
    /// Type of instruction
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string? InstructionType { get; set; }

    /// <summary>
    /// Detailed instructions
    /// </summary>
    [MaxLength(256)]
    public string? Instructions { get; set; }

    /// <summary>
    /// Reference to secret or secure info
    /// </summary>
    [MaxLength(500)]
    public string? SecretReference { get; set; }
    /// <summary>
    /// Start date/time when instruction is valid
    /// </summary>
    public DateTime? AvailableFrom { get; set; }
    /// <summary>
    /// End date/time when instruction is valid
    /// </summary>
    public DateTime? AvailableUntil { get; set; }

    /// <summary>
    /// Indicates if instruction is active
    /// </summary>
    [Required]
    public bool IsActive { get; set; }
    /// <summary>
    /// Date when record was captured
    /// </summary>
    public DateTime? CapturedDate { get; set; }

    /// <summary>
    /// User who captured the record
    /// </summary>
    [MaxLength(100)]
    public string? CapturedBy { get; set; }
    /// <summary>
    /// Date when record was last updated
    /// </summary>
    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// User who last updated the record
    /// </summary>
    [MaxLength(100)]
    public string? UpdatedBy { get; set; }
    /// <summary>
    /// FK to Listing
    /// </summary>
    public ListingDto? Listing { get; set; }
    /// <summary>
    /// FK to Lease
    /// </summary>
    public LeaseDto? Lease { get; set; }
}