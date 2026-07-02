using System;
using Arah.Domain.Financial;
using Xunit;

namespace Arah.Tests.Domain;

/// <summary>
/// FASE55 — AC-55-4: FeeSplitRule é versionada e imutável.
/// </summary>
public sealed class FeeSplitRuleTests
{
    private static FeeSplitRule NewRule(DateTimeOffset? effectiveFrom = null) =>
        new(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "marketplace",
            implementerSharePercent: 40m,
            territoryFundSharePercent: 30m,
            platformSharePercent: 30m,
            effectiveFromUtc: effectiveFrom ?? DateTimeOffset.UtcNow.AddDays(-1));

    [Fact] // AC-55-4
    public void Constructor_WhenSharesSum100_CreatesActiveRule()
    {
        var now = DateTimeOffset.UtcNow;
        var rule = NewRule(now.AddDays(-1));

        Assert.True(rule.IsActiveAt(now));
        Assert.Null(rule.SupersededAtUtc);
    }

    [Theory] // AC-55-4
    [InlineData(40, 30, 40)]  // soma 110
    [InlineData(10, 10, 10)]  // soma 30
    [InlineData(50, 50, 10)]  // soma 110
    public void Constructor_WhenSharesDoNotSum100_Throws(decimal impl, decimal fund, decimal platform)
    {
        Assert.Throws<ArgumentException>(() => new FeeSplitRule(
            Guid.NewGuid(), Guid.NewGuid(), "marketplace", impl, fund, platform, DateTimeOffset.UtcNow));
    }

    [Fact] // AC-55-4
    public void Constructor_WhenShareNegative_Throws()
    {
        Assert.Throws<ArgumentException>(() => new FeeSplitRule(
            Guid.NewGuid(), Guid.NewGuid(), "marketplace", -10m, 60m, 50m, DateTimeOffset.UtcNow));
    }

    [Fact] // AC-55-4 — imutabilidade: nunca editar, apenas superssedê-la
    public void Supersede_MakesRuleInactive()
    {
        var rule = NewRule();
        var supersededAt = DateTimeOffset.UtcNow;

        rule.Supersede(supersededAt);

        Assert.Equal(supersededAt, rule.SupersededAtUtc);
        Assert.False(rule.IsActiveAt(DateTimeOffset.UtcNow));
    }

    [Fact] // AC-55-4 — supersessão é uma via só
    public void Supersede_WhenAlreadySuperseded_Throws()
    {
        var rule = NewRule();
        rule.Supersede(DateTimeOffset.UtcNow);

        Assert.Throws<InvalidOperationException>(() => rule.Supersede(DateTimeOffset.UtcNow));
    }

    [Fact] // AC-55-4 — não ativa antes da vigência
    public void IsActiveAt_BeforeEffectiveFrom_IsFalse()
    {
        var effectiveFrom = DateTimeOffset.UtcNow.AddDays(1);
        var rule = NewRule(effectiveFrom);

        Assert.False(rule.IsActiveAt(DateTimeOffset.UtcNow));
    }
}
