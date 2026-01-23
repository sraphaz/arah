using Araponga.Domain.Governance;
using System.Text.Json;

namespace Araponga.Infrastructure.Postgres.Entities;

public sealed class VotingRecord
{
    public Guid Id { get; set; }
    public Guid TerritoryId { get; set; }
    public Guid CreatedByUserId { get; set; }
    public VotingType Type { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string OptionsJson { get; set; } = "[]";
    public VotingVisibility Visibility { get; set; }
    public VotingStatus Status { get; set; }
    public DateTime? StartsAtUtc { get; set; }
    public DateTime? EndsAtUtc { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}
