namespace Araponga.Api.Contracts.Moderation;

public sealed record ReportResponse(
    Guid Id,
    string TargetType,
    Guid TargetId,
    string Reason,
    string? Details,
    bool IsDuplicate,
    DateTime CreatedAtUtc);
