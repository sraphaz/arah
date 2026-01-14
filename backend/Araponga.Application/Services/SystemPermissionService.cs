using Araponga.Application.Common;
using Araponga.Application.Events;
using Araponga.Application.Interfaces;
using Araponga.Domain.Users;

namespace Araponga.Application.Services;

/// <summary>
/// Serviço para gerenciar SystemPermissions.
/// Publica eventos para invalidação automática de cache.
/// </summary>
public sealed class SystemPermissionService
{
    private readonly ISystemPermissionRepository _repository;
    private readonly IEventBus _eventBus;
    private readonly IUnitOfWork _unitOfWork;

    public SystemPermissionService(
        ISystemPermissionRepository repository,
        IEventBus eventBus,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _eventBus = eventBus;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Revoga uma SystemPermission e publica evento para invalidação de cache.
    /// </summary>
    public async Task<OperationResult> RevokeAsync(
        Guid permissionId,
        Guid revokedByUserId,
        CancellationToken cancellationToken)
    {
        var permission = await _repository.GetByIdAsync(permissionId, cancellationToken);
        if (permission is null)
        {
            return OperationResult.Failure("Permission not found.");
        }

        if (!permission.IsActive())
        {
            return OperationResult.Failure("Permission is already revoked.");
        }

        var revokedAtUtc = DateTime.UtcNow;
        permission.Revoke(revokedByUserId, revokedAtUtc);

        await _repository.UpdateAsync(permission, cancellationToken);

        // Publicar evento para invalidação de cache
        var revokedEvent = new SystemPermissionRevokedEvent(
            permission.UserId,
            permission.PermissionType,
            revokedAtUtc);

        await _eventBus.PublishAsync(revokedEvent, cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);

        return OperationResult.Success();
    }
}
