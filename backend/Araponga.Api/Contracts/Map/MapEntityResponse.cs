namespace Araponga.Api.Contracts.Map;

public sealed record MapEntityResponse(
    Guid Id,
    string Name,
    string Category,
    string Visibility,
    DateTime CreatedAtUtc
);
