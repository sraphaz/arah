namespace Araponga.Application.Events;

/// <summary>
/// Publicado quando um usuário envia uma solicitação de conexão (círculo de amigos).
/// O destinatário (TargetUserId) deve ser notificado.
/// </summary>
public sealed record ConnectionRequestedEvent(
    Guid ConnectionId,
    Guid RequesterUserId,
    Guid TargetUserId,
    Guid? TerritoryId,
    DateTime OccurredAtUtc) : IAppEvent;
