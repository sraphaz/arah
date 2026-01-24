using Araponga.Domain.Territories;
using Xunit;

namespace Araponga.Tests.Domain;

/// <summary>
/// Comprehensive edge case tests for Territory entity.
/// Tests boundary conditions, special characters, Unicode, and coordinate validation.
/// </summary>
public sealed class TerritoryEdgeCasesTests
{
    [Fact]
    public void Constructor_WithSpecialCharactersInName_TrimsAndAccepts()
    {
        // Arrange
        var name = "  Territ@ry!!! São Paulo  ";
        
        // Act
        var territory = new Territory(
            Guid.NewGuid(),
            null,
            name,
            null,
            TerritoryStatus.Active,
            "São Paulo",
            "SP",
            -23.5505,
            -46.6333,
            DateTime.UtcNow);
        
        // Assert
        Assert.Equal("Territ@ry!!! São Paulo", territory.Name);
    }

    [Fact]
    public void Constructor_WithUnicodeCharacters_Succeeds()
    {
        // Arrange
        var unicodeName = "Terrítório Çentral 🏘️";
        
        // Act & Assert
        var territory = new Territory(
            Guid.NewGuid(),
            null,
            unicodeName,
            null,
            TerritoryStatus.Active,
            "Rio",
            "RJ",
            -22.9068,
            -43.1729,
            DateTime.UtcNow);
        
        Assert.Equal(unicodeName, territory.Name);
    }

    [Fact]
    public void Constructor_WithVeryLongName_Succeeds()
    {
        // Arrange - 500 character name
        var longName = new string('A', 500);
        
        // Act & Assert
        var territory = new Territory(
            Guid.NewGuid(),
            null,
            longName,
            null,
            TerritoryStatus.Active,
            "City",
            "ST",
            0,
            0,
            DateTime.UtcNow);
        
        Assert.Equal(longName, territory.Name);
    }

    [Fact]
    public void Constructor_WithBoundaryLatitude_MaxSucceeds()
    {
        // Arrange
        var latitude = 90.0;
        
        // Act & Assert
        var territory = new Territory(
            Guid.NewGuid(),
            null,
            "North Pole",
            null,
            TerritoryStatus.Active,
            "Arctic",
            "AP",
            latitude,
            0,
            DateTime.UtcNow);
        
        Assert.Equal(latitude, territory.Latitude);
    }

    [Fact]
    public void Constructor_WithBoundaryLatitude_MinSucceeds()
    {
        // Arrange
        var latitude = -90.0;
        
        // Act & Assert
        var territory = new Territory(
            Guid.NewGuid(),
            null,
            "South Pole",
            null,
            TerritoryStatus.Active,
            "Antarctic",
            "AA",
            latitude,
            0,
            DateTime.UtcNow);
        
        Assert.Equal(latitude, territory.Latitude);
    }

    [Fact]
    public void Constructor_WithBoundaryLongitude_MaxSucceeds()
    {
        // Arrange
        var longitude = 180.0;
        
        // Act & Assert
        var territory = new Territory(
            Guid.NewGuid(),
            null,
            "Date Line East",
            null,
            TerritoryStatus.Active,
            "Pacific",
            "PC",
            0,
            longitude,
            DateTime.UtcNow);
        
        Assert.Equal(longitude, territory.Longitude);
    }

    [Fact]
    public void Constructor_WithBoundaryLongitude_MinSucceeds()
    {
        // Arrange
        var longitude = -180.0;
        
        // Act & Assert
        var territory = new Territory(
            Guid.NewGuid(),
            null,
            "Date Line West",
            null,
            TerritoryStatus.Active,
            "Pacific",
            "PC",
            0,
            longitude,
            DateTime.UtcNow);
        
        Assert.Equal(longitude, territory.Longitude);
    }

    [Fact]
    public void Constructor_WithHighPrecisionCoordinates_Succeeds()
    {
        // Arrange - Very precise coordinates (8 decimal places)
        var latitude = 40.7127753;
        var longitude = -74.0059728;
        
        // Act & Assert
        var territory = new Territory(
            Guid.NewGuid(),
            null,
            "New York City",
            null,
            TerritoryStatus.Active,
            "New York",
            "NY",
            latitude,
            longitude,
            DateTime.UtcNow);
        
        Assert.Equal(latitude, territory.Latitude);
        Assert.Equal(longitude, territory.Longitude);
    }

    [Fact]
    public void Constructor_WithNegativeCoordinates_Succeeds()
    {
        // Arrange - Sydney, Australia
        var latitude = -33.8688;
        var longitude = 151.2093;
        
        // Act & Assert
        var territory = new Territory(
            Guid.NewGuid(),
            null,
            "Sydney",
            null,
            TerritoryStatus.Active,
            "Sydney",
            "NSW",
            latitude,
            longitude,
            DateTime.UtcNow);
        
        Assert.Equal(latitude, territory.Latitude);
        Assert.Equal(longitude, territory.Longitude);
    }

    [Fact]
    public void Constructor_WithZeroCoordinates_Succeeds()
    {
        // Arrange - Null Island (0, 0)
        
        // Act & Assert
        var territory = new Territory(
            Guid.NewGuid(),
            null,
            "Null Island",
            null,
            TerritoryStatus.Active,
            "Null",
            "NL",
            0.0,
            0.0,
            DateTime.UtcNow);
        
        Assert.Equal(0.0, territory.Latitude);
        Assert.Equal(0.0, territory.Longitude);
    }

