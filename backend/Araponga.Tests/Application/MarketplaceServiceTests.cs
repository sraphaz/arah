using Araponga.Application.Services;
using Araponga.Domain.Marketplace;
using Araponga.Domain.Users;
using Araponga.Infrastructure.InMemory;
using Xunit;

namespace Araponga.Tests.Application;

public sealed class MarketplaceServiceTests
{
    private static readonly Guid TerritoryId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    private static readonly Guid ResidentUserId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

    [Fact]
    public async Task ResidentCanCreateStoreAndListing()
    {
        var dataStore = new InMemoryDataStore();
        var storeRepository = new InMemoryStoreRepository(dataStore);
        var listingRepository = new InMemoryListingRepository(dataStore);
        var membershipRepository = new InMemoryTerritoryMembershipRepository(dataStore);
        var userRepository = new InMemoryUserRepository(dataStore);
        var unitOfWork = new InMemoryUnitOfWork();
        var accessEvaluator = new AccessEvaluator(membershipRepository);
        var storeService = new StoreService(storeRepository, userRepository, accessEvaluator, unitOfWork);
        var listingService = new ListingService(listingRepository, storeRepository, userRepository, accessEvaluator, unitOfWork);

        var storeResult = await storeService.UpsertMyStoreAsync(
            TerritoryId,
            ResidentUserId,
            "Loja do Morador",
            "Descrição",
            StoreContactVisibility.OnInquiryOnly,
            "(11) 90000-0000",
            null,
            "loja@exemplo.com",
            null,
            null,
            "whatsapp",
            CancellationToken.None);

        Assert.True(storeResult.success);
        Assert.NotNull(storeResult.store);

        var listingResult = await listingService.CreateListingAsync(
            TerritoryId,
            ResidentUserId,
            storeResult.store!.Id,
            ListingType.Product,
            "Produto",
            null,
            "Categoria",
            "tag",
            ListingPricingType.Fixed,
            10m,
            "BRL",
            "unidade",
            null,
            null,
            ListingStatus.Active,
            CancellationToken.None);

        Assert.True(listingResult.success);
        Assert.NotNull(listingResult.listing);
    }

    [Fact]
    public async Task VisitorCannotCreateStoreOrListing()
    {
        var dataStore = new InMemoryDataStore();
        var visitor = new User(
            Guid.NewGuid(),
            "Visitante",
            "visitor@araponga.com",
            null,
            "FOREIGN-123",
            "(11) 95555-0000",
            "Rua 1",
            "google",
            "visitor-external",
            UserRole.Visitor,
            DateTime.UtcNow);
        dataStore.Users.Add(visitor);

        var storeRepository = new InMemoryStoreRepository(dataStore);
        var listingRepository = new InMemoryListingRepository(dataStore);
        var membershipRepository = new InMemoryTerritoryMembershipRepository(dataStore);
        var userRepository = new InMemoryUserRepository(dataStore);
        var unitOfWork = new InMemoryUnitOfWork();
        var accessEvaluator = new AccessEvaluator(membershipRepository);
        var storeService = new StoreService(storeRepository, userRepository, accessEvaluator, unitOfWork);
        var listingService = new ListingService(listingRepository, storeRepository, userRepository, accessEvaluator, unitOfWork);

        var storeResult = await storeService.UpsertMyStoreAsync(
            TerritoryId,
            visitor.Id,
            "Loja",
            null,
            StoreContactVisibility.OnInquiryOnly,
            null,
            null,
            null,
            null,
            null,
            null,
            CancellationToken.None);

        Assert.False(storeResult.success);

        var residentStore = await storeService.UpsertMyStoreAsync(
            TerritoryId,
            ResidentUserId,
            "Loja Morador",
            null,
            StoreContactVisibility.OnInquiryOnly,
            null,
            null,
            null,
            null,
            null,
            null,
            CancellationToken.None);

        var listingResult = await listingService.CreateListingAsync(
            TerritoryId,
            visitor.Id,
            residentStore.store!.Id,
            ListingType.Product,
            "Produto",
            null,
            null,
            null,
            ListingPricingType.Fixed,
            5m,
            "BRL",
            "unidade",
            null,
            null,
            ListingStatus.Active,
            CancellationToken.None);

        Assert.False(listingResult.success);
    }

