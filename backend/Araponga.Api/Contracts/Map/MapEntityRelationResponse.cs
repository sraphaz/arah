namespace Araponga.Api.Contracts.Map;

public sealed record MapEntityRelationResponse(
    Guid UserId,
    Guid EntityId,
    DateTime CreatedAtUtc);
