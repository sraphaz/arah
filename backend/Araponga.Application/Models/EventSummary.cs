using Araponga.Domain.Events;

namespace Araponga.Application.Models;

public sealed record EventSummary(
    TerritoryEvent Event,
    int InterestedCount,
    int ConfirmedCount,
    string? CreatedByDisplayName);
