using Araponga.Modules.Marketplace.Domain;
using Xunit;

namespace Araponga.Tests.Modules.Marketplace.Domain;

public sealed class TerritoryPayoutConfigTests
{
    [Fact]
    public void Constructor_ShouldSetIsActiveToTrue()
    {
        // Arrange & Act
        var config = new TerritoryPayoutConfig(
            Guid.NewGuid(),
            Guid.NewGuid(),
            7, // retentionPeriodDays
            10000, // minimumPayoutAmountInCents
            null, // maximumPayoutAmountInCents
            PayoutFrequency.Weekly,
            true, // autoPayoutEnabled
            false, // requiresApproval
            "BRL");

        // Assert
        Assert.True(config.IsActive);
        Assert.Equal(7, config.RetentionPeriodDays);
        Assert.Equal(10000, config.MinimumPayoutAmountInCents);
        Assert.Null(config.MaximumPayoutAmountInCents);
        Assert.Equal(PayoutFrequency.Weekly, config.Frequency);
        Assert.True(config.AutoPayoutEnabled);
        Assert.False(config.RequiresApproval);
        Assert.Equal("BRL", config.Currency);
    }

    [Fact]
    public void Update_ShouldUpdateProperties()
    {
        // Arrange
        var config = new TerritoryPayoutConfig(
            Guid.NewGuid(),
            Guid.NewGuid(),
            7,
            10000,
            null,
            PayoutFrequency.Weekly,
            true,
            false,
            "BRL");

        // Act
        config.Update(
            14, // new retentionPeriodDays
            20000, // new minimumPayoutAmountInCents
            5000000, // new maximumPayoutAmountInCents
            PayoutFrequency.Monthly,
            false, // new autoPayoutEnabled
            true); // new requiresApproval

        // Assert
        Assert.Equal(14, config.RetentionPeriodDays);
        Assert.Equal(20000, config.MinimumPayoutAmountInCents);
        Assert.Equal(5000000, config.MaximumPayoutAmountInCents);
        Assert.Equal(PayoutFrequency.Monthly, config.Frequency);
        Assert.False(config.AutoPayoutEnabled);
        Assert.True(config.RequiresApproval);
    }

    [Fact]
    public void Deactivate_ShouldSetIsActiveToFalse()
    {
        // Arrange
        var config = new TerritoryPayoutConfig(
            Guid.NewGuid(),
            Guid.NewGuid(),
            7,
            10000,
            null,
            PayoutFrequency.Weekly,
            true,
            false,
            "BRL");

        // Act
        config.Deactivate();

        // Assert
        Assert.False(config.IsActive);
    }

    [Fact]
    public void Activate_ShouldSetIsActiveToTrue()
    {
        // Arrange
        var config = new TerritoryPayoutConfig(
            Guid.NewGuid(),
            Guid.NewGuid(),
            7,
            10000,
            null,
            PayoutFrequency.Weekly,
            true,
            false,
            "BRL");
        config.Deactivate();

        // Act
        config.Activate();

        // Assert
        Assert.True(config.IsActive);
    }
}
