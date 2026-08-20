// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;
using Arcora.Api.Entities;

namespace Arcora.Api.DTOs;
/// <summary>
/// Stores reminders for payment notifications related to invoices.
/// </summary>
public class PaymentReminderDto
{
    /// <summary>
    /// Key
    /// </summary>
    public Guid? PaymentReminderID { get; set; }

    /// <summary>
    /// FK to InvoiceMaster
    /// </summary>
    [Required]
    public Guid InvoiceMasterID { get; set; }

    /// <summary>
    /// Notification channel
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string? Channel { get; set; }

    /// <summary>
    /// Scheduled date and time for reminder
    /// </summary>
    [Required]
    public DateTime ScheduledAt { get; set; }
    /// <summary>
    /// Date and time reminder was sent
    /// </summary>
    public DateTime? SentAt { get; set; }

    /// <summary>
    /// Reminder status
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string? Status { get; set; } = "QUEUED";

    /// <summary>
    /// Subject of the reminder message
    /// </summary>
    [MaxLength(255)]
    public string? MessageSubject { get; set; }

    /// <summary>
    /// Body content of the reminder message
    /// </summary>
    [Required]
    [MaxLength(256)]
    public string? MessageBody { get; set; }
    /// <summary>
    /// Date reminder was captured
    /// </summary>
    public DateTime? CapturedDate { get; set; }

    /// <summary>
    /// User ID who captured the reminder
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string? CapturedBy { get; set; }
    /// <summary>
    /// FK to InvoiceMaster
    /// </summary>
    public InvoiceMasterDto? InvoiceMaster { get; set; }
}