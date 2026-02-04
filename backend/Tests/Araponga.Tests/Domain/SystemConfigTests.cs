using Araponga.Domain.Configuration;
using Xunit;

namespace Araponga.Tests.Domain;

public sealed class SystemConfigTests
{
    [Fact]
    public void SystemConfig_RequiresId()
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            new SystemConfig(
                Guid.Empty,
                "moderation.reports.threshold",
                "3",
                SystemConfigCategory.Moderation,
                null,
                DateTime.UtcNow,
                Guid.NewGuid(),
                null,
                null));

        Assert.Contains("ID", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void SystemConfig_RequiresKey()
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            new SystemConfig(
                Guid.NewGuid(),
                "",
                "3",
                SystemConfigCategory.Moderation,
                null,
                DateTime.UtcNow,
                Guid.NewGuid(),
                null,
                null));

        Assert.Contains("Key", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void SystemConfig_NormalizesKey_ToLower()
    {
        var cfg = new SystemConfig(
            Guid.NewGuid(),
            "Moderation.Reports.Threshold",
            "3",
            SystemConfigCategory.Moderation,
            null,
            DateTime.UtcNow,
            Guid.NewGuid(),
            null,
            null);

        Assert.Equal("moderation.reports.threshold", cfg.Key);
    }

    [Fact]
    public void SystemConfig_Update_UpdatesValueAndAuditFields()
    {
        var cfg = new SystemConfig(
            Guid.NewGuid(),
            "moderation.reports.threshold",
            "3",
            SystemConfigCategory.Moderation,
            null,
            DateTime.UtcNow,
            Guid.NewGuid(),
            null,
            null);

        var actor = Guid.NewGuid();
        var now = DateTime.UtcNow;
        cfg.Update("4", SystemConfigCategory.Moderation, "updated", actor, now);

        Assert.Equal("4", cfg.Value);
        Assert.Equal(actor, cfg.UpdatedByUserId);
        Assert.Equal(now, cfg.UpdatedAtUtc);
        Assert.Equal("updated", cfg.Description);
    }
}

