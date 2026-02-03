using System.Text.Json;
using System.Text.Json.Serialization;
using Araponga.Application.Interfaces;
using Araponga.Application.Models;

namespace Araponga.Application.Events;

public sealed class ConnectionRequestedNotificationHandler : IEventHandler<ConnectionRequestedEvent>
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = false,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
        MaxDepth = 64,
        ReferenceHandler = ReferenceHandler.IgnoreCycles
    };
    private readonly IOutbox _outbox;

    public ConnectionRequestedNotificationHandler(IOutbox outbox)
    {
        _outbox = outbox;
    }

    public async Task HandleAsync(ConnectionRequestedEvent appEvent, CancellationToken cancellationToken)
    {
        var payload = new NotificationDispatchPayload(
            "connection.request",
            new[] { appEvent.TargetUserId },
            "Nova solicitação de conexão",
            "Alguém quer adicionar você ao círculo de amigos.",
            new Dictionary<string, string>
            {
                ["connectionId"] = appEvent.ConnectionId.ToString(),
                ["requesterUserId"] = appEvent.RequesterUserId.ToString()
            });

        var message = new OutboxMessage(
            Guid.NewGuid(),
            "notification.dispatch",
            JsonSerializer.Serialize(payload, JsonOptions),
            appEvent.OccurredAtUtc);

        await _outbox.EnqueueAsync(message, cancellationToken);
    }
}
