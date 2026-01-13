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

    [Fact]
    public async Task StoreService_UpdateAndStatusChanges()
    {
        var dataStore = new InMemoryDataStore();
        var storeRepository = new InMemoryStoreRepository(dataStore);
        var membershipRepository = new InMemoryTerritoryMembershipRepository(dataStore);
        var userRepository = new InMemoryUserRepository(dataStore);
        var unitOfWork = new InMemoryUnitOfWork();
        var accessEvaluator = new AccessEvaluator(membershipRepository);
        var storeService = new StoreService(storeRepository, userRepository, accessEvaluator, unitOfWork);

        var createResult = await storeService.UpsertMyStoreAsync(
            TerritoryId,
            ResidentUserId,
            "Loja Original",
            "Descrição original",
            StoreContactVisibility.Public,
            "(11) 90000-0000",
            null,
            "loja@exemplo.com",
            null,
            null,
            "email",
            CancellationToken.None);

        Assert.True(createResult.success);
        var store = createResult.store!;

        var updateResult = await storeService.UpdateStoreAsync(
            store.Id,
            ResidentUserId,
            "Loja Atualizada",
            "Nova descrição",
            StoreContactVisibility.OnInquiryOnly,
            "(11) 99999-9999",
            null,
            "novo@exemplo.com",
            null,
            null,
            "whatsapp",
            CancellationToken.None);

        Assert.True(updateResult.success);
        Assert.Equal("Loja Atualizada", updateResult.store!.DisplayName);
        Assert.Equal(StoreContactVisibility.OnInquiryOnly, updateResult.store.ContactVisibility);

        var pauseResult = await storeService.SetStoreStatusAsync(store.Id, ResidentUserId, StoreStatus.Paused, CancellationToken.None);
        Assert.True(pauseResult.success);
        Assert.Equal(StoreStatus.Paused, pauseResult.store!.Status);

        var activateResult = await storeService.SetStoreStatusAsync(store.Id, ResidentUserId, StoreStatus.Active, CancellationToken.None);
        Assert.True(activateResult.success);
        Assert.Equal(StoreStatus.Active, activateResult.store!.Status);
    }

    [Fact]
    public async Task StoreService_GetMyStoreReturnsNullWhenNotExists()
    {
        var dataStore = new InMemoryDataStore();
        var storeRepository = new InMemoryStoreRepository(dataStore);
        var membershipRepository = new InMemoryTerritoryMembershipRepository(dataStore);
        var userRepository = new InMemoryUserRepository(dataStore);
        var unitOfWork = new InMemoryUnitOfWork();
        var accessEvaluator = new AccessEvaluator(membershipRepository);
        var storeService = new StoreService(storeRepository, userRepository, accessEvaluator, unitOfWork);

        var store = await storeService.GetMyStoreAsync(TerritoryId, ResidentUserId, CancellationToken.None);
        Assert.Null(store);
    }

    [Fact]
    public async Task ListingService_UpdateAndArchive()
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
            "Loja",
            null,
            StoreContactVisibility.Public,
            null,
            null,
            null,
            null,
            null,
            null,
            CancellationToken.None);

        var createResult = await listingService.CreateListingAsync(
            TerritoryId,
            ResidentUserId,
            storeResult.store!.Id,
            ListingType.Product,
            "Produto Original",
            "Descrição original",
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

        Assert.True(createResult.success);
        var listing = createResult.listing!;

        var updateResult = await listingService.UpdateListingAsync(
            listing.Id,
            ResidentUserId,
            ListingType.Product,
            "Produto Atualizado",
            "Nova descrição",
            "Nova categoria",
            "nova-tag",
            ListingPricingType.Fixed,
            15m,
            "BRL",
            "unidade",
            null,
            null,
            ListingStatus.Active,
            CancellationToken.None);

        Assert.True(updateResult.success);
        Assert.Equal("Produto Atualizado", updateResult.listing!.Title);
        Assert.Equal(15m, updateResult.listing.PriceAmount);

        var archiveResult = await listingService.ArchiveListingAsync(listing.Id, ResidentUserId, CancellationToken.None);
        Assert.True(archiveResult.success);
        Assert.Equal(ListingStatus.Archived, archiveResult.listing!.Status);
    }

    [Fact]
    public async Task ListingService_SearchFiltersWork()
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
            "Loja",
            null,
            StoreContactVisibility.Public,
            null,
            null,
            null,
            null,
            null,
            null,
            CancellationToken.None);

        await listingService.CreateListingAsync(
            TerritoryId,
            ResidentUserId,
            storeResult.store!.Id,
            ListingType.Product,
            "Mel",
            "Mel local",
            "Alimentos",
            "mel,doce",
            ListingPricingType.Fixed,
            30m,
            "BRL",
            "unidade",
            null,
            null,
            ListingStatus.Active,
            CancellationToken.None);

        await listingService.CreateListingAsync(
            TerritoryId,
            ResidentUserId,
            storeResult.store.Id,
            ListingType.Service,
            "Aula",
            "Aula de música",
            "Educação",
            "aula,musica",
            ListingPricingType.Negotiable,
            null,
            "BRL",
            "hora",
            null,
            null,
            ListingStatus.Active,
            CancellationToken.None);

        var productResults = await listingService.SearchListingsAsync(
            TerritoryId,
            ListingType.Product,
            "mel",
            null,
            null,
            ListingStatus.Active,
            CancellationToken.None);

        Assert.Single(productResults);
        Assert.Equal(ListingType.Product, productResults[0].Type);

        var categoryResults = await listingService.SearchListingsAsync(
            TerritoryId,
            null,
            null,
            "Educação",
            null,
            ListingStatus.Active,
            CancellationToken.None);

        Assert.Single(categoryResults);
        Assert.Equal("Educação", categoryResults[0].Category);
    }

    [Fact]
    public async Task CartService_AddUpdateRemoveItems()
    {
        var dataStore = new InMemoryDataStore();
        var storeRepository = new InMemoryStoreRepository(dataStore);
        var listingRepository = new InMemoryListingRepository(dataStore);
        var cartRepository = new InMemoryCartRepository(dataStore);
        var cartItemRepository = new InMemoryCartItemRepository(dataStore);
        var checkoutRepository = new InMemoryCheckoutRepository(dataStore);
        var checkoutItemRepository = new InMemoryCheckoutItemRepository(dataStore);
        var inquiryRepository = new InMemoryInquiryRepository(dataStore);
        var feeRepository = new InMemoryPlatformFeeConfigRepository(dataStore);
        var membershipRepository = new InMemoryTerritoryMembershipRepository(dataStore);
        var userRepository = new InMemoryUserRepository(dataStore);
        var unitOfWork = new InMemoryUnitOfWork();
        var accessEvaluator = new AccessEvaluator(membershipRepository);
        var storeService = new StoreService(storeRepository, userRepository, accessEvaluator, unitOfWork);
        var listingService = new ListingService(listingRepository, storeRepository, userRepository, accessEvaluator, unitOfWork);
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

        var storeResult = await storeService.UpsertMyStoreAsync(
            TerritoryId,
            ResidentUserId,
            "Loja",
            null,
            StoreContactVisibility.Public,
            null,
            null,
            null,
            null,
            null,
            null,
            CancellationToken.None);

        var listingResult = await listingService.CreateListingAsync(
            TerritoryId,
            ResidentUserId,
            storeResult.store!.Id,
            ListingType.Product,
            "Produto",
            null,
            null,
            null,
            ListingPricingType.Fixed,
            10m,
            "BRL",
            "unidade",
            null,
            null,
            ListingStatus.Active,
            CancellationToken.None);

        var addResult = await cartService.AddItemAsync(
            TerritoryId,
            ResidentUserId,
            listingResult.listing!.Id,
            2,
            "Nota inicial",
            CancellationToken.None);

        Assert.True(addResult.success);
        Assert.NotNull(addResult.item);
        Assert.Equal(2, addResult.item.Quantity);

        var cart = await cartService.GetCartAsync(TerritoryId, ResidentUserId, CancellationToken.None);
        Assert.Single(cart.Items);

        var updateResult = await cartService.UpdateItemAsync(
            addResult.item!.Id,
            ResidentUserId,
            3,
            "Nota atualizada",
            CancellationToken.None);

        Assert.True(updateResult.success);
        Assert.Equal(3, updateResult.item!.Quantity);
        Assert.Equal("Nota atualizada", updateResult.item.Notes);

        var removeResult = await cartService.RemoveItemAsync(addResult.item.Id, ResidentUserId, CancellationToken.None);
        Assert.True(removeResult);

        var emptyCart = await cartService.GetCartAsync(TerritoryId, ResidentUserId, CancellationToken.None);
        Assert.Empty(emptyCart.Items);
    }

    [Fact]
    public async Task InquiryService_ListMyAndReceivedInquiries()
    {
        var dataStore = new InMemoryDataStore();
        var inquiryRepository = new InMemoryInquiryRepository(dataStore);
        var listingRepository = new InMemoryListingRepository(dataStore);
        var storeRepository = new InMemoryStoreRepository(dataStore);
        var membershipRepository = new InMemoryTerritoryMembershipRepository(dataStore);
        var userRepository = new InMemoryUserRepository(dataStore);
        var unitOfWork = new InMemoryUnitOfWork();
        var accessEvaluator = new AccessEvaluator(membershipRepository);
        var storeService = new StoreService(storeRepository, userRepository, accessEvaluator, unitOfWork);
        var listingService = new ListingService(listingRepository, storeRepository, userRepository, accessEvaluator, unitOfWork);
        var inquiryService = new InquiryService(inquiryRepository, listingRepository, storeRepository, unitOfWork);

        var buyerId = Guid.NewGuid();
        await userRepository.AddAsync(
            new User(buyerId, "Comprador", "comprador@araponga.com", "123.456.789-00", null, null, null, "google", "buyer-ext", UserRole.Visitor, DateTime.UtcNow),
            CancellationToken.None);

        var storeResult = await storeService.UpsertMyStoreAsync(
            TerritoryId,
            ResidentUserId,
            "Loja",
            null,
            StoreContactVisibility.Public,
            null,
            null,
            null,
            null,
            null,
            null,
            CancellationToken.None);

        var listingResult = await listingService.CreateListingAsync(
            TerritoryId,
            ResidentUserId,
            storeResult.store!.Id,
            ListingType.Product,
            "Produto",
            null,
            null,
            null,
            ListingPricingType.Fixed,
            10m,
            "BRL",
            "unidade",
            null,
            null,
            ListingStatus.Active,
            CancellationToken.None);

        var inquiryResult = await inquiryService.CreateInquiryAsync(
            listingResult.listing!.Id,
            buyerId,
            "Quero comprar",
            null,
            CancellationToken.None);

        Assert.True(inquiryResult.success);

        var myInquiries = await inquiryService.ListMyInquiriesAsync(buyerId, CancellationToken.None);
        Assert.Single(myInquiries);
        Assert.Equal(buyerId, myInquiries[0].FromUserId);

        var receivedInquiries = await inquiryService.ListReceivedInquiriesAsync(ResidentUserId, CancellationToken.None);
        Assert.Single(receivedInquiries);
        Assert.Equal(ResidentUserId, receivedInquiries[0].StoreId);
    }
}
