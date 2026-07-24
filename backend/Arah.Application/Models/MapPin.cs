namespace Arah.Application.Models;

/// <summary>
/// Ponto (pin) do mapa de um território, agnóstico de transporte/API.
/// </summary>
public sealed record MapPin(
    string PinType,
    double Latitude,
    double Longitude,
    string Title,
    Guid? AssetId,
    Guid? PostId,
    Guid? MediaId,
    Guid? EventId,
    Guid? EntityId,
    string? Status);
