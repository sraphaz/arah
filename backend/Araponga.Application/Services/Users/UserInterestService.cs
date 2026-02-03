using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Domain.Users;

namespace Araponga.Application.Services;

/// <summary>
/// Serviço para gerenciar interesses de usuários.
/// </summary>
public sealed class UserInterestService
{
    private readonly IUserInterestRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private const int MaxInterestsPerUser = 10;

    public UserInterestService(
        IUserInterestRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Adiciona um interesse para um usuário.
    /// </summary>
    public async Task<Result<UserInterest>> AddInterestAsync(
        Guid userId,
        string interestTag,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(interestTag))
        {
            return Result<UserInterest>.Failure("Interest tag is required.");
        }

        var normalizedTag = interestTag.Trim().ToLowerInvariant();
        if (normalizedTag.Length > 50)
        {
            return Result<UserInterest>.Failure("Interest tag must not exceed 50 characters.");
        }

        // Verificar se já existe
        var exists = await _repository.ExistsAsync(userId, normalizedTag, cancellationToken);
        if (exists)
        {
            return Result<UserInterest>.Failure("Interest already exists for this user.");
        }

        // Verificar limite de interesses
        var currentCount = await _repository.CountByUserIdAsync(userId, cancellationToken);
        if (currentCount >= MaxInterestsPerUser)
        {
            return Result<UserInterest>.Failure($"Maximum of {MaxInterestsPerUser} interests allowed per user.");
        }

        var interest = new UserInterest(
            Guid.NewGuid(),
            userId,
            normalizedTag,
            DateTime.UtcNow);

        await _repository.AddAsync(interest, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result<UserInterest>.Success(interest);
    }

    /// <summary>
    /// Remove um interesse de um usuário.
    /// </summary>
    public async Task<OperationResult> RemoveInterestAsync(
        Guid userId,
        string interestTag,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(interestTag))
        {
            return OperationResult.Failure("Interest tag is required.");
        }

        var normalizedTag = interestTag.Trim().ToLowerInvariant();
        var exists = await _repository.ExistsAsync(userId, normalizedTag, cancellationToken);
        if (!exists)
        {
            return OperationResult.Failure("Interest does not exist for this user.");
        }

        await _repository.RemoveAsync(userId, normalizedTag, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return OperationResult.Success();
    }

    /// <summary>
    /// Lista todos os interesses de um usuário.
    /// </summary>
    public async Task<IReadOnlyList<UserInterest>> ListInterestsAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        return await _repository.ListByUserIdAsync(userId, cancellationToken);
    }

    /// <summary>
    /// Lista todos os usuários que têm um interesse específico em um território.
    /// </summary>
    public async Task<IReadOnlyList<Guid>> ListUsersByInterestAsync(
        string interestTag,
        Guid territoryId,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(interestTag))
        {
            return Array.Empty<Guid>();
        }

        var normalizedTag = interestTag.Trim().ToLowerInvariant();
        return await _repository.ListUserIdsByInterestAsync(normalizedTag, territoryId, cancellationToken);
    }
}
