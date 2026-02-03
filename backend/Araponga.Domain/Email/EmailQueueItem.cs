namespace Araponga.Domain.Email;

/// <summary>
/// Representa um item na fila de envio de emails.
/// </summary>
public sealed class EmailQueueItem
{
    public Guid Id { get; internal set; }
    public string To { get; internal set; } = string.Empty;
    public string Subject { get; internal set; } = string.Empty;
    public string Body { get; internal set; } = string.Empty;
    public bool IsHtml { get; internal set; } = true;
    public string? TemplateName { get; internal set; }
    public string? TemplateDataJson { get; internal set; }
    public EmailQueuePriority Priority { get; internal set; } = EmailQueuePriority.Normal;
    public DateTime? ScheduledFor { get; internal set; }
    public int Attempts { get; internal set; }
    public EmailQueueStatus Status { get; internal set; } = EmailQueueStatus.Pending;
    public DateTime CreatedAtUtc { get; internal set; }
    public DateTime? ProcessedAtUtc { get; internal set; }
    public string? ErrorMessage { get; internal set; }
    public DateTime? NextRetryAtUtc { get; internal set; }

    // Construtor privado para EF Core / mappers
    private EmailQueueItem() { }

    public EmailQueueItem(
        Guid id,
        string to,
        string subject,
        string body,
        bool isHtml = true,
        string? templateName = null,
        string? templateDataJson = null,
        EmailQueuePriority priority = EmailQueuePriority.Normal,
        DateTime? scheduledFor = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(to);
        ArgumentException.ThrowIfNullOrWhiteSpace(subject);
        // Body pode ser vazio se TemplateName for fornecido
        if (string.IsNullOrWhiteSpace(body) && string.IsNullOrWhiteSpace(templateName))
        {
            throw new ArgumentException("Body or TemplateName must be provided.", nameof(body));
        }

        Id = id;
        To = to;
        Subject = subject;
        Body = body;
        IsHtml = isHtml;
        TemplateName = templateName;
        TemplateDataJson = templateDataJson;
        Priority = priority;
        ScheduledFor = scheduledFor;
        CreatedAtUtc = DateTime.UtcNow;
        Status = EmailQueueStatus.Pending;
    }

    public void MarkAsProcessing()
    {
        Status = EmailQueueStatus.Processing;
    }

    public void MarkAsCompleted()
    {
        Status = EmailQueueStatus.Completed;
        ProcessedAtUtc = DateTime.UtcNow;
    }

    public void MarkAsFailed(string errorMessage, DateTime? nextRetryAt = null)
    {
        Attempts++;
        ErrorMessage = errorMessage;
        NextRetryAtUtc = nextRetryAt;

        if (Attempts >= 4)
        {
            Status = EmailQueueStatus.Failed;
        }
        else
        {
            Status = EmailQueueStatus.Pending;
        }
    }

    public void MarkAsDeadLetter()
    {
        Status = EmailQueueStatus.DeadLetter;
        ProcessedAtUtc = DateTime.UtcNow;
    }

    public bool ShouldRetry()
    {
        return Status == EmailQueueStatus.Pending &&
               Attempts < 4 &&
               (NextRetryAtUtc == null || NextRetryAtUtc <= DateTime.UtcNow);
    }

    // Método público para restaurar estado do banco de dados (usado por mappers)
    public void RestoreState(
        EmailQueueStatus status,
        int attempts,
        string? errorMessage,
        DateTime? nextRetryAtUtc,
        DateTime? processedAtUtc,
        DateTime createdAtUtc)
    {
        Status = status;
        Attempts = attempts;
        ErrorMessage = errorMessage;
        NextRetryAtUtc = nextRetryAtUtc;
        ProcessedAtUtc = processedAtUtc;
        CreatedAtUtc = createdAtUtc;
    }
}

/// <summary>
/// Prioridade do email na fila.
/// </summary>
public enum EmailQueuePriority
{
    Low = 0,
    Normal = 1,
    High = 2,
    Critical = 3
}

/// <summary>
/// Status do item na fila de emails.
/// </summary>
public enum EmailQueueStatus
{
    Pending = 0,
    Processing = 1,
    Completed = 2,
    Failed = 3,
    DeadLetter = 4
}
