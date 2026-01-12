namespace Araponga.Application.Models;

public sealed record OutboxMessage(
    Guid Id,
    string Type,
    string PayloadJson,
    DateTime OccurredAtUtc,
    DateTime? ProcessedAtUtc = null,
    int Attempts = 0,
    string? LastError = null,
    DateTime? ProcessAfterUtc = null
);
