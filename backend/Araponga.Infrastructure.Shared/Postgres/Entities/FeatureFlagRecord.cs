using Araponga.Application.Models;

namespace Araponga.Infrastructure.Shared.Postgres.Entities;

public sealed class FeatureFlagRecord
{
    public Guid TerritoryId { get; set; }
    public FeatureFlag Flag { get; set; }
}
