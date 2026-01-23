namespace Araponga.Api.Contracts.Governance;

public sealed record CreateVotingRequest(
    string Type,
    string Title,
    string Description,
    IReadOnlyList<string> Options,
    string Visibility,
    DateTime? StartsAtUtc,
    DateTime? EndsAtUtc);
