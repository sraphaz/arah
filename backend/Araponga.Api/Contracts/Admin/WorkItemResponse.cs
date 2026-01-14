namespace Araponga.Api.Contracts.Admin;

public sealed record WorkItemResponse(
    Guid Id,
    string Type,
    string Status,
    Guid? TerritoryId,
    Guid CreatedByUserId,
    DateTime CreatedAtUtc,
    string? RequiredSystemPermission,
    string? RequiredCapability,
    string SubjectType,
    Guid SubjectId,
    string? PayloadJson,
    string Outcome,
    DateTime? CompletedAtUtc,
    Guid? CompletedByUserId,
    string? CompletionNotes);

