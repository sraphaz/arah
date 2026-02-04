using System.Security.Cryptography;
using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Application.Models;
using Araponga.Modules.Moderation.Application.Interfaces;
using Araponga.Modules.Moderation.Domain.Evidence;

namespace Araponga.Application.Services;

public sealed class DocumentEvidenceService
{
    private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg",
        "image/png",
        "application/pdf"
    };

    private const long DefaultMaxBytes = 5 * 1024 * 1024; // 5MB

    private readonly IFileStorage _storage;
    private readonly IDocumentEvidenceRepository _repository;
    private readonly IAuditLogger _auditLogger;
    private readonly IUnitOfWork _unitOfWork;

    public DocumentEvidenceService(
        IFileStorage storage,
        IDocumentEvidenceRepository repository,
        IAuditLogger auditLogger,
        IUnitOfWork unitOfWork)
    {
        _storage = storage;
        _repository = repository;
        _auditLogger = auditLogger;
        _unitOfWork = unitOfWork;
    }

    public async Task<OperationResult<DocumentEvidence>> CreateAsync(
        Guid userId,
        Guid? territoryId,
        DocumentEvidenceKind kind,
        string fileName,
        string contentType,
        Stream content,
        CancellationToken cancellationToken)
    {
        if (userId == Guid.Empty)
        {
            return OperationResult<DocumentEvidence>.Failure("userId is required.");
        }

        if (kind == DocumentEvidenceKind.Residency && (territoryId is null || territoryId.Value == Guid.Empty))
        {
            return OperationResult<DocumentEvidence>.Failure("territoryId is required for residency evidence.");
        }

        if (string.IsNullOrWhiteSpace(contentType) || !AllowedContentTypes.Contains(contentType))
        {
            return OperationResult<DocumentEvidence>.Failure("Unsupported content type.");
        }

        // Copiar para memória para calcular hash e tamanho antes de persistir
        await using var ms = new MemoryStream();
        await content.CopyToAsync(ms, cancellationToken);
        var bytes = ms.ToArray();
        if (bytes.LongLength <= 0)
        {
            return OperationResult<DocumentEvidence>.Failure("File is empty.");
        }

        if (bytes.LongLength > DefaultMaxBytes)
        {
            return OperationResult<DocumentEvidence>.Failure($"File too large (max {DefaultMaxBytes} bytes).");
        }

        var sha = Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();

        // Salvar no storage (local por enquanto)
        await using var uploadStream = new MemoryStream(bytes);
        var storageKey = await _storage.SaveAsync(uploadStream, fileName, contentType, cancellationToken);

        var now = DateTime.UtcNow;
        var evidence = new DocumentEvidence(
            Guid.NewGuid(),
            userId,
            territoryId,
            kind,
            _storage.Provider,
            storageKey,
            contentType,
            bytes.LongLength,
            sha,
            originalFileName: fileName,
            createdAtUtc: now);

        await _repository.AddAsync(evidence, cancellationToken);

        await _auditLogger.LogAsync(
            new AuditEntry("document_evidence.created", userId, territoryId ?? Guid.Empty, evidence.Id, now),
            cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);

        return OperationResult<DocumentEvidence>.Success(evidence);
    }
}

