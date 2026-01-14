namespace Araponga.Api.Contracts.Admin;

public sealed record SystemConfigResponse(
    string Key,
    string Value,
    string Category,
    string? Description,
    DateTime CreatedAtUtc,
    Guid CreatedByUserId,
    DateTime? UpdatedAtUtc,
    Guid? UpdatedByUserId);

