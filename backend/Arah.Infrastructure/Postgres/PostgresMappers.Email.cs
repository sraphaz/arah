using Arah.Domain.Email;
using Arah.Infrastructure.Postgres.Entities;

namespace Arah.Infrastructure.Postgres;

public static partial class PostgresMappers
{
    public static EmailQueueItemRecord ToRecord(this EmailQueueItem item)
    {
        return new EmailQueueItemRecord
        {
            Id = item.Id,
            To = item.To,
            Subject = item.Subject,
            Body = item.Body,
            IsHtml = item.IsHtml,
            TemplateName = item.TemplateName,
            TemplateDataJson = item.TemplateDataJson,
            Priority = (int)item.Priority,
            ScheduledFor = item.ScheduledFor,
            Attempts = item.Attempts,
            Status = (int)item.Status,
            CreatedAtUtc = item.CreatedAtUtc,
            ProcessedAtUtc = item.ProcessedAtUtc,
            ErrorMessage = item.ErrorMessage,
            NextRetryAtUtc = item.NextRetryAtUtc
        };
    }

    public static EmailQueueItem ToDomain(this EmailQueueItemRecord record)
    {
        // Usar construtor privado via reflection ou criar via método factory
        // Por enquanto, vamos criar usando o construtor público e depois restaurar estado
        var item = new EmailQueueItem(
            record.Id,
            record.To,
            record.Subject,
            record.Body ?? string.Empty,
            record.IsHtml,
            record.TemplateName,
            record.TemplateDataJson,
            (EmailQueuePriority)record.Priority,
            record.ScheduledFor);

        // Restaurar estado usando método público
        item.RestoreState(
            (EmailQueueStatus)record.Status,
            record.Attempts,
            record.ErrorMessage,
            record.NextRetryAtUtc,
            record.ProcessedAtUtc,
            record.CreatedAtUtc);

        return item;
    }
}
