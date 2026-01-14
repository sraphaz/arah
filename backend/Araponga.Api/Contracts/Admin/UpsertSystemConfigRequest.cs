namespace Araponga.Api.Contracts.Admin;

public sealed record UpsertSystemConfigRequest(
    string Value,
    string Category,
    string? Description);

