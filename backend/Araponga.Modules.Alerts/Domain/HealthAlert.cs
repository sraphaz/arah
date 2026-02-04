namespace Araponga.Domain.Health;

public sealed class HealthAlert
{
    public HealthAlert(
        Guid id,
        Guid territoryId,
        Guid reporterUserId,
        string title,
        string description,
        HealthAlertStatus status,
        DateTime createdAtUtc)
    {
        if (territoryId == Guid.Empty)
        {
            throw new ArgumentException("Territory ID is required.", nameof(territoryId));
        }

        if (reporterUserId == Guid.Empty)
        {
            throw new ArgumentException("Reporter user ID is required.", nameof(reporterUserId));
        }

        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Title is required.", nameof(title));
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException("Description is required.", nameof(description));
        }

        Id = id;
        TerritoryId = territoryId;
        ReporterUserId = reporterUserId;
        Title = title.Trim();
        Description = description.Trim();
        Status = status;
        CreatedAtUtc = createdAtUtc;
    }

    public Guid Id { get; }
    public Guid TerritoryId { get; }
    public Guid ReporterUserId { get; }
    public string Title { get; }
    public string Description { get; }
    public HealthAlertStatus Status { get; private set; }
    public DateTime CreatedAtUtc { get; }

    public void UpdateStatus(HealthAlertStatus status)
    {
        Status = status;
    }
}
