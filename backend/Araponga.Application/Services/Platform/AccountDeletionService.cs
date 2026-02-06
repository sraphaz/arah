using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Domain.Users;

namespace Araponga.Application.Services;

/// <summary>
/// Serviço para exclusão de conta com anonimização de dados (LGPD).
/// </summary>
public sealed class AccountDeletionService
{
    private readonly IUserRepository _userRepository;
    private readonly ITerritoryMembershipRepository _membershipRepository;
    private readonly IFeedRepository _feedRepository;
    private readonly ITerritoryEventRepository _eventRepository;
    private readonly INotificationInboxRepository _notificationRepository;
    private readonly IUserPreferencesRepository _preferencesRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AccountDeletionService(
        IUserRepository userRepository,
        ITerritoryMembershipRepository membershipRepository,
        IFeedRepository feedRepository,
        ITerritoryEventRepository eventRepository,
        INotificationInboxRepository notificationRepository,
        IUserPreferencesRepository preferencesRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _membershipRepository = membershipRepository;
        _feedRepository = feedRepository;
        _notificationRepository = notificationRepository;
        _preferencesRepository = preferencesRepository;
        _feedRepository = feedRepository;
        _eventRepository = eventRepository;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Anonimiza dados pessoais do usuário (LGPD).
    /// Remove informações identificáveis, mantendo dados agregados para estatísticas.
    /// </summary>
    public async Task<Result<bool>> AnonymizeUserDataAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            return Result<bool>.Failure("User not found.");
        }

        // Anonimizar dados pessoais do usuário
        // Criar novo User com dados anonimizados
        // Nota: User requer CPF ou foreignDocument, então usamos um valor anonimizado
        var anonymizedUser = new User(
            user.Id,
            $"User_{user.Id.ToString("N")[..8]}", // DisplayName anonimizado
            null, // Email removido
            "000.000.000-00", // CPF anonimizado (valor fictício)
            null, // ForeignDocument removido
            null, // PhoneNumber removido
            null, // Address removido
            user.AuthProvider,
            $"anon_{user.Id}", // ExternalId anonimizado
            false, // 2FA desabilitado
            null, // TwoFactorSecret removido
            null, // TwoFactorRecoveryCodesHash removido
            null, // TwoFactorVerifiedAtUtc removido
            UserIdentityVerificationStatus.Unverified, // Verificação resetada
            null, // IdentityVerifiedAtUtc removido
            null, // AvatarMediaAssetId removido
            null, // Bio removido
            null, // PasswordHash removido
            user.CreatedAtUtc); // Manter data de criação para estatísticas

        await _userRepository.UpdateAsync(anonymizedUser, cancellationToken);

        // Remover preferências (dados pessoais)
        var preferences = await _preferencesRepository.GetByUserIdAsync(userId, cancellationToken);
        if (preferences is not null)
        {
            // Criar preferências padrão anonimizadas
            var defaultPreferences = UserPreferences.CreateDefault(userId, DateTime.UtcNow);
            await _preferencesRepository.UpdateAsync(defaultPreferences, cancellationToken);
        }

        // Nota: Posts, eventos e outras entidades mantêm o AuthorUserId para estatísticas,
        // mas os dados pessoais do User já foram anonimizados acima.
        // Se necessário, podemos adicionar lógica adicional para anonimizar conteúdo de posts/eventos.

        await _unitOfWork.CommitAsync(cancellationToken);

        return Result<bool>.Success(true);
    }

    /// <summary>
    /// Verifica se o usuário pode ser deletado (sem dependências críticas).
    /// </summary>
    public async Task<Result<bool>> CanDeleteUserAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            return Result<bool>.Failure("User not found.");
        }

        // Verificar se há dependências que impedem exclusão
        // Por exemplo: lojas ativas, transações pendentes, etc.
        // Por enquanto, permitir exclusão de qualquer usuário

        return Result<bool>.Success(true);
    }
}
