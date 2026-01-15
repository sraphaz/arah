using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Application.Services;
using Araponga.Domain.Marketplace;
using Araponga.Domain.Membership;

namespace Araponga.Application.Services;

public sealed class StoreService
{
    private readonly IStoreRepository _storeRepository;
    private readonly IUserRepository _userRepository;
    private readonly AccessEvaluator _accessEvaluator;
    private readonly MembershipAccessRules _accessRules;
    private readonly IUnitOfWork _unitOfWork;
    private readonly CacheInvalidationService? _cacheInvalidation;

    public StoreService(
        IStoreRepository storeRepository,
        IUserRepository userRepository,
        AccessEvaluator accessEvaluator,
        MembershipAccessRules accessRules,
        IUnitOfWork unitOfWork,
        CacheInvalidationService? cacheInvalidation = null)
    {
        _storeRepository = storeRepository;
        _userRepository = userRepository;
        _accessEvaluator = accessEvaluator;
        _accessRules = accessRules;
        _unitOfWork = unitOfWork;
        _cacheInvalidation = cacheInvalidation;
    }

    public async Task<Result<Store>> UpsertMyStoreAsync(
        Guid territoryId,
        Guid userId,
        string displayName,
        string? description,
        StoreContactVisibility contactVisibility,
        string? phone,
        string? whatsapp,
        string? email,
        string? instagram,
        string? website,
        string? preferredContactMethod,
        CancellationToken cancellationToken)
    {
        // Verificar regras de marketplace
        if (!await _accessRules.CanCreateStoreOrItemInMarketplaceAsync(userId, territoryId, cancellationToken))
        {
            // Verificar se é Curator (pode gerenciar stores de outros)
            var isCurator = await _accessEvaluator.HasCapabilityAsync(userId, territoryId, MembershipCapabilityType.Curator, cancellationToken);
            if (!isCurator)
            {
                return Result<Store>.Failure("Only confirmed residents (marketplace enabled + opt-in) or curators can manage stores.");
            }
        }

        if (string.IsNullOrWhiteSpace(displayName))
        {
            return Result<Store>.Failure("Display name is required.");
        }

        var existing = await _storeRepository.GetByOwnerAsync(territoryId, userId, cancellationToken);
        var now = DateTime.UtcNow;

        if (existing is null)
        {
            var store = new Store(
                Guid.NewGuid(),
                territoryId,
                userId,
                displayName,
                description,
                StoreStatus.Active,
                false,
                contactVisibility,
                phone,
                whatsapp,
                email,
                instagram,
                website,
                preferredContactMethod,
                now,
                now);

            await _storeRepository.AddAsync(store, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
            
            // Invalidar cache de store
            _cacheInvalidation?.InvalidateStoreCache(territoryId, store.Id);
            
            return Result<Store>.Success(store);
        }

        existing.UpdateDetails(
            displayName,
            description,
            contactVisibility,
            phone,
            whatsapp,
            email,
            instagram,
            website,
            preferredContactMethod,
            now);

        await _storeRepository.UpdateAsync(existing, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        
        // Invalidar cache de store
        _cacheInvalidation?.InvalidateStoreCache(territoryId, existing.Id);
        
        return Result<Store>.Success(existing);
    }

    public Task<Store?> GetMyStoreAsync(Guid territoryId, Guid userId, CancellationToken cancellationToken)
    {
        return _storeRepository.GetByOwnerAsync(territoryId, userId, cancellationToken);
    }

    public async Task<Result<Store>> UpdateStoreAsync(
        Guid storeId,
        Guid userId,
        string? displayName,
        string? description,
        StoreContactVisibility? contactVisibility,
        string? phone,
        string? whatsapp,
        string? email,
        string? instagram,
        string? website,
        string? preferredContactMethod,
        CancellationToken cancellationToken)
    {
        var store = await _storeRepository.GetByIdAsync(storeId, cancellationToken);
        if (store is null)
        {
            return Result<Store>.Failure("Store not found.");
        }

        if (displayName is not null && string.IsNullOrWhiteSpace(displayName))
        {
            return Result<Store>.Failure("Display name is required.");
        }

        if (!await CanManageStoreAsync(store, userId, cancellationToken))
        {
            return Result<Store>.Failure("Not authorized.");
        }

        var now = DateTime.UtcNow;

        store.UpdateDetails(
            displayName ?? store.DisplayName,
            description ?? store.Description,
            contactVisibility ?? store.ContactVisibility,
            phone ?? store.Phone,
            whatsapp ?? store.Whatsapp,
            email ?? store.Email,
            instagram ?? store.Instagram,
            website ?? store.Website,
            preferredContactMethod ?? store.PreferredContactMethod,
            now);

        await _storeRepository.UpdateAsync(store, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        
        // Invalidar cache de store
        _cacheInvalidation?.InvalidateStoreCache(store.TerritoryId, store.Id);
        
        return Result<Store>.Success(store);
    }

    public async Task<Result<Store>> SetStoreStatusAsync(
        Guid storeId,
        Guid userId,
        StoreStatus status,
        CancellationToken cancellationToken)
    {
        var store = await _storeRepository.GetByIdAsync(storeId, cancellationToken);
        if (store is null)
        {
            return Result<Store>.Failure("Store not found.");
        }

        if (!await CanManageStoreAsync(store, userId, cancellationToken))
        {
            return Result<Store>.Failure("Not authorized.");
        }

        store.SetStatus(status, DateTime.UtcNow);
        await _storeRepository.UpdateAsync(store, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        
        // Invalidar cache de store
        _cacheInvalidation?.InvalidateStoreCache(store.TerritoryId, store.Id);
        
        return Result<Store>.Success(store);
    }

    public async Task<Result<Store>> SetPaymentsEnabledAsync(
        Guid storeId,
        Guid userId,
        bool enabled,
        CancellationToken cancellationToken)
    {
        var store = await _storeRepository.GetByIdAsync(storeId, cancellationToken);
        if (store is null)
        {
            return Result<Store>.Failure("Store not found.");
        }

        if (!await CanManageStoreAsync(store, userId, cancellationToken))
        {
            return Result<Store>.Failure("Not authorized.");
        }

        store.SetPaymentsEnabled(enabled, DateTime.UtcNow);
        await _storeRepository.UpdateAsync(store, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        
        // Invalidar cache de store
        _cacheInvalidation?.InvalidateStoreCache(store.TerritoryId, store.Id);
        
        return Result<Store>.Success(store);
    }

    private async Task<bool> CanManageStoreAsync(Store store, Guid userId, CancellationToken cancellationToken)
    {
        if (store.OwnerUserId == userId)
        {
            return true;
        }

        // Verificar se tem capacidade de Curator no território da Store
        return await _accessEvaluator.HasCapabilityAsync(
            userId,
            store.TerritoryId,
            MembershipCapabilityType.Curator,
            cancellationToken);
    }

    private async Task<bool> IsResidentOrCuratorAsync(Guid userId, Guid territoryId, CancellationToken cancellationToken)
    {
        // Verificar se tem capacidade de Curator
        if (await _accessEvaluator.HasCapabilityAsync(userId, territoryId, MembershipCapabilityType.Curator, cancellationToken))
        {
            return true;
        }

        return await _accessEvaluator.IsResidentAsync(userId, territoryId, cancellationToken);
    }
}
