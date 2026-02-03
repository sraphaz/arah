namespace Araponga.Infrastructure.Shared.Postgres.Entities;

public sealed class EmailQueueItemRecord
{
    public Guid Id { get; set; }
    public string To { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public bool IsHtml { get; set; } = true;
    public string? TemplateName { get; set; }
    public string? TemplateDataJson { get; set; }
    public int Priority { get; set; } = 1; // Normal
    public DateTime? ScheduledFor { get; set; }
    public int Attempts { get; set; } = 0;
    public int RetryCount { get; set; } = 0;
    public int Status { get; set; } = 0; // Pending
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? ProcessedAtUtc { get; set; }
    public DateTime? SentAtUtc { get; set; }
    public DateTime? FailedAtUtc { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime? NextRetryAtUtc { get; set; }
}
