namespace Araponga.Api.Contracts.Territories;

public sealed record TerritorySelectionResponse(
    string SessionId,
    Guid TerritoryId
);
