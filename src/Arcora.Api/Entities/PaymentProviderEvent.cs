// ===================================THIS FILE WAS AUTO GENERATED===================================
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arcora.Api.Entities;
/// <summary>
/// Stores events received from payment providers for processing and tracking.
/// </summary>
[Table("PaymentProviderEvent")]
public class PaymentProviderEvent
{
    /// <summary>
    /// Primary key
    /// </summary>
    [Key]
    public Guid PaymentProviderEventID { get; set; }

    /// <summary>
    /// Name of the payment provider
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string? ProviderName { get; set; }

    /// <summary>
    /// Identifier of the event from the provider
    /// </summary>
    [Required]
    [MaxLength(255)]
    public string? ProviderEventID { get; set; }

    /// <summary>
    /// Type of the event
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string? EventType { get; set; }

    /// <summary>
    /// Processing status of the event
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string? ProcessingStatus { get; set; } = "RECEIVED";

    /// <summary>
    /// JSON payload of the event
    /// </summary>
    [Required]
    [Column(TypeName = "nvarchar(max)")]
    public string? Payload { get; set; }

    /// <summary>
    /// Date and time event was received
    /// </summary>
    [Required]
    public DateTime ReceivedAt { get; set; }
    /// <summary>
    /// Date and time event was processed
    /// </summary>
    public DateTime? ProcessedAt { get; set; }

    /// <summary>
    /// Reason for failure if any
    /// </summary>
    [MaxLength(256)]
    public string? FailureReason { get; set; }

    /// <summary>
    /// Number of retry attempts
    /// </summary>
    [Required]
    public int RetryCount { get; set; } = 0;
    /// <summary>
    /// Date and time record was captured
    /// </summary>
    public DateTime? CapturedDate { get; set; }
}