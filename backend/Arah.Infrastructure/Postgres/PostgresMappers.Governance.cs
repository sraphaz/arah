using Arah.Infrastructure.Postgres.Entities;
using System.Text.Json;

namespace Arah.Infrastructure.Postgres;

public static partial class PostgresMappers
{
    public static VotingRecord ToRecord(this Domain.Governance.Voting voting)
    {
        return new VotingRecord
        {
            Id = voting.Id,
            TerritoryId = voting.TerritoryId,
            CreatedByUserId = voting.CreatedByUserId,
            Type = voting.Type,
            Title = voting.Title,
            Description = voting.Description,
            OptionsJson = JsonSerializer.Serialize(voting.Options),
            Visibility = voting.Visibility,
            Status = voting.Status,
            StartsAtUtc = voting.StartsAtUtc,
            EndsAtUtc = voting.EndsAtUtc,
            CreatedAtUtc = voting.CreatedAtUtc,
            UpdatedAtUtc = voting.UpdatedAtUtc
        };
    }

    public static Domain.Governance.Voting ToDomain(this VotingRecord record)
    {
        var options = JsonSerializer.Deserialize<List<string>>(record.OptionsJson) ?? new List<string>();
        return new Domain.Governance.Voting(
            record.Id,
            record.TerritoryId,
            record.CreatedByUserId,
            record.Type,
            record.Title,
            record.Description,
            options,
            record.Visibility,
            record.Status,
            record.StartsAtUtc,
            record.EndsAtUtc,
            record.CreatedAtUtc,
            record.UpdatedAtUtc);
    }

    public static VoteRecord ToRecord(this Domain.Governance.Vote vote)
    {
        return new VoteRecord
        {
            Id = vote.Id,
            VotingId = vote.VotingId,
            UserId = vote.UserId,
            SelectedOption = vote.SelectedOption,
            CreatedAtUtc = vote.CreatedAtUtc
        };
    }

    public static Domain.Governance.Vote ToDomain(this VoteRecord record)
    {
        return new Domain.Governance.Vote(
            record.Id,
            record.VotingId,
            record.UserId,
            record.SelectedOption,
            record.CreatedAtUtc);
    }

    public static TerritoryModerationRuleRecord ToRecord(this Domain.Governance.TerritoryModerationRule rule)
    {
        return new TerritoryModerationRuleRecord
        {
            Id = rule.Id,
            TerritoryId = rule.TerritoryId,
            CreatedByVotingId = rule.CreatedByVotingId,
            RuleType = rule.RuleType,
            RuleJson = rule.RuleJson,
            IsActive = rule.IsActive,
            CreatedAtUtc = rule.CreatedAtUtc,
            UpdatedAtUtc = rule.UpdatedAtUtc
        };
    }

    public static Domain.Governance.TerritoryModerationRule ToDomain(this TerritoryModerationRuleRecord record)
    {
        return new Domain.Governance.TerritoryModerationRule(
            record.Id,
            record.TerritoryId,
            record.CreatedByVotingId,
            record.RuleType,
            record.RuleJson,
            record.IsActive,
            record.CreatedAtUtc,
            record.UpdatedAtUtc);
    }
}
