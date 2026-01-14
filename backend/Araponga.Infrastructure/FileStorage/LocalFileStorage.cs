using System.Security.Cryptography;
using Araponga.Application.Interfaces;
using Araponga.Domain.Evidence;

namespace Araponga.Infrastructure.FileStorage;

public sealed class LocalFileStorage : IFileStorage
{
    private readonly string _rootPath;

    public LocalFileStorage(string rootPath)
    {
        _rootPath = rootPath;
    }

    public StorageProvider Provider => StorageProvider.Local;

    public async Task<string> SaveAsync(
        Stream content,
        string fileName,
        string contentType,
        CancellationToken cancellationToken)
    {
        Directory.CreateDirectory(_rootPath);

        var safeName = string.IsNullOrWhiteSpace(fileName) ? "upload.bin" : Path.GetFileName(fileName);
        var key = $"{DateTime.UtcNow:yyyyMMdd}/{Guid.NewGuid():N}-{safeName}";
        var fullPath = Path.Combine(_rootPath, key.Replace('/', Path.DirectorySeparatorChar));

        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);

        await using var output = File.Create(fullPath);
        await content.CopyToAsync(output, cancellationToken);

        return key;
    }

    public Task<Stream> OpenReadAsync(string storageKey, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(storageKey))
        {
            throw new ArgumentException("storageKey is required.", nameof(storageKey));
        }

        var fullPath = Path.Combine(_rootPath, storageKey.Replace('/', Path.DirectorySeparatorChar));
        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException("File not found for storageKey.", fullPath);
        }

        Stream stream = File.Open(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        return Task.FromResult(stream);
    }
}

