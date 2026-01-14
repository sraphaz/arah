using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Domain.Marketplace;

namespace Araponga.Application.Services;

public sealed class StoreService
{
    private readonly IStoreRepository _storeRepository;
    private readonly IUserRepository _userRepository;
    private readonly AccessEvaluator _accessEvaluator;
    private readonly IUnitOfWork _unitOfWork;

    public StoreService(
        IStoreRepository storeRepository,
        IUserRepository userRepository,
        AccessEvaluator accessEvaluator,
        IUnitOfWork unitOfWork)
    {
        _storeRepository = storeRepository;
        _userRepository = userRepository;
        _accessEvaluator = accessEvaluator;
        _unitOfWork = unitOfWork;
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
        if (!await IsResidentOrCuratorAsync(userId, territoryId, cancellationToken))
        {
            return Result<Store>.Failure("Only confirmed residents or admins can manage stores.");
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
        return Result<Store>.Success(store);
    }

    private async Task<bool> CanManageStoreAsync(Store store, Guid userId, CancellationToken cancellationToken)
    {
        if (store.OwnerUserId == userId)
        {
            return true;
        }

        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        return user is not null && _accessEvaluator.IsCurator(user);
    }

    private async Task<bool> IsResidentOrCuratorAsync(Guid userId, Guid territoryId, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user is not null && _accessEvaluator.IsCurator(user))
        {
            return true;
        }

        return await _accessEvaluator.IsResidentAsync(userId, territoryId, cancellationToken);
    }
}
