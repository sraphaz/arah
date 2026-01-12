using System.Text.Json;
using Araponga.Application.Interfaces;
using Araponga.Application.Models;
using Araponga.Domain.Users;

namespace Araponga.Application.Events;

public sealed class ReportCreatedNotificationHandler : IEventHandler<ReportCreatedEvent>
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly IUserRepository _userRepository;
    private readonly IOutbox _outbox;

    public ReportCreatedNotificationHandler(IUserRepository userRepository, IOutbox outbox)
    {
        _userRepository = userRepository;
        _outbox = outbox;
    }

    public async Task HandleAsync(ReportCreatedEvent appEvent, CancellationToken cancellationToken)
    {
        var recipients = await _userRepository.ListUserIdsByRoleAsync(UserRole.Curator, cancellationToken);
        var filteredRecipients = recipients
            .Where(userId => userId != appEvent.ReporterUserId)
            .Distinct()
            .ToList();

        if (filteredRecipients.Count == 0)
        {
            return;
        }

        var payload = new NotificationDispatchPayload(
            "report.created",
            filteredRecipients,
            "Novo relatório de moderação",
            "Um novo relatório foi criado para o território.",
            new Dictionary<string, string>
            {
                ["reportId"] = appEvent.ReportId.ToString(),
                ["territoryId"] = appEvent.TerritoryId.ToString()
            });

        var message = new OutboxMessage(
            Guid.NewGuid(),
            "notification.dispatch",
            JsonSerializer.Serialize(payload, JsonOptions),
            appEvent.OccurredAtUtc);

        await _outbox.EnqueueAsync(message, cancellationToken);
    }
}
