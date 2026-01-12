using System.Text.Json;
using Araponga.Application.Interfaces;
using Araponga.Application.Models;

namespace Araponga.Application.Events;

public sealed class PostCreatedNotificationHandler : IEventHandler<PostCreatedEvent>
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly ITerritoryMembershipRepository _membershipRepository;
    private readonly IOutbox _outbox;

    public PostCreatedNotificationHandler(
        ITerritoryMembershipRepository membershipRepository,
        IOutbox outbox)
    {
        _membershipRepository = membershipRepository;
        _outbox = outbox;
    }

    public async Task HandleAsync(PostCreatedEvent appEvent, CancellationToken cancellationToken)
    {
        var recipients = await _membershipRepository.ListResidentUserIdsAsync(appEvent.TerritoryId, cancellationToken);
        var filteredRecipients = recipients
            .Where(userId => userId != appEvent.AuthorUserId)
            .Distinct()
            .ToList();

        if (filteredRecipients.Count == 0)
        {
            return;
        }

        var payload = new NotificationDispatchPayload(
            "post.created",
            filteredRecipients,
            "Novo post publicado",
            "Um novo post foi publicado no território.",
            new Dictionary<string, string>
            {
                ["postId"] = appEvent.PostId.ToString(),
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
