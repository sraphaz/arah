namespace Araponga.Application.Events;

/// <summary>
/// Evento disparado quando uma nova versão de Política de Privacidade é publicada e se torna ativa.
/// </summary>
public sealed record PrivacyPolicyPublishedEvent(
    Guid PolicyId,
    string Version,
    DateTime OccurredAtUtc) : IAppEvent;
