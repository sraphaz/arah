using Araponga.Application.Common;
using Araponga.Application.Events;
using Araponga.Application.Interfaces;
using Araponga.Application.Models;
using Araponga.Domain.Users;

namespace Araponga.Application.Services;

/// <summary>
/// Serviço para gerenciar SystemPermissions.
/// Publica eventos para invalidação automática de cache e registra auditoria.
/// </summary>
public sealed class SystemPermissionService
{
    private readonly ISystemPermissionRepository _repository;
    private readonly IEventBus _eventBus;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditLogger _auditLogger;

    public SystemPermissionService(
        ISystemPermissionRepository repository,
        IEventBus eventBus,
        IUnitOfWork unitOfWork,
        IAuditLogger auditLogger)
    {
        _repository = repository;
        _eventBus = eventBus;
        _unitOfWork = unitOfWork;
        _auditLogger = auditLogger;
    }

    /// <summary>
    /// Concede uma SystemPermission e registra auditoria.
    /// </summary>
    public async Task<OperationResult<SystemPermission>> GrantAsync(
        Guid userId,
        SystemPermissionType permissionType,
        Guid grantedByUserId,
        CancellationToken cancellationToken)
    {
        // Verificar se já existe permissão ativa
        var hasActivePermission = await _repository.HasActivePermissionAsync(userId, permissionType, cancellationToken);
        if (hasActivePermission)
        {
            return OperationResult<SystemPermission>.Failure("User already has this active permission.");
        }

        var grantedAtUtc = DateTime.UtcNow;
        var permission = new SystemPermission(
            Guid.NewGuid(),
            userId,
            permissionType,
            grantedAtUtc,
            grantedByUserId,
            revokedAtUtc: null,
            revokedByUserId: null);

        await _repository.AddAsync(permission, cancellationToken);

        // Registrar auditoria
        await _auditLogger.LogAsync(
            new AuditEntry(
                "system_permission.granted",
                grantedByUserId,
                Guid.Empty, // TerritoryId não aplicável para SystemPermission
                permission.Id,
                grantedAtUtc),
            cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);

        return OperationResult<SystemPermission>.Success(permission);
    }

    /// <summary>
    /// Revoga uma SystemPermission, publica evento para invalidação de cache e registra auditoria.
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

        // Registrar auditoria
        await _auditLogger.LogAsync(
            new AuditEntry(
                "system_permission.revoked",
                revokedByUserId,
                Guid.Empty, // TerritoryId não aplicável para SystemPermission
                permission.Id,
                revokedAtUtc),
            cancellationToken);

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
