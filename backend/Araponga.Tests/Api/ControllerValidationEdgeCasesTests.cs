using Araponga.Api.Contracts.Marketplace;
using Araponga.Api.Validators;
using FluentValidation;
using Xunit;

namespace Araponga.Tests.Api;

/// <summary>
/// Edge case tests for Controller Request Validators,
/// focusing on FluentValidation rules, boundary conditions, and edge cases.
/// </summary>
public class ControllerValidationEdgeCasesTests
{
    // CreateItemRequestValidator tests
    [Fact]
    public void CreateItemRequestValidator_WithValidRequest_Passes()
    {
        var validator = new CreateItemRequestValidator();
        var request = new CreateItemRequest(
            TerritoryId: Guid.NewGuid(),
            StoreId: Guid.NewGuid(),
            Type: "Product",
            Title: "Test Product",
            Description: null,
            Category: null,
            Tags: null,
            PricingType: "Fixed",
            PriceAmount: 100m,
            Currency: "BRL",
            Unit: null,
            Latitude: null,
            Longitude: null,
            Status: null,
            MediaIds: null);

        var result = validator.Validate(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void CreateItemRequestValidator_WithEmptyTerritoryId_Fails()
    {
        var validator = new CreateItemRequestValidator();
        var request = new CreateItemRequest(
            TerritoryId: Guid.Empty,
            StoreId: Guid.NewGuid(),
            Type: "Product",
            Title: "Test Product",
            Description: null,
            Category: null,
            Tags: null,
            PricingType: "Fixed",
            PriceAmount: null,
            Currency: null,
            Unit: null,
            Latitude: null,
            Longitude: null,
            Status: null,
            MediaIds: null);

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void CreateItemRequestValidator_WithEmptyStoreId_Fails()
    {
        var validator = new CreateItemRequestValidator();
        var request = new CreateItemRequest(
            TerritoryId: Guid.NewGuid(),
            StoreId: Guid.Empty,
            Type: "Product",
            Title: "Test Product",
            Description: null,
            Category: null,
            Tags: null,
            PricingType: "Fixed",
            PriceAmount: null,
            Currency: null,
            Unit: null,
            Latitude: null,
            Longitude: null,
            Status: null,
            MediaIds: null);

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void CreateItemRequestValidator_WithNullType_Fails()
    {
        var validator = new CreateItemRequestValidator();
        var request = new CreateItemRequest(
            TerritoryId: Guid.NewGuid(),
            StoreId: Guid.NewGuid(),
            Type: null!,
            Title: "Test Product",
            Description: null,
            Category: null,
            Tags: null,
            PricingType: "Fixed",
            PriceAmount: null,
            Currency: null,
            Unit: null,
            Latitude: null,
            Longitude: null,
            Status: null,
            MediaIds: null);

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void CreateItemRequestValidator_WithEmptyType_Fails()
    {
        var validator = new CreateItemRequestValidator();
        var request = new CreateItemRequest(
            TerritoryId: Guid.NewGuid(),
            StoreId: Guid.NewGuid(),
            Type: "   ",
            Title: "Test Product",
            Description: null,
            Category: null,
            Tags: null,
            PricingType: "Fixed",
            PriceAmount: null,
            Currency: null,
            Unit: null,
            Latitude: null,
            Longitude: null,
            Status: null,
            MediaIds: null);

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void CreateItemRequestValidator_WithInvalidType_Fails()
    {
        var validator = new CreateItemRequestValidator();
        var request = new CreateItemRequest(
            TerritoryId: Guid.NewGuid(),
            StoreId: Guid.NewGuid(),
            Type: "InvalidType",
            Title: "Test Product",
            Description: null,
            Category: null,
            Tags: null,
            PricingType: "Fixed",
            PriceAmount: null,
            Currency: null,
            Unit: null,
            Latitude: null,
            Longitude: null,
            Status: null,
            MediaIds: null);

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void CreateItemRequestValidator_WithCaseInsensitiveType_Passes()
    {
        var validator = new CreateItemRequestValidator();
        var types = new[] { "product", "PRODUCT", "Product", "Service", "SERVICE", "service" };

        foreach (var type in types)
        {
            var request = new CreateItemRequest(
                TerritoryId: Guid.NewGuid(),
                StoreId: Guid.NewGuid(),
                Type: type,
                Title: "Test",
                Description: null,
                Category: null,
                Tags: null,
                PricingType: "Fixed",
                PriceAmount: null,
                Currency: null,
                Unit: null,
                Latitude: null,
                Longitude: null,
                Status: null,
                MediaIds: null);

            var result = validator.Validate(request);
            Assert.True(result.IsValid, $"Type '{type}' should be valid");
        }
    }

    [Fact]
    public void CreateItemRequestValidator_WithNullTitle_Fails()
    {
        var validator = new CreateItemRequestValidator();
        var request = new CreateItemRequest(
            TerritoryId: Guid.NewGuid(),
            StoreId: Guid.NewGuid(),
            Type: "Product",
            Title: null!,
            Description: null,
            Category: null,
            Tags: null,
            PricingType: "Fixed",
            PriceAmount: null,
            Currency: null,
            Unit: null,
            Latitude: null,
            Longitude: null,
            Status: null,
            MediaIds: null);

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void CreateItemRequestValidator_WithEmptyTitle_Fails()
    {
        var validator = new CreateItemRequestValidator();
        var request = new CreateItemRequest(
            TerritoryId: Guid.NewGuid(),
            StoreId: Guid.NewGuid(),
            Type: "Product",
            Title: "   ",
            Description: null,
            Category: null,
            Tags: null,
            PricingType: "Fixed",
            PriceAmount: null,
            Currency: null,
            Unit: null,
            Latitude: null,
            Longitude: null,
            Status: null,
            MediaIds: null);

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void CreateItemRequestValidator_WithTitleExceeding200Chars_Fails()
    {
        var validator = new CreateItemRequestValidator();
        var request = new CreateItemRequest(
            TerritoryId: Guid.NewGuid(),
            StoreId: Guid.NewGuid(),
            Type: "Product",
            Title: new string('A', 201),
            Description: null,
            Category: null,
            Tags: null,
            PricingType: "Fixed",
            PriceAmount: null,
            Currency: null,
            Unit: null,
            Latitude: null,
            Longitude: null,
            Status: null,
            MediaIds: null);

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void CreateItemRequestValidator_WithTitleExactly200Chars_Passes()
    {
        var validator = new CreateItemRequestValidator();
        var request = new CreateItemRequest(
            TerritoryId: Guid.NewGuid(),
            StoreId: Guid.NewGuid(),
            Type: "Product",
            Title: new string('A', 200),
            Description: null,
            Category: null,
            Tags: null,
            PricingType: "Fixed",
            PriceAmount: null,
            Currency: null,
            Unit: null,
            Latitude: null,
            Longitude: null,
            Status: null,
            MediaIds: null);

        var result = validator.Validate(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void CreateItemRequestValidator_WithDescriptionExceeding2000Chars_Fails()
    {
        var validator = new CreateItemRequestValidator();
        var request = new CreateItemRequest(
            TerritoryId: Guid.NewGuid(),
            StoreId: Guid.NewGuid(),
            Type: "Product",
            Title: "Test",
            Description: new string('A', 2001),
            Category: null,
            Tags: null,
            PricingType: "Fixed",
            PriceAmount: null,
            Currency: null,
            Unit: null,
            Latitude: null,
            Longitude: null,
            Status: null,
            MediaIds: null);

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void CreateItemRequestValidator_WithDescriptionExactly2000Chars_Passes()
    {
        var validator = new CreateItemRequestValidator();
        var request = new CreateItemRequest(
            TerritoryId: Guid.NewGuid(),
            StoreId: Guid.NewGuid(),
            Type: "Product",
            Title: "Test",
            Description: new string('A', 2000),
            Category: null,
            Tags: null,
            PricingType: "Fixed",
            PriceAmount: null,
            Currency: null,
            Unit: null,
            Latitude: null,
            Longitude: null,
            Status: null,
            MediaIds: null);

        var result = validator.Validate(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void CreateItemRequestValidator_WithNullPricingType_Fails()
    {
        var validator = new CreateItemRequestValidator();
        var request = new CreateItemRequest(
            TerritoryId: Guid.NewGuid(),
            StoreId: Guid.NewGuid(),
            Type: "Product",
            Title: "Test",
            Description: null,
            Category: null,
            Tags: null,
            PricingType: null!,
            PriceAmount: null,
            Currency: null,
            Unit: null,
            Latitude: null,
            Longitude: null,
            Status: null,
            MediaIds: null);

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void CreateItemRequestValidator_WithInvalidPricingType_Fails()
    {
        var validator = new CreateItemRequestValidator();
        var request = new CreateItemRequest(
            TerritoryId: Guid.NewGuid(),
            StoreId: Guid.NewGuid(),
            Type: "Product",
            Title: "Test",
            Description: null,
            Category: null,
            Tags: null,
            PricingType: "InvalidPricing",
            PriceAmount: null,
            Currency: null,
            Unit: null,
            Latitude: null,
            Longitude: null,
            Status: null,
            MediaIds: null);

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void CreateItemRequestValidator_WithCaseInsensitivePricingType_Passes()
    {
        var validator = new CreateItemRequestValidator();
        var pricingTypes = new[] { "fixed", "FIXED", "Fixed", "negotiable", "NEGOTIABLE", "Negotiable", "free", "FREE", "Free" };

        foreach (var pricingType in pricingTypes)
        {
            var request = new CreateItemRequest(
                TerritoryId: Guid.NewGuid(),
                StoreId: Guid.NewGuid(),
                Type: "Product",
                Title: "Test",
                Description: null,
                Category: null,
                Tags: null,
                PricingType: pricingType,
                PriceAmount: null,
                Currency: null,
                Unit: null,
                Latitude: null,
                Longitude: null,
                Status: null,
                MediaIds: null);

            var result = validator.Validate(request);
            Assert.True(result.IsValid, $"PricingType '{pricingType}' should be valid");
        }
    }

    [Fact]
    public void CreateItemRequestValidator_WithFixedPricingAndZeroPrice_Fails()
    {
        var validator = new CreateItemRequestValidator();
        var request = new CreateItemRequest(
            TerritoryId: Guid.NewGuid(),
            StoreId: Guid.NewGuid(),
            Type: "Product",
            Title: "Test",
            Description: null,
            Category: null,
            Tags: null,
            PricingType: "Fixed",
            PriceAmount: 0m,
            Currency: null,
            Unit: null,
            Latitude: null,
            Longitude: null,
            Status: null,
            MediaIds: null);

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void CreateItemRequestValidator_WithFixedPricingAndNegativePrice_Fails()
    {
        var validator = new CreateItemRequestValidator();
        var request = new CreateItemRequest(
            TerritoryId: Guid.NewGuid(),
            StoreId: Guid.NewGuid(),
            Type: "Product",
            Title: "Test",
            Description: null,
            Category: null,
            Tags: null,
            PricingType: "Fixed",
            PriceAmount: -10m,
            Currency: null,
            Unit: null,
            Latitude: null,
            Longitude: null,
            Status: null,
            MediaIds: null);

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void CreateItemRequestValidator_WithFixedPricingAndValidPrice_Passes()
    {
        var validator = new CreateItemRequestValidator();
        var request = new CreateItemRequest(
            TerritoryId: Guid.NewGuid(),
            StoreId: Guid.NewGuid(),
            Type: "Product",
            Title: "Test",
            Description: null,
            Category: null,
            Tags: null,
            PricingType: "Fixed",
            PriceAmount: 100m,
            Currency: "BRL",
            Unit: null,
            Latitude: null,
            Longitude: null,
            Status: null,
            MediaIds: null);

        var result = validator.Validate(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void CreateItemRequestValidator_WithInvalidLatitude_Fails()
    {
        var validator = new CreateItemRequestValidator();
        var request = new CreateItemRequest(
            TerritoryId: Guid.NewGuid(),
            StoreId: Guid.NewGuid(),
            Type: "Product",
            Title: "Test",
            Description: null,
            Category: null,
            Tags: null,
            PricingType: "Fixed",
            PriceAmount: null,
            Currency: null,
            Unit: null,
            Latitude: 91.0,
            Longitude: null,
            Status: null,
            MediaIds: null);

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void CreateItemRequestValidator_WithInvalidLongitude_Fails()
    {
        var validator = new CreateItemRequestValidator();
        var request = new CreateItemRequest(
            TerritoryId: Guid.NewGuid(),
            StoreId: Guid.NewGuid(),
            Type: "Product",
            Title: "Test",
            Description: null,
            Category: null,
            Tags: null,
            PricingType: "Fixed",
            PriceAmount: null,
            Currency: null,
            Unit: null,
            Latitude: null,
            Longitude: 181.0,
            Status: null,
            MediaIds: null);

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void CreateItemRequestValidator_WithValidCoordinates_Passes()
    {
        var validator = new CreateItemRequestValidator();
        var request = new CreateItemRequest(
            TerritoryId: Guid.NewGuid(),
            StoreId: Guid.NewGuid(),
            Type: "Product",
            Title: "Test",
            Description: null,
            Category: null,
            Tags: null,
            PricingType: "Fixed",
            PriceAmount: null,
            Currency: null,
            Unit: null,
            Latitude: -23.5505,
            Longitude: -46.6333,
            Status: null,
            MediaIds: null);

        var result = validator.Validate(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void CreateItemRequestValidator_WithNullCoordinates_Passes()
    {
        var validator = new CreateItemRequestValidator();
        var request = new CreateItemRequest(
            TerritoryId: Guid.NewGuid(),
            StoreId: Guid.NewGuid(),
            Type: "Product",
            Title: "Test",
            Description: null,
            Category: null,
            Tags: null,
            PricingType: "Fixed",
            PriceAmount: null,
            Currency: null,
            Unit: null,
            Latitude: null,
            Longitude: null,
            Status: null,
            MediaIds: null);

        var result = validator.Validate(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void CreateItemRequestValidator_WithMoreThan10MediaIds_Fails()
    {
        var validator = new CreateItemRequestValidator();
        var request = new CreateItemRequest(
            TerritoryId: Guid.NewGuid(),
            StoreId: Guid.NewGuid(),
            Type: "Product",
            Title: "Test",
            Description: null,
            Category: null,
            Tags: null,
            PricingType: "Fixed",
            PriceAmount: null,
            Currency: null,
            Unit: null,
            Latitude: null,
            Longitude: null,
            Status: null,
            MediaIds: Enumerable.Range(0, 11).Select(_ => Guid.NewGuid()).ToList());

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void CreateItemRequestValidator_WithExactly10MediaIds_Passes()
    {
        var validator = new CreateItemRequestValidator();
        var request = new CreateItemRequest(
            TerritoryId: Guid.NewGuid(),
            StoreId: Guid.NewGuid(),
            Type: "Product",
            Title: "Test",
            Description: null,
            Category: null,
            Tags: null,
            PricingType: "Fixed",
            PriceAmount: null,
            Currency: null,
            Unit: null,
            Latitude: null,
            Longitude: null,
            Status: null,
            MediaIds: Enumerable.Range(0, 10).Select(_ => Guid.NewGuid()).ToList());

        var result = validator.Validate(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void CreateItemRequestValidator_WithEmptyGuidInMediaIds_Fails()
    {
        var validator = new CreateItemRequestValidator();
        var request = new CreateItemRequest(
            TerritoryId: Guid.NewGuid(),
            StoreId: Guid.NewGuid(),
            Type: "Product",
            Title: "Test",
            Description: null,
            Category: null,
            Tags: null,
            PricingType: "Fixed",
            PriceAmount: null,
            Currency: null,
            Unit: null,
            Latitude: null,
            Longitude: null,
            Status: null,
            MediaIds: new List<Guid> { Guid.NewGuid(), Guid.Empty, Guid.NewGuid() });

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void CreateItemRequestValidator_WithDuplicateMediaIds_Fails()
    {
        var validator = new CreateItemRequestValidator();
        var duplicateId = Guid.NewGuid();
        var request = new CreateItemRequest(
            TerritoryId: Guid.NewGuid(),
            StoreId: Guid.NewGuid(),
            Type: "Product",
            Title: "Test",
            Description: null,
            Category: null,
            Tags: null,
            PricingType: "Fixed",
            PriceAmount: null,
            Currency: null,
            Unit: null,
            Latitude: null,
            Longitude: null,
            Status: null,
            MediaIds: new List<Guid> { duplicateId, duplicateId, Guid.NewGuid() });

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
    }

    // UpsertStoreRequestValidator tests
    [Fact]
    public void UpsertStoreRequestValidator_WithValidRequest_Passes()
    {
        var validator = new UpsertStoreRequestValidator();
        var request = new UpsertStoreRequest(
            TerritoryId: Guid.NewGuid(),
            DisplayName: "Test Store",
            Description: null,
            ContactVisibility: "Public",
            Contact: null);

        var result = validator.Validate(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void UpsertStoreRequestValidator_WithEmptyTerritoryId_Fails()
    {
        var validator = new UpsertStoreRequestValidator();
        var request = new UpsertStoreRequest(
            TerritoryId: Guid.Empty,
            DisplayName: "Test Store",
            Description: null,
            ContactVisibility: "Public",
            Contact: null);

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void UpsertStoreRequestValidator_WithNullDisplayName_Fails()
    {
        var validator = new UpsertStoreRequestValidator();
        var request = new UpsertStoreRequest(
            TerritoryId: Guid.NewGuid(),
            DisplayName: null!,
            Description: null,
            ContactVisibility: "Public",
            Contact: null);

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void UpsertStoreRequestValidator_WithEmptyDisplayName_Fails()
    {
        var validator = new UpsertStoreRequestValidator();
        var request = new UpsertStoreRequest(
            TerritoryId: Guid.NewGuid(),
            DisplayName: "   ",
            Description: null,
            ContactVisibility: "Public",
            Contact: null);

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void UpsertStoreRequestValidator_WithDisplayNameExceeding200Chars_Fails()
    {
        var validator = new UpsertStoreRequestValidator();
        var request = new UpsertStoreRequest(
            TerritoryId: Guid.NewGuid(),
            DisplayName: new string('A', 201),
            Description: null,
            ContactVisibility: "Public",
            Contact: null);

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void UpsertStoreRequestValidator_WithDisplayNameExactly200Chars_Passes()
    {
        var validator = new UpsertStoreRequestValidator();
        var request = new UpsertStoreRequest(
            TerritoryId: Guid.NewGuid(),
            DisplayName: new string('A', 200),
            Description: null,
            ContactVisibility: "Public",
            Contact: null);

        var result = validator.Validate(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void UpsertStoreRequestValidator_WithDescriptionExceeding2000Chars_Fails()
    {
        var validator = new UpsertStoreRequestValidator();
        var request = new UpsertStoreRequest(
            TerritoryId: Guid.NewGuid(),
            DisplayName: "Test Store",
            Description: new string('A', 2001),
            ContactVisibility: "Public",
            Contact: null);

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void UpsertStoreRequestValidator_WithDescriptionExactly2000Chars_Passes()
    {
        var validator = new UpsertStoreRequestValidator();
        var request = new UpsertStoreRequest(
            TerritoryId: Guid.NewGuid(),
            DisplayName: "Test Store",
            Description: new string('A', 2000),
            ContactVisibility: "Public",
            Contact: null);

        var result = validator.Validate(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void UpsertStoreRequestValidator_WithNullContactVisibility_Fails()
    {
        var validator = new UpsertStoreRequestValidator();
        var request = new UpsertStoreRequest(
            TerritoryId: Guid.NewGuid(),
            DisplayName: "Test Store",
            Description: null,
            ContactVisibility: null!,
            Contact: null);

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void UpsertStoreRequestValidator_WithInvalidContactVisibility_Fails()
    {
        var validator = new UpsertStoreRequestValidator();
        var request = new UpsertStoreRequest(
            TerritoryId: Guid.NewGuid(),
            DisplayName: "Test Store",
            Description: null,
            ContactVisibility: "InvalidVisibility",
            Contact: null);

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void UpsertStoreRequestValidator_WithCaseInsensitiveContactVisibility_Passes()
    {
        var validator = new UpsertStoreRequestValidator();
        var visibilities = new[] { "public", "PUBLIC", "Public", "onInquiryOnly", "ON_INQUIRY_ONLY", "OnInquiryOnly" };

        foreach (var visibility in visibilities)
        {
            var request = new UpsertStoreRequest(
                TerritoryId: Guid.NewGuid(),
                DisplayName: "Test Store",
                Description: null,
                ContactVisibility: visibility,
                Contact: null);

            var result = validator.Validate(request);
            Assert.True(result.IsValid, $"ContactVisibility '{visibility}' should be valid");
        }
    }

    [Fact]
    public void UpsertStoreRequestValidator_WithInvalidEmail_Fails()
    {
        var validator = new UpsertStoreRequestValidator();
        var request = new UpsertStoreRequest(
            TerritoryId: Guid.NewGuid(),
            DisplayName: "Test Store",
            Description: null,
            ContactVisibility: "Public",
            Contact: new StoreContactPayload(
                Phone: null,
                Whatsapp: null,
                Email: "invalid-email",
                Instagram: null,
                Website: null,
                PreferredContactMethod: null));

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void UpsertStoreRequestValidator_WithValidEmail_Passes()
    {
        var validator = new UpsertStoreRequestValidator();
        var request = new UpsertStoreRequest(
            TerritoryId: Guid.NewGuid(),
            DisplayName: "Test Store",
            Description: null,
            ContactVisibility: "Public",
            Contact: new StoreContactPayload(
                Phone: null,
                Whatsapp: null,
                Email: "test@example.com",
                Instagram: null,
                Website: null,
                PreferredContactMethod: null));

        var result = validator.Validate(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void UpsertStoreRequestValidator_WithInvalidWebsite_Fails()
    {
        var validator = new UpsertStoreRequestValidator();
        var request = new UpsertStoreRequest(
            TerritoryId: Guid.NewGuid(),
            DisplayName: "Test Store",
            Description: null,
            ContactVisibility: "Public",
            Contact: new StoreContactPayload(
                Phone: null,
                Whatsapp: null,
                Email: null,
                Instagram: null,
                Website: "not-a-valid-url",
                PreferredContactMethod: null));

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void UpsertStoreRequestValidator_WithValidWebsite_Passes()
    {
        var validator = new UpsertStoreRequestValidator();
        var request = new UpsertStoreRequest(
            TerritoryId: Guid.NewGuid(),
            DisplayName: "Test Store",
            Description: null,
            ContactVisibility: "Public",
            Contact: new StoreContactPayload(
                Phone: null,
                Whatsapp: null,
                Email: null,
                Instagram: null,
                Website: "https://example.com",
                PreferredContactMethod: null));

        var result = validator.Validate(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void UpsertStoreRequestValidator_WithPhoneExceeding20Chars_Fails()
    {
        var validator = new UpsertStoreRequestValidator();
        var request = new UpsertStoreRequest(
            TerritoryId: Guid.NewGuid(),
            DisplayName: "Test Store",
            Description: null,
            ContactVisibility: "Public",
            Contact: new StoreContactPayload(
                Phone: new string('1', 21),
                Whatsapp: null,
                Email: null,
                Instagram: null,
                Website: null,
                PreferredContactMethod: null));

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void UpsertStoreRequestValidator_WithPhoneExactly20Chars_Passes()
    {
        var validator = new UpsertStoreRequestValidator();
        var request = new UpsertStoreRequest(
            TerritoryId: Guid.NewGuid(),
            DisplayName: "Test Store",
            Description: null,
            ContactVisibility: "Public",
            Contact: new StoreContactPayload(
                Phone: new string('1', 20),
                Whatsapp: null,
                Email: null,
                Instagram: null,
                Website: null,
                PreferredContactMethod: null));

        var result = validator.Validate(request);

        Assert.True(result.IsValid);
    }

    // GeoValidationRules tests
    [Fact]
    public void GeoValidationRules_IsValidLatitude_WithValidValues_ReturnsTrue()
    {
        Assert.True(GeoValidationRules.IsValidLatitude(0));
        Assert.True(GeoValidationRules.IsValidLatitude(90));
        Assert.True(GeoValidationRules.IsValidLatitude(-90));
        Assert.True(GeoValidationRules.IsValidLatitude(45.5));
    }

    [Fact]
    public void GeoValidationRules_IsValidLatitude_WithInvalidValues_ReturnsFalse()
    {
        Assert.False(GeoValidationRules.IsValidLatitude(91));
        Assert.False(GeoValidationRules.IsValidLatitude(-91));
        Assert.False(GeoValidationRules.IsValidLatitude(100));
        Assert.False(GeoValidationRules.IsValidLatitude(-100));
    }

    [Fact]
    public void GeoValidationRules_IsValidLongitude_WithValidValues_ReturnsTrue()
    {
        Assert.True(GeoValidationRules.IsValidLongitude(0));
        Assert.True(GeoValidationRules.IsValidLongitude(180));
        Assert.True(GeoValidationRules.IsValidLongitude(-180));
        Assert.True(GeoValidationRules.IsValidLongitude(45.5));
    }

    [Fact]
    public void GeoValidationRules_IsValidLongitude_WithInvalidValues_ReturnsFalse()
    {
        Assert.False(GeoValidationRules.IsValidLongitude(181));
        Assert.False(GeoValidationRules.IsValidLongitude(-181));
        Assert.False(GeoValidationRules.IsValidLongitude(200));
        Assert.False(GeoValidationRules.IsValidLongitude(-200));
    }
}
