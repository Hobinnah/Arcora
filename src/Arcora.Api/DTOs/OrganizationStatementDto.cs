// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;
using Arcora.Api.Entities;

namespace Arcora.Api.DTOs;
/// <summary>
/// Summarizes financial statements for organizations including income, expenses, and net amount for a period.
/// </summary>
public class OrganizationStatementDto
{
    /// <summary>
    /// Key
    /// </summary>
    public Guid? OrganizationStatementID { get; set; }

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
    public OrganizationDto? Organization { get; set; }
}