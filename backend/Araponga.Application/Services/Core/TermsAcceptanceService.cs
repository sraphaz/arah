using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Domain.Policies;

namespace Araponga.Application.Services;

/// <summary>
/// Serviço para gerenciar aceites de Termos de Uso.
/// </summary>
public sealed class TermsAcceptanceService
{
    private readonly ITermsAcceptanceRepository _acceptanceRepository;
    private readonly ITermsOfServiceRepository _termsRepository;
    private readonly IUnitOfWork _unitOfWork;

    public TermsAcceptanceService(
        ITermsAcceptanceRepository acceptanceRepository,
        ITermsOfServiceRepository termsRepository,
        IUnitOfWork unitOfWork)
    {
        _acceptanceRepository = acceptanceRepository;
        _termsRepository = termsRepository;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Aceita termos de serviço.
    /// </summary>
    public async Task<Result<TermsAcceptance>> AcceptTermsAsync(
        Guid userId,
        Guid termsId,
        string? ipAddress,
        string? userAgent,
        CancellationToken cancellationToken)
    {
        var terms = await _termsRepository.GetByIdAsync(termsId, cancellationToken);
        if (terms is null)
        {
            return Result<TermsAcceptance>.Failure("Terms of Service not found.");
        }

        if (!terms.IsActive)
        {
            return Result<TermsAcceptance>.Failure("Terms of Service is not active.");
        }

        var now = DateTime.UtcNow;
        if (terms.EffectiveDate > now)
        {
            return Result<TermsAcceptance>.Failure("Terms of Service is not yet effective.");
        }

        if (terms.ExpirationDate.HasValue && terms.ExpirationDate.Value <= now)
        {
            return Result<TermsAcceptance>.Failure("Terms of Service has expired.");
        }

        // Verificar se já existe um aceite não revogado
        var existing = await _acceptanceRepository.GetByUserAndTermsAsync(userId, termsId, cancellationToken);
        if (existing is not null && !existing.IsRevoked)
        {
            // Se já aceitou a mesma versão, retornar sucesso
            if (existing.AcceptedVersion == terms.Version)
            {
                return Result<TermsAcceptance>.Success(existing);
            }

            // Se a versão mudou, criar novo aceite (versão antiga fica como histórico)
        }

        var acceptance = new TermsAcceptance(
            Guid.NewGuid(),
            userId,
            termsId,
            now,
            terms.Version,
            ipAddress,
            userAgent);

        await _acceptanceRepository.AddAsync(acceptance, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result<TermsAcceptance>.Success(acceptance);
    }

    /// <summary>
    /// Verifica se o usuário aceitou os termos específicos.
    /// </summary>
    public Task<bool> HasAcceptedTermsAsync(Guid userId, Guid termsId, CancellationToken cancellationToken)
    {
        return _acceptanceRepository.HasAcceptedAsync(userId, termsId, cancellationToken);
    }

    /// <summary>
    /// Verifica se o usuário aceitou todos os termos obrigatórios.
    /// </summary>
    public async Task<Result<bool>> HasAcceptedRequiredTermsAsync(
        Guid userId,
        IReadOnlyList<TermsOfService> requiredTerms,
        CancellationToken cancellationToken)
    {
        foreach (var terms in requiredTerms)
        {
            var hasAccepted = await _acceptanceRepository.HasAcceptedAsync(userId, terms.Id, cancellationToken);
            if (!hasAccepted)
            {
                return Result<bool>.Success(false);
            }
        }

        return Result<bool>.Success(true);
    }

    /// <summary>
    /// Obtém histórico de aceites do usuário.
    /// </summary>
    public Task<IReadOnlyList<TermsAcceptance>> GetAcceptanceHistoryAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        return _acceptanceRepository.GetByUserIdAsync(userId, cancellationToken);
    }

    /// <summary>
    /// Revoga aceite de termos (opcional).
    /// </summary>
    public async Task<Result<TermsAcceptance>> RevokeAcceptanceAsync(
        Guid userId,
        Guid termsId,
        CancellationToken cancellationToken)
    {
        var acceptance = await _acceptanceRepository.GetByUserAndTermsAsync(userId, termsId, cancellationToken);
        if (acceptance is null)
        {
            return Result<TermsAcceptance>.Failure("Acceptance not found.");
        }

        if (acceptance.IsRevoked)
        {
            return Result<TermsAcceptance>.Failure("Acceptance is already revoked.");
        }

        acceptance.Revoke(DateTime.UtcNow);
        await _acceptanceRepository.UpdateAsync(acceptance, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result<TermsAcceptance>.Success(acceptance);
    }
}
