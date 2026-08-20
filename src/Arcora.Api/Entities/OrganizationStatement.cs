// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arcora.Api.Entities;
/// <summary>
/// Summarizes financial statements for organizations including income, expenses, and net amount for a period.
/// </summary>
[Table("OrganizationStatement")]
public class OrganizationStatement
{
    /// <summary>
    /// Primary key
    /// </summary>
    [Key]
    public Guid OrganizationStatementID { get; set; }

    /// <summary>
    /// FK to Organization
    /// </summary>
    [Required]
    public Guid OrganizationID { get; set; }
    /// <summary>
    /// Financial period
    /// </summary>
    public int? Period { get; set; }
    /// <summary>
    /// Total income
    /// </summary>
    public decimal? Income { get; set; }
    /// <summary>
    /// Total expenses
    /// </summary>
    public decimal? Expenses { get; set; }
    /// <summary>
    /// Net amount (Income - Expenses)
    /// </summary>
    public decimal? NetAmount { get; set; }

    /// <summary>
    /// User who captured the record
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string? CapturedBy { get; set; }
    /// <summary>
    /// Date when record was captured
    /// </summary>
    public DateTime? CapturedDate { get; set; }

    /// <summary>
    /// FK to Organization
    /// </summary>
    [ForeignKey(nameof(OrganizationID))]
    public Organization? Organization { get; set; }
}