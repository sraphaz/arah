using Arah.Domain.Fiscal;
using Xunit;

namespace Arah.Tests.Domain;

public sealed class TerritoryFiscalPackBindingTests
{
    private static readonly Guid TerritoryId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private static readonly Guid UserId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

    [Fact]
    public void Activate_RequiresActorAndSetsActive()
    {
        var binding = new TerritoryFiscalPackBinding(
            Guid.NewGuid(),
            TerritoryId,
            "brazil.v1",
            FiscalPackBindingStatus.Off,
            null,
            null,
            null,
            DateTimeOffset.UtcNow);

        var at = DateTimeOffset.UtcNow;
        binding.Activate(UserId, at, "3550704");

        Assert.True(binding.IsActive);
        Assert.Equal(UserId, binding.ActivatedByUserId);
        Assert.Equal("3550704", binding.MunicipalityIbge);
        Assert.Equal(at, binding.ActivatedAtUtc);
    }

    [Fact]
    public void Constructor_ActiveWithoutActor_Throws()
    {
        Assert.Throws<ArgumentException>(() =>
            new TerritoryFiscalPackBinding(
                Guid.NewGuid(),
                TerritoryId,
                "brazil.v1",
                FiscalPackBindingStatus.Active,
                null,
                DateTimeOffset.UtcNow,
                null,
                DateTimeOffset.UtcNow));
    }

    [Fact]
    public void Constructor_InvalidMunicipality_Throws()
    {
        Assert.Throws<ArgumentException>(() =>
            new TerritoryFiscalPackBinding(
                Guid.NewGuid(),
                TerritoryId,
                "brazil.v1",
                FiscalPackBindingStatus.Off,
                null,
                null,
                "123",
                DateTimeOffset.UtcNow));
    }

    [Fact]
    public void BrazilV1_CatalogId_IsNormalized()
    {
        Assert.Equal("brazil.v1", FiscalPackDefinition.BrazilV1.Id);
        Assert.Equal("BR", FiscalPackDefinition.BrazilV1.CountryCode);
        Assert.Contains("kyc", FiscalPackDefinition.BrazilV1.Capabilities);
    }
}
