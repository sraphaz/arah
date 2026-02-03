using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Araponga.Infrastructure.Postgres.Entities;

[Table("email_queue_items")]
public sealed class EmailQueueItemRecord
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Required]
    [Column("to_address")]
    [MaxLength(500)]
    public string To { get; set; } = string.Empty;

    [Required]
    [Column("subject")]
    [MaxLength(500)]
    public string Subject { get; set; } = string.Empty;

    [Required]
    [Column("body")]
    public string Body { get; set; } = string.Empty;

    [Column("is_html")]
    public bool IsHtml { get; set; } = true;

    [Column("template_name")]
    [MaxLength(100)]
    public string? TemplateName { get; set; }

    [Column("template_data_json")]
    public string? TemplateDataJson { get; set; }

    [Column("priority")]
    public int Priority { get; set; } = 1; // Normal

    [Column("scheduled_for")]
    public DateTime? ScheduledFor { get; set; }

    [Column("attempts")]
    public int Attempts { get; set; } = 0;

    [Column("retry_count")]
    public int RetryCount { get; set; } = 0;

    [Column("status")]
    public int Status { get; set; } = 0; // Pending

    [Column("created_at_utc")]
    public DateTime CreatedAtUtc { get; set; }

    [Column("processed_at_utc")]
    public DateTime? ProcessedAtUtc { get; set; }

    [Column("sent_at_utc")]
    public DateTime? SentAtUtc { get; set; }

    [Column("failed_at_utc")]
    public DateTime? FailedAtUtc { get; set; }

    [Column("error_message")]
    [MaxLength(2000)]
    public string? ErrorMessage { get; set; }

    [Column("next_retry_at_utc")]
    public DateTime? NextRetryAtUtc { get; set; }
}
