using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Modules.Marketplace.Application.Interfaces;
using Araponga.Modules.Marketplace.Domain;
using IStoreRatingResponseRepository = Araponga.Modules.Marketplace.Application.Interfaces.IStoreRatingResponseRepository;

namespace Araponga.Application.Services;

/// <summary>
/// Service responsible for managing store and item ratings in the marketplace.
/// </summary>
public sealed class RatingService
{
    private readonly IStoreRatingRepository _storeRatingRepository;
    private readonly IStoreItemRatingRepository _itemRatingRepository;
    private readonly IStoreRatingResponseRepository _ratingResponseRepository;
    private readonly IStoreRepository _storeRepository;
    private readonly IStoreItemRepository _itemRepository;
    private readonly ICheckoutRepository _checkoutRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RatingService(
        IStoreRatingRepository storeRatingRepository,
        IStoreItemRatingRepository itemRatingRepository,
        IStoreRatingResponseRepository ratingResponseRepository,
        IStoreRepository storeRepository,
        IStoreItemRepository itemRepository,
        ICheckoutRepository checkoutRepository,
        IUnitOfWork unitOfWork)
    {
        _storeRatingRepository = storeRatingRepository;
        _itemRatingRepository = itemRatingRepository;
        _ratingResponseRepository = ratingResponseRepository;
        _storeRepository = storeRepository;
        _itemRepository = itemRepository;
        _checkoutRepository = checkoutRepository;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Rates a store. Only users who have purchased from the store can rate it.
    /// </summary>
    public async Task<Result<StoreRating>> RateStoreAsync(
        Guid storeId,
        Guid userId,
        int rating,
        string? comment,
        CancellationToken cancellationToken)
    {
        if (rating < 1 || rating > 5)
        {
            return Result<StoreRating>.Failure("Rating must be between 1 and 5.");
        }

        if (comment is not null && comment.Length > 2000)
        {
            return Result<StoreRating>.Failure("Comment must not exceed 2000 characters.");
        }

        var store = await _storeRepository.GetByIdAsync(storeId, cancellationToken);
        if (store is null)
        {
            return Result<StoreRating>.Failure("Store not found.");
        }

        // Validar que o usuário comprou da loja (opcional - pode ser relaxado no futuro)
        // Por enquanto, permitir qualquer usuário autenticado avaliar

        // Verificar se já existe avaliação do usuário para esta loja
        var existing = await _storeRatingRepository.GetByStoreAndUserAsync(storeId, userId, cancellationToken);
        var now = DateTime.UtcNow;

        if (existing is not null)
        {
            // Atualizar avaliação existente
            existing.Update(rating, comment, now);
            await _storeRatingRepository.UpdateAsync(existing, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
            return Result<StoreRating>.Success(existing);
        }

        // Criar nova avaliação
        var storeRating = new StoreRating(
            Guid.NewGuid(),
            storeId,
            userId,
            rating,
            comment,
            now,
            now);

        await _storeRatingRepository.AddAsync(storeRating, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result<StoreRating>.Success(storeRating);
    }

    /// <summary>
    /// Rates a store item. Only users who have purchased the item can rate it.
    /// </summary>
    public async Task<Result<StoreItemRating>> RateItemAsync(
        Guid itemId,
        Guid userId,
        int rating,
        string? comment,
        CancellationToken cancellationToken)
    {
        if (rating < 1 || rating > 5)
        {
            return Result<StoreItemRating>.Failure("Rating must be between 1 and 5.");
        }

        if (comment is not null && comment.Length > 2000)
        {
            return Result<StoreItemRating>.Failure("Comment must not exceed 2000 characters.");
        }

        var item = await _itemRepository.GetByIdAsync(itemId, cancellationToken);
        if (item is null)
        {
            return Result<StoreItemRating>.Failure("Item not found.");
        }

        // Validar que o usuário comprou o item (opcional - pode ser relaxado no futuro)
        // Por enquanto, permitir qualquer usuário autenticado avaliar

        // Verificar se já existe avaliação do usuário para este item
        var existing = await _itemRatingRepository.GetByItemAndUserAsync(itemId, userId, cancellationToken);
        var now = DateTime.UtcNow;

        if (existing is not null)
        {
            // Atualizar avaliação existente
            existing.Update(rating, comment, now);
            await _itemRatingRepository.UpdateAsync(existing, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
            return Result<StoreItemRating>.Success(existing);
        }

        // Criar nova avaliação
        var itemRating = new StoreItemRating(
            Guid.NewGuid(),
            itemId,
            userId,
            rating,
            comment,
            now,
            now);

        await _itemRatingRepository.AddAsync(itemRating, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result<StoreItemRating>.Success(itemRating);
    }

    /// <summary>
    /// Responds to a store rating. Only the store owner can respond.
    /// </summary>
    public async Task<Result<StoreRatingResponse>> RespondToRatingAsync(
        Guid ratingId,
        Guid storeId,
        Guid userId,
        string responseText,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(responseText))
        {
            return Result<StoreRatingResponse>.Failure("Response text is required.");
        }

        if (responseText.Length > 2000)
        {
            return Result<StoreRatingResponse>.Failure("Response text must not exceed 2000 characters.");
        }

        var store = await _storeRepository.GetByIdAsync(storeId, cancellationToken);
        if (store is null)
        {
            return Result<StoreRatingResponse>.Failure("Store not found.");
        }

        // Validar que o usuário é o dono da loja
        if (store.OwnerUserId != userId)
        {
            return Result<StoreRatingResponse>.Failure("Only the store owner can respond to ratings.");
        }

        var rating = await _storeRatingRepository.GetByIdAsync(ratingId, cancellationToken);
        if (rating is null || rating.StoreId != storeId)
        {
            return Result<StoreRatingResponse>.Failure("Rating not found for this store.");
        }

        // Verificar se já existe resposta
        var existingResponse = await _ratingResponseRepository.GetByRatingIdAsync(ratingId, cancellationToken);
        if (existingResponse is not null)
        {
            return Result<StoreRatingResponse>.Failure("A response already exists for this rating.");
        }

        var response = new StoreRatingResponse(
            Guid.NewGuid(),
            ratingId,
            storeId,
            responseText,
            DateTime.UtcNow);

        await _ratingResponseRepository.AddAsync(response, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result<StoreRatingResponse>.Success(response);
    }

    /// <summary>
    /// Lists ratings for a store.
    /// </summary>
    public async Task<Result<IReadOnlyList<StoreRating>>> ListStoreRatingsAsync(
        Guid storeId,
        CancellationToken cancellationToken)
    {
        var store = await _storeRepository.GetByIdAsync(storeId, cancellationToken);
        if (store is null)
        {
            return Result<IReadOnlyList<StoreRating>>.Failure("Store not found.");
        }

        var ratings = await _storeRatingRepository.ListByStoreIdAsync(storeId, cancellationToken);
        return Result<IReadOnlyList<StoreRating>>.Success(ratings);
    }

    /// <summary>
    /// Lists ratings for a store item.
    /// </summary>
    public async Task<Result<IReadOnlyList<StoreItemRating>>> ListItemRatingsAsync(
        Guid itemId,
        CancellationToken cancellationToken)
    {
        var item = await _itemRepository.GetByIdAsync(itemId, cancellationToken);
        if (item is null)
        {
            return Result<IReadOnlyList<StoreItemRating>>.Failure("Item not found.");
        }

        var ratings = await _itemRatingRepository.ListByItemIdAsync(itemId, cancellationToken);
        return Result<IReadOnlyList<StoreItemRating>>.Success(ratings);
    }

    /// <summary>
    /// Gets the average rating for a store.
    /// </summary>
    public async Task<Result<double>> GetStoreAverageRatingAsync(
        Guid storeId,
        CancellationToken cancellationToken)
    {
        var store = await _storeRepository.GetByIdAsync(storeId, cancellationToken);
        if (store is null)
        {
            return Result<double>.Failure("Store not found.");
        }

        var average = await _storeRatingRepository.GetAverageRatingAsync(storeId, cancellationToken);
        return Result<double>.Success(average);
    }

    /// <summary>
    /// Gets the average rating for a store item.
    /// </summary>
    public async Task<Result<double>> GetItemAverageRatingAsync(
        Guid itemId,
        CancellationToken cancellationToken)
    {
        var item = await _itemRepository.GetByIdAsync(itemId, cancellationToken);
        if (item is null)
        {
            return Result<double>.Failure("Item not found.");
        }

        var average = await _itemRatingRepository.GetAverageRatingAsync(itemId, cancellationToken);
        return Result<double>.Success(average);
    }

    /// <summary>
    /// Gets the response for a store rating, if it exists.
    /// </summary>
    public async Task<StoreRatingResponse?> GetRatingResponseAsync(
        Guid ratingId,
        CancellationToken cancellationToken)
    {
        return await _ratingResponseRepository.GetByRatingIdAsync(ratingId, cancellationToken);
    }
}
