using Araponga.Application.Interfaces;
using Araponga.Application.Services;
using Araponga.Application.Models;
using Araponga.Modules.Marketplace.Domain;
using Araponga.Infrastructure.InMemory;
using Xunit;

namespace Araponga.Tests.Application;

public sealed class TerritoryPayoutConfigServiceTests
{
    private static readonly Guid TerritoryId = TestIds.Territory2;
    private static readonly Guid UserId = TestIds.ResidentUser;

    private static TerritoryPayoutConfigService CreateService(InMemoryDataStore dataStore)
    {
        var repository = new InMemoryTerritoryPayoutConfigRepository(dataStore);
        var auditLogger = new Araponga.Infrastructure.InMemory.InMemoryAuditLogger(dataStore);
        var unitOfWork = new InMemoryUnitOfWork();

        return new TerritoryPayoutConfigService(repository, auditLogger, unitOfWork);
    }

    [Fact]
    public async Task GetActiveConfigAsync_ShouldReturnNull_WhenNoConfigExists()
    {
        // Arrange
        var dataStore = new InMemoryDataStore();
        var service = CreateService(dataStore);

        // Act
        var result = await service.GetActiveConfigAsync(TerritoryId, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UpsertConfigAsync_ShouldCreateNewConfig_WhenNoneExists()
    {
        // Arrange
        var dataStore = new InMemoryDataStore();
        var service = CreateService(dataStore);

        // Act
        var result = await service.UpsertConfigAsync(
            TerritoryId,
            7, // retentionPeriodDays
            10000, // minimumPayoutAmountInCents
            null, // maximumPayoutAmountInCents
            PayoutFrequency.Weekly,
            true, // autoPayoutEnabled
            false, // requiresApproval
            "BRL",
            UserId,
            CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(7, result.Value.RetentionPeriodDays);
        Assert.Equal(10000, result.Value.MinimumPayoutAmountInCents);
        Assert.Null(result.Value.MaximumPayoutAmountInCents);
        Assert.Equal(PayoutFrequency.Weekly, result.Value.Frequency);
        Assert.True(result.Value.AutoPayoutEnabled);
        Assert.False(result.Value.RequiresApproval);
        Assert.Equal("BRL", result.Value.Currency);
        Assert.True(result.Value.IsActive);
    }

    [Fact]
    public async Task UpsertConfigAsync_ShouldDeactivateOldAndCreateNew_WhenConfigExists()
    {
        // Arrange
        var dataStore = new InMemoryDataStore();
        var service = CreateService(dataStore);

        // Criar primeira configuração
        var firstResult = await service.UpsertConfigAsync(
            TerritoryId,
            7,
            10000,
            null,
            PayoutFrequency.Weekly,
            true,
            false,
            "BRL",
            UserId,
            CancellationToken.None);

        Assert.True(firstResult.IsSuccess);
        var firstConfigId = firstResult.Value!.Id;

        // Act - Criar nova configuração
        var secondResult = await service.UpsertConfigAsync(
            TerritoryId,
            14, // new retentionPeriodDays
            20000, // new minimumPayoutAmountInCents
            5000000, // new maximumPayoutAmountInCents
            PayoutFrequency.Monthly,
            false, // new autoPayoutEnabled
            true, // new requiresApproval
            "BRL",
            UserId,
            CancellationToken.None);

        // Assert
        Assert.True(secondResult.IsSuccess);
        Assert.NotNull(secondResult.Value);
        Assert.NotEqual(firstConfigId, secondResult.Value.Id); // Nova configuração
        Assert.Equal(14, secondResult.Value.RetentionPeriodDays);
        Assert.Equal(20000, secondResult.Value.MinimumPayoutAmountInCents);

        // Verificar que a primeira foi desativada
        var oldConfig = await service.GetActiveConfigAsync(TerritoryId, CancellationToken.None);
        Assert.Equal(secondResult.Value.Id, oldConfig!.Id); // Nova está ativa
        Assert.True(oldConfig.IsActive);
    }

    [Fact]
    public async Task UpsertConfigAsync_ShouldReturnFailure_WhenRetentionPeriodDaysIsNegative()
    {
        // Arrange
        var dataStore = new InMemoryDataStore();
        var service = CreateService(dataStore);

        // Act
        var result = await service.UpsertConfigAsync(
            TerritoryId,
            -1, // invalid retentionPeriodDays
            10000,
            null,
            PayoutFrequency.Weekly,
            true,
            false,
            "BRL",
            UserId,
            CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("RetentionPeriodDays", result.Error ?? "");
    }

    [Fact]
    public async Task UpsertConfigAsync_ShouldReturnFailure_WhenMinimumPayoutAmountIsNegative()
    {
        // Arrange
        var dataStore = new InMemoryDataStore();
        var service = CreateService(dataStore);

        // Act
        var result = await service.UpsertConfigAsync(
            TerritoryId,
            7,
            -1, // invalid minimumPayoutAmountInCents
            null,
            PayoutFrequency.Weekly,
            true,
            false,
            "BRL",
            UserId,
            CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("MinimumPayoutAmountInCents", result.Error ?? "");
    }

    [Fact]
    public async Task UpsertConfigAsync_ShouldReturnFailure_WhenMaximumIsLessThanMinimum()
    {
        // Arrange
        var dataStore = new InMemoryDataStore();
        var service = CreateService(dataStore);

        // Act
        var result = await service.UpsertConfigAsync(
            TerritoryId,
            7,
            10000, // minimum
            5000,  // maximum < minimum (invalid)
            PayoutFrequency.Weekly,
            true,
            false,
            "BRL",
            UserId,
            CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("MaximumPayoutAmountInCents", result.Error ?? "");
    }

    [Fact]
    public async Task UpsertConfigAsync_ShouldReturnFailure_WhenCurrencyIsInvalid()
    {
        // Arrange
        var dataStore = new InMemoryDataStore();
        var service = CreateService(dataStore);

        // Act - currency vazia
        var result1 = await service.UpsertConfigAsync(
            TerritoryId,
            7,
            10000,
            null,
            PayoutFrequency.Weekly,
            true,
            false,
            "", // invalid currency
            UserId,
            CancellationToken.None);

        // Assert
        Assert.False(result1.IsSuccess);
        Assert.Contains("Currency", result1.Error ?? "");

        // Act - currency com mais de 3 caracteres
        var result2 = await service.UpsertConfigAsync(
            TerritoryId,
            7,
            10000,
            null,
            PayoutFrequency.Weekly,
            true,
            false,
            "BRLS", // invalid currency (4 chars)
            UserId,
            CancellationToken.None);

        // Assert
        Assert.False(result2.IsSuccess);
        Assert.Contains("Currency", result2.Error ?? "");
    }

    [Fact]
    public async Task UpsertConfigAsync_ShouldNormalizeCurrencyToUpperCase()
    {
        // Arrange
        var dataStore = new InMemoryDataStore();
        var service = CreateService(dataStore);

        // Act
        var result = await service.UpsertConfigAsync(
            TerritoryId,
            7,
            10000,
            null,
            PayoutFrequency.Weekly,
            true,
            false,
            "brl", // lowercase
            UserId,
            CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("BRL", result.Value!.Currency); // Upper case
    }
}
