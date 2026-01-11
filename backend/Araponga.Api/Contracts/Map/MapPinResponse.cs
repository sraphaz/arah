namespace Araponga.Api.Contracts.Map;

public sealed record MapPinResponse(
    Guid Id,
    string Type,
    double Latitude,
    double Longitude,
    string Title,
    string Status);
