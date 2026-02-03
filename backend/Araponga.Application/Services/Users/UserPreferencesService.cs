using Araponga.Application.Interfaces;
using Araponga.Domain.Users;

namespace Araponga.Application.Services;

public sealed class UserPreferencesService
{
    private readonly IUserPreferencesRepository _preferencesRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UserPreferencesService(
        IUserPreferencesRepository preferencesRepository,
        IUnitOfWork unitOfWork)
    {
        _preferencesRepository = preferencesRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<UserPreferences> GetPreferencesAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var preferences = await _preferencesRepository.GetOrCreateDefaultAsync(
            userId,
            cancellationToken);
        return preferences;
    }

    public async Task<UserPreferences> UpdatePrivacyPreferencesAsync(
        Guid userId,
        ProfileVisibility profileVisibility,
        ContactVisibility contactVisibility,
        bool shareLocation,
        bool showMemberships,
        CancellationToken cancellationToken)
    {
        var preferences = await _preferencesRepository.GetOrCreateDefaultAsync(
            userId,
            cancellationToken);

        preferences.UpdatePrivacy(
            profileVisibility,
            contactVisibility,
            shareLocation,
            showMemberships,
            DateTime.UtcNow);

        await _preferencesRepository.UpdateAsync(preferences, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return preferences;
    }

    public async Task<UserPreferences> UpdateNotificationPreferencesAsync(
        Guid userId,
        NotificationPreferences notificationPreferences,
        CancellationToken cancellationToken)
    {
        var preferences = await _preferencesRepository.GetOrCreateDefaultAsync(
            userId,
            cancellationToken);

        preferences.UpdateNotificationPreferences(
            notificationPreferences,
            DateTime.UtcNow);

        await _preferencesRepository.UpdateAsync(preferences, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return preferences;
    }

    public async Task<UserPreferences> UpdateEmailPreferencesAsync(
        Guid userId,
        EmailPreferences emailPreferences,
        CancellationToken cancellationToken)
    {
        var preferences = await _preferencesRepository.GetOrCreateDefaultAsync(
            userId,
            cancellationToken);

        preferences.UpdateEmailPreferences(
            emailPreferences,
            DateTime.UtcNow);

        await _preferencesRepository.UpdateAsync(preferences, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return preferences;
    }
}
