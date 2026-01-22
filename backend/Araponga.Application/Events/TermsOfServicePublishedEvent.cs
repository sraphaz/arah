namespace Araponga.Application.Events;

/// <summary>
/// Evento disparado quando uma nova versão de Termos de Serviço é publicada e se torna ativa.
/// </summary>
public sealed record TermsOfServicePublishedEvent(
    Guid TermsId,
    string Version,
    DateTime OccurredAtUtc) : IAppEvent;
