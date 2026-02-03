namespace Araponga.Application.Events;

/// <summary>
/// Publicado quando um usuário aceita uma solicitação de conexão.
/// O solicitante (RequesterUserId) deve ser notificado.
/// </summary>
public sealed record ConnectionAcceptedEvent(
    Guid ConnectionId,
    Guid RequesterUserId,
    Guid AcceptorUserId,
    DateTime OccurredAtUtc) : IAppEvent;
