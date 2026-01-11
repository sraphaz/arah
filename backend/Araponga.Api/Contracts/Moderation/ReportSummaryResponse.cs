namespace Araponga.Api.Contracts.Moderation;

public sealed record ReportSummaryResponse(
    Guid Id,
    Guid TerritoryId,
    string TargetType,
    Guid TargetId,
    string Reason,
    string Status,
    DateTime CreatedAtUtc);
