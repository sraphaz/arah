namespace Araponga.Infrastructure.Postgres.Entities;

public sealed class UserInterestRecord
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string InterestTag { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
}
