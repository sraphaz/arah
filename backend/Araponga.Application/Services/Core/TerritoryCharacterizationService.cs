using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Domain.Territories;

namespace Araponga.Application.Services;

/// <summary>
/// Serviço para gerenciar caracterização de territórios.
/// </summary>
public sealed class TerritoryCharacterizationService
{
    private readonly ITerritoryCharacterizationRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public TerritoryCharacterizationService(
        ITerritoryCharacterizationRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Atualiza a caracterização de um território.
    /// </summary>
    public async Task<Result<TerritoryCharacterization>> UpdateCharacterizationAsync(
        Guid territoryId,
        IReadOnlyList<string> tags,
        CancellationToken cancellationToken)
    {
        var existing = await _repository.GetByTerritoryIdAsync(territoryId, cancellationToken);

        TerritoryCharacterization characterization;
        if (existing is not null)
        {
            existing.UpdateTags(tags);
            characterization = existing;
        }
        else
        {
            characterization = new TerritoryCharacterization(
                territoryId,
                tags,
                DateTime.UtcNow);
        }

        await _repository.UpsertAsync(characterization, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result<TerritoryCharacterization>.Success(characterization);
    }

    /// <summary>
    /// Obtém a caracterização de um território.
    /// </summary>
    public async Task<TerritoryCharacterization?> GetCharacterizationAsync(
        Guid territoryId,
        CancellationToken cancellationToken)
    {
        return await _repository.GetByTerritoryIdAsync(territoryId, cancellationToken);
    }
}
