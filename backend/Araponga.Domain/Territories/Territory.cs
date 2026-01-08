namespace Araponga.Domain.Territories;

public sealed class Territory
{
    public Territory(
        Guid id,
        string name,
        string? description,
        SensitivityLevel sensitivity,
        TerritoryStatus status,
        DateTime createdAtUtc)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Territory name is required.", nameof(name));
        }

        Id = id;
        Name = name.Trim();
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        Sensitivity = sensitivity;
        Status = status;
        CreatedAtUtc = createdAtUtc;
    }

    public Guid Id { get; }
    public string Name { get; }
    public string? Description { get; }
    public SensitivityLevel Sensitivity { get; }
    public TerritoryStatus Status { get; }
    public DateTime CreatedAtUtc { get; }
}
