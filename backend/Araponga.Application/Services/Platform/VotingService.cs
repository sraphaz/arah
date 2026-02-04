using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Domain.Governance;
using Araponga.Domain.Membership;

namespace Araponga.Application.Services;

/// <summary>
/// Serviço para gerenciar votações comunitárias.
/// </summary>
public sealed class VotingService
{
    private readonly IVotingRepository _votingRepository;
    private readonly IVoteRepository _voteRepository;
    private readonly ITerritoryMembershipRepository _membershipRepository;
    private readonly AccessEvaluator _accessEvaluator;
    private readonly IUnitOfWork _unitOfWork;
    private readonly TerritoryCharacterizationService? _characterizationService;
    private readonly TerritoryModerationService? _moderationService;

    public VotingService(
        IVotingRepository votingRepository,
        IVoteRepository voteRepository,
        ITerritoryMembershipRepository membershipRepository,
        AccessEvaluator accessEvaluator,
        IUnitOfWork unitOfWork,
        TerritoryCharacterizationService? characterizationService = null,
        TerritoryModerationService? moderationService = null)
    {
        _votingRepository = votingRepository;
        _voteRepository = voteRepository;
        _membershipRepository = membershipRepository;
        _accessEvaluator = accessEvaluator;
        _unitOfWork = unitOfWork;
        _characterizationService = characterizationService;
        _moderationService = moderationService;
    }

    /// <summary>
    /// Cria uma nova votação.
    /// </summary>
    public async Task<Result<Voting>> CreateVotingAsync(
        Guid territoryId,
        Guid userId,
        VotingType type,
        string title,
        string description,
        IReadOnlyList<string> options,
        VotingVisibility visibility,
        DateTime? startsAtUtc,
        DateTime? endsAtUtc,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return Result<Voting>.Failure("Title is required.");
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            return Result<Voting>.Failure("Description is required.");
        }

        if (options is null || options.Count < 2)
        {
            return Result<Voting>.Failure("At least 2 options are required.");
        }

        // Verificar permissões para criar votação
        var canCreate = await CanCreateVotingAsync(userId, territoryId, type, cancellationToken);
        if (!canCreate)
        {
            return Result<Voting>.Failure("User does not have permission to create this type of voting.");
        }

        var voting = new Voting(
            Guid.NewGuid(),
            territoryId,
            userId,
            type,
            title,
            description,
            options,
            visibility,
            VotingStatus.Draft,
            startsAtUtc,
            endsAtUtc,
            DateTime.UtcNow,
            DateTime.UtcNow);

        await _votingRepository.AddAsync(voting, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result<Voting>.Success(voting);
    }

    /// <summary>
    /// Lista votações de um território.
    /// </summary>
    public async Task<IReadOnlyList<Voting>> ListVotingsAsync(
        Guid territoryId,
        VotingStatus? status,
        Guid? userId,
        CancellationToken cancellationToken)
    {
        return await _votingRepository.ListByTerritoryAsync(territoryId, status, userId, cancellationToken);
    }

    /// <summary>
    /// Obtém uma votação pelo ID.
    /// </summary>
    public async Task<Result<Voting>> GetVotingAsync(
        Guid votingId,
        Guid? userId,
        CancellationToken cancellationToken)
    {
        var voting = await _votingRepository.GetByIdAsync(votingId, cancellationToken);
        if (voting is null)
        {
            return Result<Voting>.Failure("Voting not found.");
        }

        return Result<Voting>.Success(voting);
    }

