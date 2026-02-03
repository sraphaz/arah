using System.Text.Json;
using System.Text.Json.Serialization;
using Araponga.Application.Interfaces;
using Araponga.Application.Models;

namespace Araponga.Application.Events;

public sealed class ConnectionAcceptedNotificationHandler : IEventHandler<ConnectionAcceptedEvent>
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

    public ConnectionAcceptedNotificationHandler(IOutbox outbox)
    {
        _outbox = outbox;
    }

    public async Task HandleAsync(ConnectionAcceptedEvent appEvent, CancellationToken cancellationToken)
    {
        var payload = new NotificationDispatchPayload(
            "connection.accepted",
            new[] { appEvent.RequesterUserId },
            "Solicitação de conexão aceita",
            "Sua solicitação para o círculo de amigos foi aceita.",
            new Dictionary<string, string>
            {
                ["connectionId"] = appEvent.ConnectionId.ToString(),
                ["acceptorUserId"] = appEvent.AcceptorUserId.ToString()
            });

        var message = new OutboxMessage(
            Guid.NewGuid(),
            "notification.dispatch",
            JsonSerializer.Serialize(payload, JsonOptions),
            appEvent.OccurredAtUtc);

        await _outbox.EnqueueAsync(message, cancellationToken);
    }
}
