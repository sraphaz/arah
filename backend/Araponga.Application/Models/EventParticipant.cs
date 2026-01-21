using Araponga.Domain.Events;

namespace Araponga.Application.Models;

public sealed record EventParticipant(
    Guid UserId,
    string DisplayName,
    EventParticipationStatus Status,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc);
