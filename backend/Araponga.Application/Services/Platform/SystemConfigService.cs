using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Application.Models;
using Araponga.Domain.Configuration;

namespace Araponga.Application.Services;

public sealed class SystemConfigService
{
    private readonly ISystemConfigRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditLogger _auditLogger;
    private readonly SystemConfigCacheService? _cacheService;

    public SystemConfigService(
        ISystemConfigRepository repository,
        IUnitOfWork unitOfWork,
        IAuditLogger auditLogger,
        SystemConfigCacheService? cacheService = null)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _auditLogger = auditLogger;
        _cacheService = cacheService;
    }

    public async Task<SystemConfig?> GetByKeyAsync(string key, CancellationToken cancellationToken)
    {
        if (_cacheService is not null)
        {
            return await _cacheService.GetByKeyAsync(key, cancellationToken);
        }

        var normalizedKey = NormalizeKey(key);
        if (string.IsNullOrWhiteSpace(normalizedKey))
        {
            return null;
        }

        return await _repository.GetByKeyAsync(normalizedKey, cancellationToken);
    }

    public async Task<IReadOnlyList<SystemConfig>> ListAsync(
        SystemConfigCategory? category,
        CancellationToken cancellationToken)
    {
        return await _repository.ListAsync(category, cancellationToken);
    }

    public async Task<OperationResult<SystemConfig>> UpsertAsync(
        string key,
        string value,
        SystemConfigCategory category,
        string? description,
        Guid actorUserId,
        CancellationToken cancellationToken)
    {
        try
        {
            var normalizedKey = NormalizeKey(key);
            if (string.IsNullOrWhiteSpace(normalizedKey))
            {
                return OperationResult<SystemConfig>.Failure("Key is required.");
            }

            var existing = await _repository.GetByKeyAsync(normalizedKey, cancellationToken);
            var now = DateTime.UtcNow;

            SystemConfig config;
            if (existing is null)
            {
                config = new SystemConfig(
                    Guid.NewGuid(),
                    normalizedKey,
                    value,
                    category,
                    description,
                    createdAtUtc: now,
                    createdByUserId: actorUserId,
                    updatedAtUtc: null,
                    updatedByUserId: null);
            }
            else
            {
                existing.Update(value, category, description, actorUserId, now);
                config = existing;
            }

            await _repository.UpsertAsync(config, cancellationToken);

            await _auditLogger.LogAsync(
                new AuditEntry(
                    existing is null ? "system_config.created" : "system_config.updated",
                    actorUserId,
                    Guid.Empty,
                    config.Id,
                    now),
                cancellationToken);

            await _unitOfWork.CommitAsync(cancellationToken);

            _cacheService?.Invalidate(normalizedKey);

            return OperationResult<SystemConfig>.Success(config);
        }
        catch (Exception ex)
        {
            return OperationResult<SystemConfig>.Failure(ex.Message);
        }
    }

    private static string NormalizeKey(string key)
        => string.IsNullOrWhiteSpace(key) ? string.Empty : key.Trim().ToLowerInvariant();
}

