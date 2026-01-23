namespace Araponga.Api.Contracts.Users;

public sealed record UserProfileGovernanceResponse(
    Guid UserId,
    IReadOnlyList<VotingHistoryItem> VotingHistory,
    int ModerationContributions);

public sealed record VotingHistoryItem(
    Guid VotingId,
    string VotingTitle,
    string VotingType,
    string SelectedOption,
    DateTime VotedAtUtc);
