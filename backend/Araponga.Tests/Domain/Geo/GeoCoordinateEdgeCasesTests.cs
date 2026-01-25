using Araponga.Domain.Geo;
using Xunit;

namespace Araponga.Tests.Domain.Geo;

public sealed class GeoCoordinateEdgeCasesTests
{
    [Fact]
    public void IsValid_WithValidCoordinates_ReturnsTrue()
    {
        Assert.True(GeoCoordinate.IsValid(0, 0));
        Assert.True(GeoCoordinate.IsValid(90, 180));
        Assert.True(GeoCoordinate.IsValid(-90, -180));
        Assert.True(GeoCoordinate.IsValid(45.5, -120.3));
    }

    [Fact]
    public void IsValid_WithLatitudeGreaterThan90_ReturnsFalse()
    {
        Assert.False(GeoCoordinate.IsValid(90.1, 0));
        Assert.False(GeoCoordinate.IsValid(91, 0));
        Assert.False(GeoCoordinate.IsValid(180, 0));
    }

    [Fact]
    public void IsValid_WithLatitudeLessThanNegative90_ReturnsFalse()
    {
        Assert.False(GeoCoordinate.IsValid(-90.1, 0));
        Assert.False(GeoCoordinate.IsValid(-91, 0));
        Assert.False(GeoCoordinate.IsValid(-180, 0));
    }

    [Fact]
    public void IsValid_WithLongitudeGreaterThan180_ReturnsFalse()
    {
        Assert.False(GeoCoordinate.IsValid(0, 180.1));
        Assert.False(GeoCoordinate.IsValid(0, 181));
        Assert.False(GeoCoordinate.IsValid(0, 360));
    }

    [Fact]
    public void IsValid_WithLongitudeLessThanNegative180_ReturnsFalse()
    {
        Assert.False(GeoCoordinate.IsValid(0, -180.1));
        Assert.False(GeoCoordinate.IsValid(0, -181));
        Assert.False(GeoCoordinate.IsValid(0, -360));
    }

    [Fact]
    public void IsValid_WithBoundaryValues_ReturnsTrue()
    {
        Assert.True(GeoCoordinate.IsValid(90, 180));
        Assert.True(GeoCoordinate.IsValid(-90, -180));
        Assert.True(GeoCoordinate.IsValid(90, -180));
        Assert.True(GeoCoordinate.IsValid(-90, 180));
    }

    [Fact]
    public void IsValid_WithInvalidLatitudeAndLongitude_ReturnsFalse()
    {
        Assert.False(GeoCoordinate.IsValid(91, 181));
        Assert.False(GeoCoordinate.IsValid(-91, -181));
    }

    [Fact]
    public void IsValid_WithValidLatitudeButInvalidLongitude_ReturnsFalse()
    {
        Assert.False(GeoCoordinate.IsValid(0, 181));
        Assert.False(GeoCoordinate.IsValid(45, -181));
    }

    [Fact]
    public void IsValid_WithInvalidLatitudeButValidLongitude_ReturnsFalse()
    {
        Assert.False(GeoCoordinate.IsValid(91, 0));
        Assert.False(GeoCoordinate.IsValid(-91, 0));
    }
}
