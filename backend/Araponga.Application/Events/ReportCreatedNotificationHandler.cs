using System.Text.Json;
using System.Text.Json.Serialization;
using Araponga.Application.Interfaces;
using Araponga.Application.Models;
using Araponga.Domain.Membership;
using Araponga.Domain.Users;

namespace Araponga.Application.Events;

public sealed class ReportCreatedNotificationHandler : IEventHandler<ReportCreatedEvent>
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
    private readonly IMembershipCapabilityRepository _capabilityRepository;
    private readonly ITerritoryMembershipRepository _membershipRepository;
    private readonly IOutbox _outbox;

    public ReportCreatedNotificationHandler(
        IMembershipCapabilityRepository capabilityRepository,
        ITerritoryMembershipRepository membershipRepository,
        IOutbox outbox)
    {
        _capabilityRepository = capabilityRepository;
        _membershipRepository = membershipRepository;
        _outbox = outbox;
    }

    public async Task HandleAsync(ReportCreatedEvent appEvent, CancellationToken cancellationToken)
    {
        // Buscar membershipIds com capability de Curator no território do report
        var membershipIds = await _capabilityRepository.ListMembershipIdsWithCapabilityAsync(
            MembershipCapabilityType.Curator,
            appEvent.TerritoryId,
            cancellationToken);

        // Converter membershipIds para userIds
        var userIds = new List<Guid>();
        foreach (var membershipId in membershipIds)
        {
            var membership = await _membershipRepository.GetByIdAsync(membershipId, cancellationToken);
            if (membership != null)
            {
                userIds.Add(membership.UserId);
            }
        }

        // Nota: Usamos apenas MembershipCapability para identificar curadores.
        // Não há mais fallback para UserRole, pois foi completamente removido do modelo.

        var filteredRecipients = userIds
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
