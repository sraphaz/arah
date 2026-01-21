namespace Araponga.Api.Contracts.Events;

public sealed record EventParticipantResponse(
    Guid UserId,
    string DisplayName,
    string Status,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc);
