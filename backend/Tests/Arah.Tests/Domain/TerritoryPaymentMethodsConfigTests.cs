using Arah.Domain.Fiscal;
using Xunit;

namespace Arah.Tests.Domain;

public sealed class TerritoryPaymentMethodsConfigTests
{
    [Fact]
    public void ReplaceMethods_DeduplicatesAndOrders()
    {
        var config = new TerritoryPaymentMethodsConfig(
            Guid.NewGuid(),
            Guid.Parse("11111111-1111-1111-1111-111111111111"),
            new[] { TerritoryPaymentMethodKind.Card },
            null,
            DateTimeOffset.UtcNow);

        config.ReplaceMethods(
            new[] { TerritoryPaymentMethodKind.Boleto, TerritoryPaymentMethodKind.Pix, TerritoryPaymentMethodKind.Pix },
            "mock-psp",
            DateTimeOffset.UtcNow);

        Assert.Equal(2, config.Methods.Count);
        Assert.Equal(TerritoryPaymentMethodKind.Pix, config.Methods[0]);
        Assert.Equal(TerritoryPaymentMethodKind.Boleto, config.Methods[1]);
        Assert.True(config.HasMethod(TerritoryPaymentMethodKind.Pix));
        Assert.False(config.HasMethod(TerritoryPaymentMethodKind.Card));
        Assert.Equal("mock-psp", config.PspProvider);
    }
}
