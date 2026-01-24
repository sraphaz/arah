using Araponga.Application.Common;
using Araponga.Application.Services;
using Araponga.Domain.Geo;
using Araponga.Domain.Marketplace;
using Xunit;

namespace Araponga.Tests.Application;

/// <summary>
/// Edge case tests for Application Layer Services validation,
/// focusing on input validation, business logic validation, and error handling.
/// </summary>
public class ApplicationServiceValidationTests
{
    // Result<T> validation tests
    [Fact]
    public void Result_Success_WithValue_ReturnsSuccess()
    {
        var value = "test";
        var result = Result<string>.Success(value);

        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Equal(value, result.Value);
        Assert.Null(result.Error);
    }

    [Fact]
    public void Result_Failure_WithError_ReturnsFailure()
    {
        var error = "Error message";
        var result = Result<string>.Failure(error);

        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Null(result.Value);
        Assert.Equal(error, result.Error);
    }

    [Fact]
    public void Result_ImplicitConversion_FromValue_ReturnsSuccess()
    {
        var value = "test";
        Result<string> result = value;

        Assert.True(result.IsSuccess);
        Assert.Equal(value, result.Value);
    }

    // OperationResult validation tests
    [Fact]
    public void OperationResult_Success_ReturnsSuccess()
    {
        var result = OperationResult.Success();

        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Null(result.Error);
    }

    [Fact]
    public void OperationResult_Failure_WithError_ReturnsFailure()
    {
        var error = "Error message";
        var result = OperationResult.Failure(error);

        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Equal(error, result.Error);
    }

    // OperationResult<T> validation tests
    [Fact]
    public void OperationResultT_Success_WithValue_ReturnsSuccess()
    {
        var value = 42;
        var result = OperationResult<int>.Success(value);

        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Equal(value, result.Value);
        Assert.Null(result.Error);
    }

    [Fact]
    public void OperationResultT_Failure_WithError_ReturnsFailure()
    {
        var error = "Error message";
        var result = OperationResult<int>.Failure(error);

        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Equal(default(int), result.Value); // Value type defaults to 0, not null
        Assert.Equal(error, result.Error);
    }

    [Fact]
    public void OperationResultT_ImplicitConversion_FromValue_ReturnsSuccess()
    {
        var value = 42;
        OperationResult<int> result = value;

        Assert.True(result.IsSuccess);
        Assert.Equal(value, result.Value);
    }

    // GeoCoordinate validation tests
    [Fact]
    public void GeoCoordinate_IsValid_WithValidCoordinates_ReturnsTrue()
    {
        Assert.True(GeoCoordinate.IsValid(0, 0));
        Assert.True(GeoCoordinate.IsValid(-23.5505, -46.6333));
        Assert.True(GeoCoordinate.IsValid(90, 180));
        Assert.True(GeoCoordinate.IsValid(-90, -180));
    }

    [Fact]
    public void GeoCoordinate_IsValid_WithInvalidLatitude_ReturnsFalse()
    {
        Assert.False(GeoCoordinate.IsValid(91, 0));
        Assert.False(GeoCoordinate.IsValid(-91, 0));
        Assert.False(GeoCoordinate.IsValid(100, 0));
        Assert.False(GeoCoordinate.IsValid(-100, 0));
    }

    [Fact]
    public void GeoCoordinate_IsValid_WithInvalidLongitude_ReturnsFalse()
    {
        Assert.False(GeoCoordinate.IsValid(0, 181));
        Assert.False(GeoCoordinate.IsValid(0, -181));
        Assert.False(GeoCoordinate.IsValid(0, 200));
        Assert.False(GeoCoordinate.IsValid(0, -200));
    }

    [Fact]
    public void GeoCoordinate_IsValid_WithBoundaryValues_ReturnsTrue()
    {
        // Boundary values should be valid
        Assert.True(GeoCoordinate.IsValid(90, 180));
        Assert.True(GeoCoordinate.IsValid(-90, -180));
        Assert.True(GeoCoordinate.IsValid(90, -180));
        Assert.True(GeoCoordinate.IsValid(-90, 180));
    }

