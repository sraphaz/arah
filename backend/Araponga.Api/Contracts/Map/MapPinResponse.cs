using System;

namespace Araponga.Api.Contracts.Map;

public sealed record MapPinResponse(
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
