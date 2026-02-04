namespace Araponga.Modules.Moderation.Domain.Moderation;

public sealed class Sanction
{
    public Sanction(
        Guid id,
        Guid? territoryId,
        SanctionScope scope,
        SanctionTargetType targetType,
        Guid targetId,
        SanctionType type,
        string reason,
        SanctionStatus status,
        DateTime startAtUtc,
        DateTime? endAtUtc,
        DateTime createdAtUtc)
    {
        if (targetId == Guid.Empty)
        {
            throw new ArgumentException("Target ID is required.", nameof(targetId));
        }

        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new ArgumentException("Reason is required.", nameof(reason));
        }

        if (scope == SanctionScope.Territory && territoryId == Guid.Empty)
        {
            throw new ArgumentException("Territory ID is required for territory scope.", nameof(territoryId));
        }

        Id = id;
        TerritoryId = territoryId == Guid.Empty ? null : territoryId;
        Scope = scope;
        TargetType = targetType;
        TargetId = targetId;
        Type = type;
        Reason = reason.Trim();
        Status = status;
        StartAtUtc = startAtUtc;
        EndAtUtc = endAtUtc;
        CreatedAtUtc = createdAtUtc;
    }

    public Guid Id { get; }
    public Guid? TerritoryId { get; }
    public SanctionScope Scope { get; }
    public SanctionTargetType TargetType { get; }
    public Guid TargetId { get; }
    public SanctionType Type { get; }
    public string Reason { get; }
    public SanctionStatus Status { get; }
    public DateTime StartAtUtc { get; }
    public DateTime? EndAtUtc { get; }
    public DateTime CreatedAtUtc { get; }
}