    [Fact]
    public async Task SearchListingsAndCheckoutCalculatesFees()
    {
        var dataStore = new InMemoryDataStore();
        var storeRepository = new InMemoryStoreRepository(dataStore);
        var listingRepository = new InMemoryListingRepository(dataStore);
        var inquiryRepository = new InMemoryInquiryRepository(dataStore);
        var cartRepository = new InMemoryCartRepository(dataStore);
        var cartItemRepository = new InMemoryCartItemRepository(dataStore);
        var checkoutRepository = new InMemoryCheckoutRepository(dataStore);
        var checkoutItemRepository = new InMemoryCheckoutItemRepository(dataStore);
        var feeRepository = new InMemoryPlatformFeeConfigRepository(dataStore);
        var membershipRepository = new InMemoryTerritoryMembershipRepository(dataStore);
        var userRepository = new InMemoryUserRepository(dataStore);
        var unitOfWork = new InMemoryUnitOfWork();
        var accessEvaluator = new AccessEvaluator(membershipRepository);
        var storeService = new StoreService(storeRepository, userRepository, accessEvaluator, unitOfWork);
        var listingService = new ListingService(listingRepository, storeRepository, userRepository, accessEvaluator, unitOfWork);
        var inquiryService = new InquiryService(inquiryRepository, listingRepository, storeRepository, unitOfWork);
        var cartService = new CartService(
            cartRepository,
            cartItemRepository,
            listingRepository,
            storeRepository,
            checkoutRepository,
            checkoutItemRepository,
            inquiryRepository,
            feeRepository,
            unitOfWork);
        var platformFeeService = new PlatformFeeService(feeRepository, unitOfWork);

        var storeResult = await storeService.UpsertMyStoreAsync(
            TerritoryId,
            ResidentUserId,
            "Loja",
            null,
            StoreContactVisibility.Public,
            "(11) 90000-0000",
            null,
            "loja@exemplo.com",
            null,
            null,
            "email",
            CancellationToken.None);
        var store = storeResult.store!;
        await storeService.SetPaymentsEnabledAsync(store.Id, ResidentUserId, true, CancellationToken.None);

        var product = await listingService.CreateListingAsync(
            TerritoryId,
            ResidentUserId,
            store.Id,
            ListingType.Product,
            "Mel",
            "Mel local",
            "Alimentos",
            "mel",
            ListingPricingType.Fixed,
            30m,
            "BRL",
            "unidade",
            null,
            null,
            ListingStatus.Active,
            CancellationToken.None);

        var service = await listingService.CreateListingAsync(
            TerritoryId,
            ResidentUserId,
            store.Id,
            ListingType.Service,
            "Aula",
            null,
            "Esportes",
            "aula",
            ListingPricingType.Negotiable,
            null,
            "BRL",
            "hora",
            null,
            null,
            ListingStatus.Active,
            CancellationToken.None);

        await platformFeeService.UpsertFeeConfigAsync(
            TerritoryId,
            ListingType.Product,
            PlatformFeeMode.Percentage,
            0.05m,
            "BRL",
            true,
            CancellationToken.None);

        var listings = await listingService.SearchListingsAsync(
            TerritoryId,
            ListingType.Product,
            "mel",
            null,
            null,
            ListingStatus.Active,
            CancellationToken.None);

        Assert.Single(listings);

        var inquiry = await inquiryService.CreateInquiryAsync(service.listing!.Id, ResidentUserId, "Contato", null, CancellationToken.None);
        Assert.True(inquiry.success);
        Assert.NotNull(inquiry.contact);

        await cartService.AddItemAsync(TerritoryId, ResidentUserId, product.listing!.Id, 2, null, CancellationToken.None);
        await cartService.AddItemAsync(TerritoryId, ResidentUserId, service.listing!.Id, 1, null, CancellationToken.None);

        var checkout = await cartService.CheckoutAsync(TerritoryId, ResidentUserId, "mensagem", CancellationToken.None);
        Assert.True(checkout.success);
        Assert.NotNull(checkout.result);
        Assert.Single(checkout.result!.Checkouts);
        Assert.Single(checkout.result.Inquiries);

        var createdCheckout = checkout.result.Checkouts[0].Checkout;
        Assert.Equal(60m, createdCheckout.ItemsSubtotalAmount);
        Assert.Equal(3m, createdCheckout.PlatformFeeAmount);
        Assert.Equal(63m, createdCheckout.TotalAmount);
    }
}