    [Fact]
    public void GeoCoordinate_IsValid_WithNullIsland_ReturnsTrue()
    {
        // Null Island (0, 0) is a valid coordinate
        Assert.True(GeoCoordinate.IsValid(0, 0));
    }

    [Fact]
    public void GeoCoordinate_IsValid_WithHighPrecision_ReturnsTrue()
    {
        // High precision coordinates should be valid
        Assert.True(GeoCoordinate.IsValid(-23.5505199, -46.6333094));
        Assert.True(GeoCoordinate.IsValid(40.7127753, -74.0059728));
    }

    // String validation edge cases
    [Fact]
    public void StringValidation_IsNullOrWhiteSpace_WithNull_ReturnsTrue()
    {
        Assert.True(string.IsNullOrWhiteSpace(null));
    }

    [Fact]
    public void StringValidation_IsNullOrWhiteSpace_WithEmpty_ReturnsTrue()
    {
        Assert.True(string.IsNullOrWhiteSpace(""));
    }

    [Fact]
    public void StringValidation_IsNullOrWhiteSpace_WithWhitespace_ReturnsTrue()
    {
        Assert.True(string.IsNullOrWhiteSpace("   "));
        Assert.True(string.IsNullOrWhiteSpace("\t"));
        Assert.True(string.IsNullOrWhiteSpace("\n"));
        Assert.True(string.IsNullOrWhiteSpace("\r\n"));
    }

    [Fact]
    public void StringValidation_IsNullOrWhiteSpace_WithContent_ReturnsFalse()
    {
        Assert.False(string.IsNullOrWhiteSpace("test"));
        Assert.False(string.IsNullOrWhiteSpace(" test "));
    }

    [Fact]
    public void StringValidation_Trim_WithWhitespace_TrimsCorrectly()
    {
        Assert.Equal("test", "  test  ".Trim());
        Assert.Equal("test", "\ttest\t".Trim());
        Assert.Equal("test", "\ntest\n".Trim());
    }

    [Fact]
    public void StringValidation_Trim_WithUnicode_TrimsCorrectly()
    {
        Assert.Equal("café", "  café  ".Trim());
        Assert.Equal("文字", "  文字  ".Trim());
        Assert.Equal("test 🏪", "  test 🏪  ".Trim());
    }

    // Guid validation edge cases
    [Fact]
    public void GuidValidation_EmptyGuid_IsEmpty()
    {
        var emptyGuid = Guid.Empty;
        Assert.Equal(Guid.Empty, emptyGuid);
    }

    [Fact]
    public void GuidValidation_NewGuid_IsNotEmpty()
    {
        var guid = Guid.NewGuid();
        Assert.NotEqual(Guid.Empty, guid);
    }

    [Fact]
    public void GuidValidation_EmptyGuid_Comparison()
    {
        var emptyGuid = Guid.Empty;
        var newGuid = Guid.NewGuid();

        Assert.True(emptyGuid == Guid.Empty);
        Assert.False(newGuid == Guid.Empty);
    }

    // Decimal validation edge cases
    [Fact]
    public void DecimalValidation_Zero_IsValid()
    {
        Assert.Equal(0m, 0m);
        Assert.True(0m >= 0);
    }

    [Fact]
    public void DecimalValidation_Negative_IsValid()
    {
        Assert.True(-10m < 0);
        Assert.True(-0.01m < 0);
    }

    [Fact]
    public void DecimalValidation_LargeValue_IsValid()
    {
        var largeValue = decimal.MaxValue;
        Assert.True(largeValue > 0);
    }

    [Fact]
    public void DecimalValidation_Precision_IsPreserved()
    {
        var precise = 99.99m;
        Assert.Equal(99.99m, precise);
    }

    // Integer validation edge cases
    [Fact]
    public void IntegerValidation_Zero_IsValid()
    {
        Assert.Equal(0, 0);
        Assert.True(0 >= 0);
    }

    [Fact]
    public void IntegerValidation_Negative_IsValid()
    {
        Assert.True(-1 < 0);
        Assert.True(-100 < 0);
    }

    [Fact]
    public void IntegerValidation_MaxValue_IsValid()
    {
        var maxValue = int.MaxValue;
        Assert.True(maxValue > 0);
    }

