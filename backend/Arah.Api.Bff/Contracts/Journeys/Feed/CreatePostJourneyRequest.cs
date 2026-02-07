namespace Arah.Bff.Contracts.Journeys.Feed;

public sealed record CreatePostJourneyRequest(
    string Title,
    string Content,
    string Type,
    string Visibility,
    Guid TerritoryId,
    IReadOnlyList<string>? Tags,
    Guid? MapEntityId,
    IReadOnlyList<Guid>? MediaIds = null);
