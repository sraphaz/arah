using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Domain.Policies;

namespace Araponga.Application.Services;

/// <summary>
/// Serviço para gerenciar aceites de Políticas de Privacidade.
/// </summary>
public sealed class PrivacyPolicyAcceptanceService
{
    private readonly IPrivacyPolicyAcceptanceRepository _acceptanceRepository;
    private readonly IPrivacyPolicyRepository _policyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PrivacyPolicyAcceptanceService(
        IPrivacyPolicyAcceptanceRepository acceptanceRepository,
        IPrivacyPolicyRepository policyRepository,
        IUnitOfWork unitOfWork)
    {
        _acceptanceRepository = acceptanceRepository;
        _policyRepository = policyRepository;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Aceita política de privacidade.
    /// </summary>
    public async Task<Result<PrivacyPolicyAcceptance>> AcceptPolicyAsync(
        Guid userId,
        Guid policyId,
        string? ipAddress,
        string? userAgent,
        CancellationToken cancellationToken)
    {
        var policy = await _policyRepository.GetByIdAsync(policyId, cancellationToken);
        if (policy is null)
        {
            return Result<PrivacyPolicyAcceptance>.Failure("Privacy Policy not found.");
        }

        if (!policy.IsActive)
        {
            return Result<PrivacyPolicyAcceptance>.Failure("Privacy Policy is not active.");
        }

        var now = DateTime.UtcNow;
        if (policy.EffectiveDate > now)
        {
            return Result<PrivacyPolicyAcceptance>.Failure("Privacy Policy is not yet effective.");
        }

        if (policy.ExpirationDate.HasValue && policy.ExpirationDate.Value <= now)
        {
            return Result<PrivacyPolicyAcceptance>.Failure("Privacy Policy has expired.");
        }

        // Verificar se já existe um aceite não revogado
        var existing = await _acceptanceRepository.GetByUserAndPolicyAsync(userId, policyId, cancellationToken);
        if (existing is not null && !existing.IsRevoked)
        {
            // Se já aceitou a mesma versão, retornar sucesso
            if (existing.AcceptedVersion == policy.Version)
            {
                return Result<PrivacyPolicyAcceptance>.Success(existing);
            }

            // Se a versão mudou, criar novo aceite (versão antiga fica como histórico)
        }

        var acceptance = new PrivacyPolicyAcceptance(
            Guid.NewGuid(),
            userId,
            policyId,
            now,
            policy.Version,
            ipAddress,
            userAgent);

        await _acceptanceRepository.AddAsync(acceptance, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result<PrivacyPolicyAcceptance>.Success(acceptance);
    }

    /// <summary>
    /// Verifica se o usuário aceitou a política específica.
    /// </summary>
    public Task<bool> HasAcceptedPolicyAsync(Guid userId, Guid policyId, CancellationToken cancellationToken)
    {
        return _acceptanceRepository.HasAcceptedAsync(userId, policyId, cancellationToken);
    }

    /// <summary>
    /// Verifica se o usuário aceitou todas as políticas obrigatórias.
    /// </summary>
    public async Task<Result<bool>> HasAcceptedRequiredPoliciesAsync(
        Guid userId,
        IReadOnlyList<PrivacyPolicy> requiredPolicies,
        CancellationToken cancellationToken)
    {
        foreach (var policy in requiredPolicies)
        {
            var hasAccepted = await _acceptanceRepository.HasAcceptedAsync(userId, policy.Id, cancellationToken);
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
    public Task<IReadOnlyList<PrivacyPolicyAcceptance>> GetAcceptanceHistoryAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        return _acceptanceRepository.GetByUserIdAsync(userId, cancellationToken);
    }

    /// <summary>
    /// Revoga aceite de política (opcional).
    /// </summary>
    public async Task<Result<PrivacyPolicyAcceptance>> RevokeAcceptanceAsync(
        Guid userId,
        Guid policyId,
        CancellationToken cancellationToken)
    {
        var acceptance = await _acceptanceRepository.GetByUserAndPolicyAsync(userId, policyId, cancellationToken);
        if (acceptance is null)
        {
            return Result<PrivacyPolicyAcceptance>.Failure("Acceptance not found.");
        }

        if (acceptance.IsRevoked)
        {
            return Result<PrivacyPolicyAcceptance>.Failure("Acceptance is already revoked.");
        }

        acceptance.Revoke(DateTime.UtcNow);
        await _acceptanceRepository.UpdateAsync(acceptance, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result<PrivacyPolicyAcceptance>.Success(acceptance);
    }
}
