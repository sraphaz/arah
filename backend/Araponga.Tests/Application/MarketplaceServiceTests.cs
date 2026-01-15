using Araponga.Application.Interfaces;
using Araponga.Application.Services;
using Araponga.Application.Models;
using Araponga.Domain.Marketplace;
using Araponga.Domain.Membership;
using Araponga.Domain.Users;
using Araponga.Infrastructure.InMemory;
using Araponga.Tests.TestHelpers;
using Xunit;

namespace Araponga.Tests.Application;

public sealed class MarketplaceServiceTests
{
    private static readonly Guid TerritoryId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    private static readonly Guid ResidentUserId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

    private static async Task<(StoreService storeService, StoreItemService itemService)> CreateServicesAsync(
        InMemoryDataStore dataStore,
        Guid territoryId,
        CancellationToken cancellationToken)
    {
        var storeRepository = new InMemoryStoreRepository(dataStore);
        var itemRepository = new InMemoryStoreItemRepository(dataStore);
        var membershipRepository = new InMemoryTerritoryMembershipRepository(dataStore);
        var settingsRepository = new InMemoryMembershipSettingsRepository(dataStore);
        var capabilityRepository = new InMemoryMembershipCapabilityRepository(dataStore);
        var userRepository = new InMemoryUserRepository(dataStore);
        var unitOfWork = new InMemoryUnitOfWork();
        var cache = CacheTestHelper.CreateDistributedCacheService();

        var featureFlags = new InMemoryFeatureFlagService();
        featureFlags.SetEnabledFlags(territoryId, new List<FeatureFlag> { FeatureFlag.MarketplaceEnabled });
        var featureFlagCache = new FeatureFlagCacheService(featureFlags, cache);
        var featureGuard = new TerritoryFeatureFlagGuard(featureFlagCache);

        var membershipAccessRules = new MembershipAccessRules(
            membershipRepository,
            settingsRepository,
            userRepository,
            featureFlags);

        var systemPermissionRepository = new InMemorySystemPermissionRepository(dataStore);
        var accessEvaluator = new AccessEvaluator(
            membershipRepository,
            capabilityRepository,
            systemPermissionRepository,
            membershipAccessRules,
            cache);

        // Garantir opt-in do residente default do InMemoryDataStore
        var residentMembership = await membershipRepository.GetByUserAndTerritoryAsync(ResidentUserId, territoryId, cancellationToken);
        if (residentMembership is not null)
        {
            var residentSettings = await settingsRepository.GetByMembershipIdAsync(residentMembership.Id, cancellationToken);
            if (residentSettings is not null && !residentSettings.MarketplaceOptIn)
            {
                residentSettings.UpdateMarketplaceOptIn(true, DateTime.UtcNow);
                await settingsRepository.UpdateAsync(residentSettings, cancellationToken);
            }
        }

        var storeService = new StoreService(storeRepository, userRepository, accessEvaluator, membershipAccessRules, unitOfWork);
        var itemService = new StoreItemService(itemRepository, storeRepository, userRepository, accessEvaluator, membershipAccessRules, featureGuard, unitOfWork);

        return (storeService, itemService);
    }

