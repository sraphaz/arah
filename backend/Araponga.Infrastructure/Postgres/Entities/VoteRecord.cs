namespace Araponga.Infrastructure.Postgres.Entities;

public sealed class VoteRecord
{
    public Guid Id { get; set; }
    public Guid VotingId { get; set; }
    public Guid UserId { get; set; }
    public string SelectedOption { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
}