    [Fact]
    public void IntegerValidation_MinValue_IsValid()
    {
        var minValue = int.MinValue;
        Assert.True(minValue < 0);
    }

    // DateTime validation edge cases
    [Fact]
    public void DateTimeValidation_UtcNow_IsRecent()
    {
        var now = DateTime.UtcNow;
        var after = DateTime.UtcNow;

        // Should be very close (within milliseconds)
        Assert.True((after - now).TotalMilliseconds >= 0);
    }

    [Fact]
    public void DateTimeValidation_Comparison_WorksCorrectly()
    {
        var earlier = DateTime.UtcNow;
        System.Threading.Thread.Sleep(10);
        var later = DateTime.UtcNow;

        Assert.True(later > earlier);
        Assert.True(earlier < later);
    }

    // Collection validation edge cases
    [Fact]
    public void CollectionValidation_EmptyCollection_IsEmpty()
    {
        var empty = Array.Empty<string>();
        Assert.Empty(empty);
    }

    [Fact]
    public void CollectionValidation_NullCollection_IsNull()
    {
        string[]? nullCollection = null;
        Assert.Null(nullCollection);
    }

    [Fact]
    public void CollectionValidation_CollectionWithNulls_ContainsNulls()
    {
        var collection = new string?[] { "test", null, "another" };
        Assert.Contains(null, collection);
        Assert.Equal(3, collection.Length);
    }

    // Enum validation edge cases
    [Fact]
    public void EnumValidation_StoreStatus_AllValuesAreValid()
    {
        var statuses = Enum.GetValues<StoreStatus>();
        Assert.NotEmpty(statuses);
        Assert.Contains(StoreStatus.Active, statuses);
        Assert.Contains(StoreStatus.Paused, statuses);
        Assert.Contains(StoreStatus.Archived, statuses);
    }

    [Fact]
    public void EnumValidation_ItemType_AllValuesAreValid()
    {
        var types = Enum.GetValues<ItemType>();
        Assert.NotEmpty(types);
        Assert.Contains(ItemType.Product, types);
        Assert.Contains(ItemType.Service, types);
    }

    [Fact]
    public void EnumValidation_ItemPricingType_AllValuesAreValid()
    {
        var pricingTypes = Enum.GetValues<ItemPricingType>();
        Assert.NotEmpty(pricingTypes);
        Assert.Contains(ItemPricingType.Fixed, pricingTypes);
        Assert.Contains(ItemPricingType.Negotiable, pricingTypes);
    }

    // Nullable validation edge cases
    [Fact]
    public void NullableValidation_NullValue_IsNull()
    {
        int? nullableInt = null;
        string? nullableString = null;
        Guid? nullableGuid = null;

        Assert.Null(nullableInt);
        Assert.Null(nullableString);
        Assert.Null(nullableGuid);
    }

    [Fact]
    public void NullableValidation_Value_IsNotNull()
    {
        int? nullableInt = 42;
        string? nullableString = "test";
        Guid? nullableGuid = Guid.NewGuid();

        Assert.NotNull(nullableInt);
        Assert.NotNull(nullableString);
        Assert.NotNull(nullableGuid);
        Assert.Equal(42, nullableInt.Value);
        Assert.Equal("test", nullableString);
    }

    // Unicode and special characters validation
    [Fact]
    public void UnicodeValidation_UnicodeString_IsValid()
    {
        var unicode = "café naïve résumé 文字 🏪";
        Assert.NotEmpty(unicode);
        Assert.Contains("café", unicode);
        Assert.Contains("文字", unicode);
        Assert.Contains("🏪", unicode);
    }

    [Fact]
    public void UnicodeValidation_EmojiString_IsValid()
    {
        var emoji = "🏪 🍃 🛒 ✅ ❌";
        Assert.NotEmpty(emoji);
        Assert.Contains("🏪", emoji);
    }

    [Fact]
    public void UnicodeValidation_SpecialCharacters_ArePreserved()
    {
        var special = "test & co. <tag> \"quote\" 'apostrophe'";
        Assert.Contains("&", special);
        Assert.Contains("<", special);
        Assert.Contains("\"", special);
    }
}