    [Fact]
    public void Constructor_WithDescription_TrimsWhitespace()
    {
        // Arrange
        var description = "  This is a territory description with lots of details  \n  and newlines  ";
        
        // Act
        var territory = new Territory(
            Guid.NewGuid(),
            null,
            "Test Territory",
            description,
            TerritoryStatus.Active,
            "City",
            "ST",
            0,
            0,
            DateTime.UtcNow);
        
        // Assert
        Assert.NotNull(territory.Description);
        Assert.Equal("This is a territory description with lots of details  \n  and newlines", territory.Description);
    }

    [Fact]
    public void Constructor_WithEmptyDescription_BecomesNull()
    {
        // Arrange
        var emptyDescription = "   \n\t  ";
        
        // Act
        var territory = new Territory(
            Guid.NewGuid(),
            null,
            "Test Territory",
            emptyDescription,
            TerritoryStatus.Active,
            "City",
            "ST",
            0,
            0,
            DateTime.UtcNow);
        
        // Assert
        Assert.Null(territory.Description);
    }

    [Fact]
    public void Constructor_WithParentTerritoryId_StoresHierarchy()
    {
        // Arrange
        var parentId = Guid.NewGuid();
        var childId = Guid.NewGuid();
        
        // Act
        var territory = new Territory(
            childId,
            parentId,
            "Child Territory",
            null,
            TerritoryStatus.Active,
            "City",
            "ST",
            0,
            0,
            DateTime.UtcNow);
        
        // Assert
        Assert.Equal(parentId, territory.ParentTerritoryId);
        Assert.Equal(childId, territory.Id);
    }

    [Fact]
    public void Constructor_WithNullParentTerritoryId_IsRootTerritory()
    {
        // Arrange & Act
        var territory = new Territory(
            Guid.NewGuid(),
            null,
            "Root Territory",
            null,
            TerritoryStatus.Active,
            "City",
            "ST",
            0,
            0,
            DateTime.UtcNow);
        
        // Assert
        Assert.Null(territory.ParentTerritoryId);
    }

    [Fact]
    public void Constructor_WithDifferentStatuses_AllSucceed()
    {
        // Test all status values
        var statuses = new[] { TerritoryStatus.Active, TerritoryStatus.Inactive, TerritoryStatus.Pending };
        
        foreach (var status in statuses)
        {
            // Act
            var territory = new Territory(
                Guid.NewGuid(),
                null,
                "Test Territory",
                null,
                status,
                "City",
                "ST",
                0,
                0,
                DateTime.UtcNow);
            
            // Assert
            Assert.Equal(status, territory.Status);
        }
    }

    [Fact]
    public void Constructor_WithCityAndStateVariations_AllSucceed()
    {
        // Arrange & Act - Various city/state combinations
        var testCases = new[]
        {
            ("São Paulo", "SP", -23.5505, -46.6333),
            ("Rio de Janeiro", "RJ", -22.9068, -43.1729),
            ("Porto Alegre", "RS", -30.0330, -51.2304),
            ("Brasília", "DF", -15.7975, -47.8919),
        };

        foreach (var (city, state, lat, lon) in testCases)
        {
            // Act
            var territory = new Territory(
                Guid.NewGuid(),
                null,
                city,
                null,
                TerritoryStatus.Active,
                city,
                state,
                lat,
                lon,
                DateTime.UtcNow);
            
            // Assert
            Assert.Equal(city, territory.City);
            Assert.Equal(state, territory.State);
            Assert.Equal(lat, territory.Latitude);
            Assert.Equal(lon, territory.Longitude);
        }
    }

    [Fact]
    public void Constructor_WithMultipleUnicodeLanguages_Succeeds()
    {
        // Arrange - Mix of languages
        var name = "Teritorium 北京 مصر Москва";
        
        // Act & Assert
        var territory = new Territory(
            Guid.NewGuid(),
            null,
            name,
            null,
            TerritoryStatus.Active,
            "City",
            "ST",
            0,
            0,
            DateTime.UtcNow);
        
        Assert.Equal(name, territory.Name);
    }

    [Fact]
    public void Constructor_CreatedAtUtc_PreservesTimeZoneInfo()
    {
        // Arrange
        var createdAt = new DateTime(2024, 1, 15, 14, 30, 45, DateTimeKind.Utc);
        
        // Act
        var territory = new Territory(
            Guid.NewGuid(),
            null,
            "Test Territory",
            null,
            TerritoryStatus.Active,
            "City",
            "ST",
            0,
            0,
            createdAt);
        
        // Assert
        Assert.Equal(createdAt, territory.CreatedAtUtc);
        Assert.Equal(DateTimeKind.Utc, territory.CreatedAtUtc.Kind);
    }

    [Fact]
    public void Constructor_WithAllPropertiesPopulated_ReturnsCorrectValues()
    {
        // Arrange
        var id = Guid.NewGuid();
        var parentId = Guid.NewGuid();
        var name = "Complete Territory";
        var description = "Full description";
        var city = "São Paulo";
        var state = "SP";
        var latitude = -23.5505;
        var longitude = -46.6333;
        var createdAt = DateTime.UtcNow;
        
        // Act
        var territory = new Territory(
            id,
            parentId,
            name,
            description,
            TerritoryStatus.Active,
            city,
            state,
            latitude,
            longitude,
            createdAt);
        
        // Assert
        Assert.Equal(id, territory.Id);
        Assert.Equal(parentId, territory.ParentTerritoryId);
        Assert.Equal(name, territory.Name);
        Assert.Equal(description, territory.Description);
        Assert.Equal(city, territory.City);
        Assert.Equal(state, territory.State);
        Assert.Equal(latitude, territory.Latitude);
        Assert.Equal(longitude, territory.Longitude);
        Assert.Equal(createdAt, territory.CreatedAtUtc);
        Assert.Equal(TerritoryStatus.Active, territory.Status);
    }
}
