namespace Araponga.Api.Contracts.Map;

public sealed record MapEntityResponse(
    Guid Id,
    string Name,
    string Category,
    double Latitude,
    double Longitude,
    string Status,
    string Visibility,
    int ConfirmationCount,
    DateTime CreatedAtUtc
);
