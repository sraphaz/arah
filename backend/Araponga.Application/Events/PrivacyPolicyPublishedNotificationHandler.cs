using System.Text.Json;
using System.Text.Json.Serialization;
using Araponga.Application.Interfaces;
using Araponga.Application.Models;
using Araponga.Application.Services;

namespace Araponga.Application.Events;

/// <summary>
/// Notifica usuários quando uma nova versão de Política de Privacidade é publicada e é obrigatória para eles.
/// </summary>
public sealed class PrivacyPolicyPublishedNotificationHandler : IEventHandler<PrivacyPolicyPublishedEvent>
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

    private readonly PolicyRequirementService _policyRequirementService;
    private readonly IPrivacyPolicyRepository _policyRepository;
    private readonly IOutbox _outbox;

    public PrivacyPolicyPublishedNotificationHandler(
        PolicyRequirementService policyRequirementService,
        IPrivacyPolicyRepository policyRepository,
        IOutbox outbox)
    {
        _policyRequirementService = policyRequirementService;
        _policyRepository = policyRepository;
        _outbox = outbox;
    }

    public async Task HandleAsync(PrivacyPolicyPublishedEvent appEvent, CancellationToken cancellationToken)
    {
        var policy = await _policyRepository.GetByIdAsync(appEvent.PolicyId, cancellationToken);
        if (policy is null || !policy.IsActive)
        {
            return;
        }

        // Similar ao handler de Terms, criamos uma notificação broadcast
        var payload = new NotificationDispatchPayload(
            "privacy.published",
            new List<Guid>(), // Lista vazia = broadcast (todos os usuários)
            "Nova Política de Privacidade",
            $"Uma nova versão da Política de Privacidade ({appEvent.Version}) foi publicada e precisa ser aceita.",
            new Dictionary<string, string>
            {
                ["policyId"] = appEvent.PolicyId.ToString(),
                ["version"] = appEvent.Version
            });

        var message = new OutboxMessage(
            Guid.NewGuid(),
            "notification.dispatch",
            JsonSerializer.Serialize(payload, JsonOptions),
            appEvent.OccurredAtUtc);

        await _outbox.EnqueueAsync(message, cancellationToken);
    }
}
