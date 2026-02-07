namespace Arah.Bff.Contracts.Journeys.Feed;

public sealed record PostInteractionRequest(
    Guid PostId,
    Guid TerritoryId,
    string Action);
