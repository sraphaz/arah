namespace Araponga.Api.Contracts.Chat;

public sealed record ParticipantResponse(
    Guid UserId,
    string Role,
    DateTime JoinedAtUtc,
    DateTime? LeftAtUtc,
    DateTime? MutedUntilUtc,
    Guid? LastReadMessageId,
    DateTime? LastReadAtUtc);