    private static async Task<(MembershipAccessRules rules, AccessEvaluator evaluator, TerritoryFeatureFlagGuard featureGuard)> CreateAccessAsync(
        InMemoryDataStore dataStore,
        ITerritoryMembershipRepository membershipRepository,
        IUserRepository userRepository,
        IDistributedCacheService cache,
        Guid territoryId,
        CancellationToken cancellationToken)
    {
        var settingsRepository = new InMemoryMembershipSettingsRepository(dataStore);
        var capabilityRepository = new InMemoryMembershipCapabilityRepository(dataStore);

        var featureFlags = new InMemoryFeatureFlagService();
        featureFlags.SetEnabledFlags(territoryId, new List<FeatureFlag> { FeatureFlag.MarketplaceEnabled });
        var featureFlagCache = new FeatureFlagCacheService(featureFlags, cache);
        var featureGuard = new TerritoryFeatureFlagGuard(featureFlagCache);

        var rules = new MembershipAccessRules(
            membershipRepository,
            settingsRepository,
            userRepository,
            featureFlags);

        var systemPermissionRepository = new InMemorySystemPermissionRepository(dataStore);
        var evaluator = new AccessEvaluator(
            membershipRepository,
            capabilityRepository,
            systemPermissionRepository,
            rules,
            cache);

        // Garantir opt-in do residente default do InMemoryDataStore
        var residentMembership = await membershipRepository.GetByUserAndTerritoryAsync(ResidentUserId, territoryId, cancellationToken);
        if (residentMembership is not null)
        {
            var residentSettings = await settingsRepository.GetByMembershipIdAsync(residentMembership.Id, cancellationToken);
            if (residentSettings is not null && !residentSettings.MarketplaceOptIn)
            {
                residentSettings.UpdateMarketplaceOptIn(true, DateTime.UtcNow);
                await settingsRepository.UpdateAsync(residentSettings, cancellationToken);
            }
        }

        return (rules, evaluator, featureGuard);
    }

    [Fact]
    public async Task ResidentCanCreateStoreAndListing()
    {
        var dataStore = new InMemoryDataStore();
        var (storeService, itemService) = await CreateServicesAsync(dataStore, TerritoryId, CancellationToken.None);

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

        Assert.True(storeResult.IsSuccess);
        Assert.NotNull(storeResult.Value);

        var listingResult = await itemService.CreateItemAsync(
            TerritoryId,
            ResidentUserId,
            storeResult.Value!.Id,
            ItemType.Product,
            "Produto",
            null,
            "Categoria",
            "tag",
            ItemPricingType.Fixed,
            10m,
            "BRL",
            "unidade",
            null,
            null,
            ItemStatus.Active,
            CancellationToken.None);

        Assert.True(listingResult.IsSuccess);
        Assert.NotNull(listingResult.Value);
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
            DateTime.UtcNow);
        dataStore.Users.Add(visitor);

        var (storeService, itemService) = await CreateServicesAsync(dataStore, TerritoryId, CancellationToken.None);

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

        Assert.False(storeResult.IsSuccess);

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

        var listingResult = await itemService.CreateItemAsync(
            TerritoryId,
            visitor.Id,
            residentStore.Value!.Id,
            ItemType.Product,
            "Produto",
            null,
            null,
            null,
            ItemPricingType.Fixed,
            5m,
            "BRL",
            "unidade",
            null,
            null,
            ItemStatus.Active,
            CancellationToken.None);

