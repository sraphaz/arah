using System.Text.Json;
using System.Text.Json.Serialization;
using Araponga.Application.Interfaces;
using Araponga.Application.Models;
using Araponga.Application.Services;

namespace Araponga.Application.Events;

/// <summary>
/// Notifica usuários quando uma nova versão de Termos de Serviço é publicada e é obrigatória para eles.
/// </summary>
public sealed class TermsOfServicePublishedNotificationHandler : IEventHandler<TermsOfServicePublishedEvent>
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
    private readonly ITermsOfServiceRepository _termsRepository;
    private readonly IOutbox _outbox;

    public TermsOfServicePublishedNotificationHandler(
        PolicyRequirementService policyRequirementService,
        ITermsOfServiceRepository termsRepository,
        IOutbox outbox)
    {
        _policyRequirementService = policyRequirementService;
        _termsRepository = termsRepository;
        _outbox = outbox;
    }

    public async Task HandleAsync(TermsOfServicePublishedEvent appEvent, CancellationToken cancellationToken)
    {
        var terms = await _termsRepository.GetByIdAsync(appEvent.TermsId, cancellationToken);
        if (terms is null || !terms.IsActive)
        {
            return;
        }

        // Obter todos os usuários que precisam aceitar estes termos
        // Como não temos uma lista direta, vamos enviar uma notificação genérica
        // e o frontend pode verificar as políticas obrigatórias para o usuário atual
        // Alternativamente, poderíamos iterar por todos os usuários, mas isso seria custoso
        // Por enquanto, criamos uma notificação "broadcast" que será filtrada no frontend
        
        // Para uma implementação mais eficiente, poderíamos:
        // 1. Manter um cache de usuários por papel/capability/permission
        // 2. Usar um sistema de tópicos/assinações
        // Por agora, vamos criar uma notificação que será entregue via sistema de notificações
        // e o frontend verifica se o usuário precisa aceitar

        var payload = new NotificationDispatchPayload(
            "terms.published",
            new List<Guid>(), // Lista vazia = broadcast (todos os usuários)
            "Novos Termos de Serviço",
            $"Uma nova versão dos Termos de Serviço ({appEvent.Version}) foi publicada e precisa ser aceita.",
            new Dictionary<string, string>
            {
                ["termsId"] = appEvent.TermsId.ToString(),
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
