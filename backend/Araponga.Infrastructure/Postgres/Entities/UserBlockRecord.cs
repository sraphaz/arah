namespace Araponga.Infrastructure.Postgres.Entities;

public sealed class UserBlockRecord
{
    public Guid BlockerUserId { get; set; }
    public Guid BlockedUserId { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