        Assert.False(listingResult.IsSuccess);
    }

    [Fact]
    public async Task SearchListingsAndCheckoutCalculatesFees()
    {
        var dataStore = new InMemoryDataStore();
        var storeRepository = new InMemoryStoreRepository(dataStore);
        var itemRepository = new InMemoryStoreItemRepository(dataStore);
        var inquiryRepository = new InMemoryInquiryRepository(dataStore);
        var cartRepository = new InMemoryCartRepository(dataStore);
        var cartItemRepository = new InMemoryCartItemRepository(dataStore);
        var checkoutRepository = new InMemoryCheckoutRepository(dataStore);
        var checkoutItemRepository = new InMemoryCheckoutItemRepository(dataStore);
        var feeRepository = new InMemoryPlatformFeeConfigRepository(dataStore);
        var membershipRepository = new InMemoryTerritoryMembershipRepository(dataStore);
        var userRepository = new InMemoryUserRepository(dataStore);
        var unitOfWork = new InMemoryUnitOfWork();
        var cache = CacheTestHelper.CreateDistributedCacheService();
        var (membershipAccessRules, accessEvaluator, featureGuard) = await CreateAccessAsync(
            dataStore,
            membershipRepository,
            userRepository,
            cache,
            TerritoryId,
            CancellationToken.None);
        var storeService = new StoreService(storeRepository, userRepository, accessEvaluator, membershipAccessRules, unitOfWork);
        var itemService = new StoreItemService(itemRepository, storeRepository, userRepository, accessEvaluator, membershipAccessRules, featureGuard, unitOfWork);
        var inquiryService = new InquiryService(inquiryRepository, itemRepository, storeRepository, featureGuard, unitOfWork);
        var cartService = new CartService(
            cartRepository,
            cartItemRepository,
            itemRepository,
            storeRepository,
            checkoutRepository,
            checkoutItemRepository,
            inquiryRepository,
            feeRepository,
            featureGuard,
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
        var store = storeResult.Value!;
        await storeService.SetPaymentsEnabledAsync(store.Id, ResidentUserId, true, CancellationToken.None);

        var product = await itemService.CreateItemAsync(
            TerritoryId,
            ResidentUserId,
            store.Id,
            ItemType.Product,
            "Mel",
            "Mel local",
            "Alimentos",
            "mel",
            ItemPricingType.Fixed,
            30m,
            "BRL",
            "unidade",
            null,
            null,
            ItemStatus.Active,
            CancellationToken.None);

        var service = await itemService.CreateItemAsync(
            TerritoryId,
            ResidentUserId,
            store.Id,
            ItemType.Service,
            "Aula",
            null,
            "Esportes",
            "aula",
            ItemPricingType.Negotiable,
            null,
            "BRL",
            "hora",
            null,
            null,
            ItemStatus.Active,
            CancellationToken.None);

        await platformFeeService.UpsertFeeConfigAsync(
            TerritoryId,
            ItemType.Product,
            PlatformFeeMode.Percentage,
            0.05m,
            "BRL",
            true,
            CancellationToken.None);

        var listingsResult = await itemService.SearchItemsAsync(
            TerritoryId,
            ItemType.Product,
            "mel",
            null,
            null,
            ItemStatus.Active,
            CancellationToken.None);

        Assert.True(listingsResult.IsSuccess);
        Assert.NotNull(listingsResult.Value);
        Assert.Single(listingsResult.Value!);

        var inquiry = await inquiryService.CreateInquiryAsync(service.Value!.Id, ResidentUserId, "Contato", null, CancellationToken.None);
        Assert.True(inquiry.IsSuccess);
        Assert.NotNull(inquiry.Value);
        Assert.NotNull(inquiry.Value.Contact);

        await cartService.AddItemAsync(TerritoryId, ResidentUserId, product.Value!.Id, 2, null, CancellationToken.None);
        await cartService.AddItemAsync(TerritoryId, ResidentUserId, service.Value!.Id, 1, null, CancellationToken.None);

        var checkout = await cartService.CheckoutAsync(TerritoryId, ResidentUserId, "mensagem", CancellationToken.None);
        Assert.True(checkout.IsSuccess);
        Assert.NotNull(checkout.Value);
        Assert.Single(checkout.Value!.Checkouts);
        Assert.Single(checkout.Value.Inquiries);

        var createdCheckout = checkout.Value.Checkouts[0].Checkout;
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
        var cache = CacheTestHelper.CreateDistributedCacheService();
        var (membershipAccessRules, accessEvaluator, _) = await CreateAccessAsync(
            dataStore,
            membershipRepository,
            userRepository,
            cache,
            TerritoryId,
            CancellationToken.None);
        var storeService = new StoreService(storeRepository, userRepository, accessEvaluator, membershipAccessRules, unitOfWork);

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

        Assert.True(createResult.IsSuccess);
        var store = createResult.Value!;

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

        Assert.True(updateResult.IsSuccess);
        Assert.Equal("Loja Atualizada", updateResult.Value!.DisplayName);
        Assert.Equal(StoreContactVisibility.OnInquiryOnly, updateResult.Value.ContactVisibility);

        var pauseResult = await storeService.SetStoreStatusAsync(store.Id, ResidentUserId, StoreStatus.Paused, CancellationToken.None);
        Assert.True(pauseResult.IsSuccess);
        Assert.Equal(StoreStatus.Paused, pauseResult.Value!.Status);

        var activateResult = await storeService.SetStoreStatusAsync(store.Id, ResidentUserId, StoreStatus.Active, CancellationToken.None);
        Assert.True(activateResult.IsSuccess);
        Assert.Equal(StoreStatus.Active, activateResult.Value!.Status);
    }

    [Fact]
    public async Task StoreService_GetMyStoreReturnsNullWhenNotExists()
    {
        var dataStore = new InMemoryDataStore();
        var storeRepository = new InMemoryStoreRepository(dataStore);
        var membershipRepository = new InMemoryTerritoryMembershipRepository(dataStore);
        var userRepository = new InMemoryUserRepository(dataStore);
        var unitOfWork = new InMemoryUnitOfWork();
        var cache = CacheTestHelper.CreateDistributedCacheService();
        var (membershipAccessRules, accessEvaluator, _) = await CreateAccessAsync(
            dataStore,
            membershipRepository,
            userRepository,
            cache,
            TerritoryId,
            CancellationToken.None);
        var storeService = new StoreService(storeRepository, userRepository, accessEvaluator, membershipAccessRules, unitOfWork);

        var store = await storeService.GetMyStoreAsync(TerritoryId, ResidentUserId, CancellationToken.None);
        Assert.Null(store);
    }

    [Fact]
    public async Task ListingService_UpdateAndArchive()
    {
        var dataStore = new InMemoryDataStore();
        var storeRepository = new InMemoryStoreRepository(dataStore);
        var itemRepository = new InMemoryStoreItemRepository(dataStore);
        var membershipRepository = new InMemoryTerritoryMembershipRepository(dataStore);
        var userRepository = new InMemoryUserRepository(dataStore);
        var unitOfWork = new InMemoryUnitOfWork();
        var cache = CacheTestHelper.CreateDistributedCacheService();
        var (membershipAccessRules, accessEvaluator, featureGuard) = await CreateAccessAsync(
            dataStore,
            membershipRepository,
            userRepository,
            cache,
            TerritoryId,
            CancellationToken.None);
        var storeService = new StoreService(storeRepository, userRepository, accessEvaluator, membershipAccessRules, unitOfWork);
        var itemService = new StoreItemService(itemRepository, storeRepository, userRepository, accessEvaluator, membershipAccessRules, featureGuard, unitOfWork);

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

        var createResult = await itemService.CreateItemAsync(
            TerritoryId,
            ResidentUserId,
            storeResult.Value!.Id,
            ItemType.Product,
            "Produto Original",
            "Descrição original",
            "Categoria",
            "tag",
            ItemPricingType.Fixed,
            10m,
            "BRL",
            "unidade",
            null,
            null,
            ItemStatus.Active,
            CancellationToken.None);

        Assert.True(createResult.IsSuccess);
        var item = createResult.Value!;

        var updateResult = await itemService.UpdateItemAsync(
            item.Id,
            ResidentUserId,
            ItemType.Product,
            "Produto Atualizado",
            "Nova descrição",
            "Nova categoria",
            "nova-tag",
            ItemPricingType.Fixed,
            15m,
            "BRL",
            "unidade",
            null,
            null,
            ItemStatus.Active,
            CancellationToken.None);

        Assert.True(updateResult.IsSuccess);
        Assert.Equal("Produto Atualizado", updateResult.Value!.Title);
        Assert.Equal(15m, updateResult.Value.PriceAmount);

        var archiveResult = await itemService.ArchiveItemAsync(item.Id, ResidentUserId, CancellationToken.None);
        Assert.True(archiveResult.IsSuccess);
        Assert.Equal(ItemStatus.Archived, archiveResult.Value!.Status);
    }

    [Fact]
    public async Task ListingService_SearchFiltersWork()
    {
        var dataStore = new InMemoryDataStore();
        var storeRepository = new InMemoryStoreRepository(dataStore);
        var itemRepository = new InMemoryStoreItemRepository(dataStore);
        var membershipRepository = new InMemoryTerritoryMembershipRepository(dataStore);
        var userRepository = new InMemoryUserRepository(dataStore);
        var unitOfWork = new InMemoryUnitOfWork();
        var cache = CacheTestHelper.CreateDistributedCacheService();
        var (membershipAccessRules, accessEvaluator, featureGuard) = await CreateAccessAsync(
            dataStore,
            membershipRepository,
            userRepository,
            cache,
            TerritoryId,
            CancellationToken.None);
        var storeService = new StoreService(storeRepository, userRepository, accessEvaluator, membershipAccessRules, unitOfWork);
        var itemService = new StoreItemService(itemRepository, storeRepository, userRepository, accessEvaluator, membershipAccessRules, featureGuard, unitOfWork);

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

        await itemService.CreateItemAsync(
            TerritoryId,
            ResidentUserId,
            storeResult.Value!.Id,
            ItemType.Product,
            "Mel",
            "Mel local",
            "Alimentos",
            "mel,doce",
            ItemPricingType.Fixed,
            30m,
            "BRL",
            "unidade",
            null,
            null,
            ItemStatus.Active,
            CancellationToken.None);

        await itemService.CreateItemAsync(
            TerritoryId,
            ResidentUserId,
            storeResult.Value!.Id,
            ItemType.Service,
            "Aula",
            "Aula de música",
            "Educação",
            "aula,musica",
            ItemPricingType.Negotiable,
            null,
            "BRL",
            "hora",
            null,
            null,
            ItemStatus.Active,
            CancellationToken.None);

        var productResults = await itemService.SearchItemsAsync(
            TerritoryId,
            ItemType.Product,
            "mel",
            null,
            null,
            ItemStatus.Active,
            CancellationToken.None);

        Assert.True(productResults.IsSuccess);
        Assert.NotNull(productResults.Value);
        Assert.Single(productResults.Value!);
        Assert.Equal(ItemType.Product, productResults.Value![0].Type);

        var categoryResults = await itemService.SearchItemsAsync(
            TerritoryId,
            null,
            null,
            "Educação",
            null,
            ItemStatus.Active,
            CancellationToken.None);

        Assert.True(categoryResults.IsSuccess);
        Assert.NotNull(categoryResults.Value);
        Assert.Single(categoryResults.Value!);
        Assert.Equal("Educação", categoryResults.Value![0].Category);
    }

    [Fact]
    public async Task CartService_AddUpdateRemoveItems()
    {
        var dataStore = new InMemoryDataStore();
        var storeRepository = new InMemoryStoreRepository(dataStore);
        var itemRepository = new InMemoryStoreItemRepository(dataStore);
        var cartRepository = new InMemoryCartRepository(dataStore);
        var cartItemRepository = new InMemoryCartItemRepository(dataStore);
        var checkoutRepository = new InMemoryCheckoutRepository(dataStore);
        var checkoutItemRepository = new InMemoryCheckoutItemRepository(dataStore);
        var inquiryRepository = new InMemoryInquiryRepository(dataStore);
        var feeRepository = new InMemoryPlatformFeeConfigRepository(dataStore);
        var membershipRepository = new InMemoryTerritoryMembershipRepository(dataStore);
        var userRepository = new InMemoryUserRepository(dataStore);
        var unitOfWork = new InMemoryUnitOfWork();
        var cache = CacheTestHelper.CreateDistributedCacheService();
        var (membershipAccessRules, accessEvaluator, featureGuard) = await CreateAccessAsync(
            dataStore,
            membershipRepository,
            userRepository,
            cache,
            TerritoryId,
            CancellationToken.None);
        var storeService = new StoreService(storeRepository, userRepository, accessEvaluator, membershipAccessRules, unitOfWork);
        var itemService = new StoreItemService(itemRepository, storeRepository, userRepository, accessEvaluator, membershipAccessRules, featureGuard, unitOfWork);
        var cartService = new CartService(
            cartRepository,
            cartItemRepository,
            itemRepository,
            storeRepository,
            checkoutRepository,
            checkoutItemRepository,
            inquiryRepository,
            feeRepository,
            featureGuard,
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

        var listingResult = await itemService.CreateItemAsync(
            TerritoryId,
            ResidentUserId,
            storeResult.Value!.Id,
            ItemType.Product,
            "Produto",
            null,
            null,
            null,
            ItemPricingType.Fixed,
            10m,
            "BRL",
            "unidade",
            null,
            null,
            ItemStatus.Active,
            CancellationToken.None);

        var addResult = await cartService.AddItemAsync(
            TerritoryId,
            ResidentUserId,
            listingResult.Value!.Id,
            2,
            "Nota inicial",
            CancellationToken.None);

        Assert.True(addResult.IsSuccess);
        Assert.NotNull(addResult.Value);
        Assert.Equal(2, addResult.Value.Quantity);

        var cartResult = await cartService.GetCartAsync(TerritoryId, ResidentUserId, CancellationToken.None);
        Assert.True(cartResult.IsSuccess);
        Assert.NotNull(cartResult.Value);
        Assert.Single(cartResult.Value!.Items);

        var updateResult = await cartService.UpdateItemAsync(
            addResult.Value!.Id,
            ResidentUserId,
            3,
            "Nota atualizada",
            CancellationToken.None);

        Assert.True(updateResult.IsSuccess);
        Assert.Equal(3, updateResult.Value!.Quantity);
        Assert.Equal("Nota atualizada", updateResult.Value.Notes);

        var removeResult = await cartService.RemoveItemAsync(addResult.Value!.Id, ResidentUserId, CancellationToken.None);
        Assert.True(removeResult);

        var emptyCartResult = await cartService.GetCartAsync(TerritoryId, ResidentUserId, CancellationToken.None);
        Assert.True(emptyCartResult.IsSuccess);
        Assert.NotNull(emptyCartResult.Value);
        Assert.Empty(emptyCartResult.Value!.Items);
    }

    [Fact]
    public async Task InquiryService_ListMyAndReceivedInquiries()
    {
        var dataStore = new InMemoryDataStore();
        var inquiryRepository = new InMemoryInquiryRepository(dataStore);
        var itemRepository = new InMemoryStoreItemRepository(dataStore);
        var storeRepository = new InMemoryStoreRepository(dataStore);
        var membershipRepository = new InMemoryTerritoryMembershipRepository(dataStore);
        var userRepository = new InMemoryUserRepository(dataStore);
        var unitOfWork = new InMemoryUnitOfWork();
        var cache = CacheTestHelper.CreateDistributedCacheService();
        var (membershipAccessRules, accessEvaluator, featureGuard) = await CreateAccessAsync(
            dataStore,
            membershipRepository,
            userRepository,
            cache,
            TerritoryId,
            CancellationToken.None);
        var storeService = new StoreService(storeRepository, userRepository, accessEvaluator, membershipAccessRules, unitOfWork);
        var itemService = new StoreItemService(itemRepository, storeRepository, userRepository, accessEvaluator, membershipAccessRules, featureGuard, unitOfWork);
        var inquiryService = new InquiryService(inquiryRepository, itemRepository, storeRepository, featureGuard, unitOfWork);

        var buyerId = Guid.NewGuid();
        await userRepository.AddAsync(
            new User(buyerId, "Comprador", "comprador@araponga.com", "123.456.789-00", null, null, null, "google", "buyer-ext", DateTime.UtcNow),
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

        var listingResult = await itemService.CreateItemAsync(
            TerritoryId,
            ResidentUserId,
            storeResult.Value!.Id,
            ItemType.Product,
            "Produto",
            null,
            null,
            null,
            ItemPricingType.Fixed,
            10m,
            "BRL",
            "unidade",
            null,
            null,
            ItemStatus.Active,
            CancellationToken.None);

        var inquiryResult = await inquiryService.CreateInquiryAsync(
            listingResult.Value!.Id,
            buyerId,
            "Quero comprar",
            null,
            CancellationToken.None);

        Assert.True(inquiryResult.IsSuccess);

        var myInquiries = await inquiryService.ListMyInquiriesAsync(buyerId, CancellationToken.None);
        Assert.Single(myInquiries);
        Assert.Equal(buyerId, myInquiries[0].FromUserId);

        var receivedInquiries = await inquiryService.ListReceivedInquiriesAsync(ResidentUserId, CancellationToken.None);
        Assert.Single(receivedInquiries);
        Assert.Equal(storeResult.Value!.Id, receivedInquiries[0].StoreId);
    }
}
