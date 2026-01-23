namespace Araponga.Api.Contracts.Governance;

public sealed record VotingResultsResponse(
    Guid VotingId,
    Dictionary<string, int> Results);
