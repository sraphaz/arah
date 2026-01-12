namespace Araponga.Application.Models;

public sealed record NotificationDispatchPayload(
    string Kind,
    IReadOnlyCollection<Guid> Recipients,
    string Title,
    string? Body,
    IReadOnlyDictionary<string, string>? Data
);