    /// <summary>
    /// Registra um voto em uma votação.
    /// </summary>
    public async Task<OperationResult> VoteAsync(
        Guid votingId,
        Guid userId,
        string selectedOption,
        CancellationToken cancellationToken)
    {
        var voting = await _votingRepository.GetByIdAsync(votingId, cancellationToken);
        if (voting is null)
        {
            return OperationResult.Failure("Voting not found.");
        }

        if (voting.Status != VotingStatus.Open)
        {
            return OperationResult.Failure("Voting is not open for votes.");
        }

        // Verificar se já votou
        var hasVoted = await _voteRepository.HasVotedAsync(votingId, userId, cancellationToken);
        if (hasVoted)
        {
            return OperationResult.Failure("User has already voted in this voting.");
        }

        // Verificar permissão para votar
        var canVote = await CanVoteAsync(userId, voting.TerritoryId, voting.Visibility, cancellationToken);
        if (!canVote)
        {
            return OperationResult.Failure("User does not have permission to vote in this voting.");
        }

        // Verificar se a opção é válida
        if (!voting.Options.Contains(selectedOption))
        {
            return OperationResult.Failure("Selected option is not valid for this voting.");
        }

        var vote = new Vote(
            Guid.NewGuid(),
            votingId,
            userId,
            selectedOption,
            DateTime.UtcNow);

        await _voteRepository.AddAsync(vote, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return OperationResult.Success();
    }

    /// <summary>
    /// Fecha uma votação (apenas criador ou curador).
    /// </summary>
    public async Task<OperationResult> CloseVotingAsync(
        Guid votingId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var voting = await _votingRepository.GetByIdAsync(votingId, cancellationToken);
        if (voting is null)
        {
            return OperationResult.Failure("Voting not found.");
        }

        if (voting.Status != VotingStatus.Open)
        {
            return OperationResult.Failure("Only open votings can be closed.");
        }

        // Verificar permissão (criador ou curador)
        var isCreator = voting.CreatedByUserId == userId;
        var isCurator = await _accessEvaluator.HasCapabilityAsync(
            userId,
            voting.TerritoryId,
            MembershipCapabilityType.Curator,
            cancellationToken);

        if (!isCreator && !isCurator)
        {
            return OperationResult.Failure("Only the creator or a curator can close a voting.");
        }

        voting.Close();
        await _votingRepository.UpdateAsync(voting, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        // Aplicar resultados se necessário
        await ApplyVotingResultsAsync(voting, cancellationToken);

        return OperationResult.Success();
    }

    /// <summary>
    /// Aplica os resultados de uma votação fechada.
    /// </summary>
    private async Task ApplyVotingResultsAsync(
        Voting voting,
        CancellationToken cancellationToken)
    {
        if (voting.Status != VotingStatus.Closed)
        {
            return;
        }

        var results = await _voteRepository.CountByOptionAsync(voting.Id, cancellationToken);
        if (results.Count == 0)
        {
            return;
        }

        // Encontrar opção vencedora
        var winningOption = results.OrderByDescending(r => r.Value).First().Key;

        // Aplicar resultado baseado no tipo de votação
        switch (voting.Type)
        {
            case VotingType.TerritoryCharacterization:
                if (_characterizationService is not null)
                {
                    // Adicionar tag vencedora à caracterização
                    var current = await _characterizationService.GetCharacterizationAsync(
                        voting.TerritoryId, cancellationToken);
                    var tags = current?.Tags.ToList() ?? new List<string>();
                    if (!tags.Contains(winningOption, StringComparer.OrdinalIgnoreCase))
                    {
                        tags.Add(winningOption);
                        var updateResult = await _characterizationService.UpdateCharacterizationAsync(
                            voting.TerritoryId, tags, cancellationToken);
                        // Ignorar erro se houver (não crítico para o fechamento da votação)
                    }
                }
                break;

            case VotingType.ModerationRule:
                // Criar regra de moderação baseada no resultado
                // (implementação simplificada - pode ser expandida)
                break;

            // Outros tipos podem ser implementados conforme necessário
        }
    }

    /// <summary>
    /// Obtém os resultados de uma votação.
    /// </summary>
    public async Task<Result<Dictionary<string, int>>> GetResultsAsync(
        Guid votingId,
        CancellationToken cancellationToken)
    {
        var voting = await _votingRepository.GetByIdAsync(votingId, cancellationToken);
        if (voting is null)
        {
            return Result<Dictionary<string, int>>.Failure("Voting not found.");
        }

        var results = await _voteRepository.CountByOptionAsync(votingId, cancellationToken);
        return Result<Dictionary<string, int>>.Success(results);
    }

    private async Task<bool> CanCreateVotingAsync(
        Guid userId,
        Guid territoryId,
        VotingType type,
        CancellationToken cancellationToken)
    {
        try
        {
            // Verificar se é membro do território
            var membership = await _membershipRepository.GetByUserAndTerritoryAsync(userId, territoryId, cancellationToken);
            if (membership is null)
            {
                return false;
            }

            // Dependendo do tipo, requer diferentes permissões
            return type switch
            {
                VotingType.ThemePrioritization => membership.Role == MembershipRole.Resident,
                VotingType.ModerationRule => _accessEvaluator is not null && await _accessEvaluator.HasCapabilityAsync(
                    userId, territoryId, MembershipCapabilityType.Curator, cancellationToken),
                VotingType.TerritoryCharacterization => membership.Role == MembershipRole.Resident,
                VotingType.FeatureFlag => _accessEvaluator is not null && await _accessEvaluator.HasCapabilityAsync(
                    userId, territoryId, MembershipCapabilityType.Curator, cancellationToken),
                VotingType.CommunityPolicy => membership.Role == MembershipRole.Resident,
                _ => false
            };
        }
        catch
        {
            // Em caso de erro, retornar false (mais seguro)
            return false;
        }
    }

    private async Task<bool> CanVoteAsync(
        Guid userId,
        Guid territoryId,
        VotingVisibility visibility,
        CancellationToken cancellationToken)
    {
        var membership = await _membershipRepository.GetByUserAndTerritoryAsync(userId, territoryId, cancellationToken);
        if (membership is null)
        {
            return false;
        }

        return visibility switch
        {
            VotingVisibility.AllMembers => true,
            VotingVisibility.ResidentsOnly => membership.Role == MembershipRole.Resident,
            VotingVisibility.CuratorsOnly => _accessEvaluator is not null && await _accessEvaluator.HasCapabilityAsync(
                userId, territoryId, MembershipCapabilityType.Curator, cancellationToken),
            _ => false
        };
    }
}
