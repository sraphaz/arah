using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Application.Models;

namespace Araponga.Application.Services;

/// <summary>
/// Serviço para consulta e gerenciamento de auditoria.
/// </summary>
public sealed class AuditService
{
    private readonly IAuditLogger _auditLogger;
    private readonly IAuditRepository? _auditRepository;

    public AuditService(IAuditLogger auditLogger, IAuditRepository? auditRepository = null)
    {
        _auditLogger = auditLogger;
        _auditRepository = auditRepository;
    }

    /// <summary>
    /// Registra uma entrada de auditoria.
    /// </summary>
    public Task LogAsync(
        string action,
        Guid actorUserId,
        Guid territoryId,
        Guid targetId,
        CancellationToken cancellationToken = default)
    {
        return _auditLogger.LogAsync(
            new AuditEntry(action, actorUserId, territoryId, targetId, DateTime.UtcNow),
            cancellationToken);
    }

    /// <summary>
    /// Lista entradas de auditoria com paginação.
    /// </summary>
    public async Task<PagedResult<AuditEntry>> ListAsync(
        Guid? territoryId = null,
        Guid? actorUserId = null,
        string? action = null,
        int pageNumber = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        if (_auditRepository is null)
        {
            // Se não há repositório de auditoria, retornar vazio
            return new PagedResult<AuditEntry>(Array.Empty<AuditEntry>(), pageNumber, pageSize, 0);
        }

        var pagination = new PaginationParameters(pageNumber, pageSize);
        var entries = await _auditRepository.ListAsync(
            territoryId,
            actorUserId,
            action,
            pagination.Skip,
            pagination.Take,
            cancellationToken);

        var totalCount = await _auditRepository.CountAsync(
            territoryId,
            actorUserId,
            action,
            cancellationToken);

        return new PagedResult<AuditEntry>(entries, pageNumber, pageSize, totalCount);
    }
}
