namespace Araponga.Api.Contracts.Admin;

public sealed record CompleteWorkItemRequest(
    string Outcome,
    string? Notes);

