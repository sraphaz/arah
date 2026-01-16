using Araponga.Application.Interfaces.Media;
using Araponga.Infrastructure.Media;
using Microsoft.Extensions.Options;

namespace Araponga.Infrastructure.Media;

/// <summary>
/// Serviço de armazenamento local de mídias (armazenamento em disco).
/// </summary>
public sealed class LocalMediaStorageService : IMediaStorageService
{
    private readonly MediaStorageOptions _options;
    private readonly string _basePath;

    public LocalMediaStorageService(IOptions<MediaStorageOptions> options)
    {
        _options = options.Value;
        _basePath = Path.IsPathRooted(_options.LocalPath)
            ? _options.LocalPath
            : Path.Combine(AppContext.BaseDirectory, _options.LocalPath);

        // Criar diretórios se não existirem
        EnsureDirectoryExists(_basePath);
    }

    public async Task<string> UploadAsync(Stream stream, string mimeType, string fileName, CancellationToken cancellationToken = default)
    {
        if (stream == null || !stream.CanRead)
        {
            throw new ArgumentException("Stream inválido.", nameof(stream));
        }

        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentException("Nome do arquivo é obrigatório.", nameof(fileName));
        }

        // Determinar subdiretório baseado no tipo MIME
        var subdirectory = GetSubdirectoryForMimeType(mimeType);
        var directory = Path.Combine(_basePath, subdirectory, DateTime.UtcNow.Year.ToString(), DateTime.UtcNow.Month.ToString("00"));
        EnsureDirectoryExists(directory);

        // Gerar nome único (GUID + extensão original)
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        var storageKey = Path.Combine(subdirectory, DateTime.UtcNow.Year.ToString(), DateTime.UtcNow.Month.ToString("00"), $"{Guid.NewGuid()}{extension}");
        var fullPath = Path.Combine(_basePath, storageKey);

        // Garantir que o diretório existe
        var fileDirectory = Path.GetDirectoryName(fullPath);
        if (!string.IsNullOrEmpty(fileDirectory))
        {
            EnsureDirectoryExists(fileDirectory);
        }

        // Salvar arquivo
        using var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, useAsync: true);
        await stream.CopyToAsync(fileStream, cancellationToken);
        await fileStream.FlushAsync(cancellationToken);

        // Retornar storage key (relativo ao basePath)
        return storageKey.Replace(Path.DirectorySeparatorChar, '/');
    }

    public Task<Stream> DownloadAsync(string storageKey, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(storageKey))
        {
            throw new ArgumentException("Storage key é obrigatório.", nameof(storageKey));
        }

        // Normalizar storage key (remover barras no início e converter para Path separators)
        var normalizedKey = storageKey.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
        var fullPath = Path.Combine(_basePath, normalizedKey);

        // Validação de segurança: garantir que o caminho está dentro do basePath
        var canonicalPath = Path.GetFullPath(fullPath);
        var canonicalBasePath = Path.GetFullPath(_basePath);
        if (!canonicalPath.StartsWith(canonicalBasePath, StringComparison.OrdinalIgnoreCase))
        {
            throw new UnauthorizedAccessException("Acesso ao arquivo não autorizado.");
        }

        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException($"Arquivo não encontrado: {storageKey}");
        }

        return Task.FromResult<Stream>(new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read));
    }

    public Task DeleteAsync(string storageKey, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(storageKey))
        {
            throw new ArgumentException("Storage key é obrigatório.", nameof(storageKey));
        }

        var normalizedKey = storageKey.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
        var fullPath = Path.Combine(_basePath, normalizedKey);

        // Validação de segurança
        var canonicalPath = Path.GetFullPath(fullPath);
        var canonicalBasePath = Path.GetFullPath(_basePath);
        if (!canonicalPath.StartsWith(canonicalBasePath, StringComparison.OrdinalIgnoreCase))
        {
            throw new UnauthorizedAccessException("Acesso ao arquivo não autorizado.");
        }

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }

        return Task.CompletedTask;
    }

    public Task<string> GetUrlAsync(string storageKey, TimeSpan? expiresIn = null, CancellationToken cancellationToken = default)
    {
        // Para armazenamento local, retornar URL relativa
        // Em produção, isso seria mapeado para uma rota estática ou CDN
        var normalizedKey = storageKey.TrimStart('/');
        var url = $"/media/{normalizedKey}";
        return Task.FromResult(url);
    }

    public Task<bool> ExistsAsync(string storageKey, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(storageKey))
        {
            return Task.FromResult(false);
        }

        var normalizedKey = storageKey.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
        var fullPath = Path.Combine(_basePath, normalizedKey);

        // Validação de segurança
        var canonicalPath = Path.GetFullPath(fullPath);
        var canonicalBasePath = Path.GetFullPath(_basePath);
        if (!canonicalPath.StartsWith(canonicalBasePath, StringComparison.OrdinalIgnoreCase))
        {
            return Task.FromResult(false);
        }

        return Task.FromResult(File.Exists(fullPath));
    }

    private static string GetSubdirectoryForMimeType(string mimeType)
    {
        if (mimeType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
        {
            return "images";
        }

        if (mimeType.StartsWith("video/", StringComparison.OrdinalIgnoreCase))
        {
            return "videos";
        }

        if (mimeType.StartsWith("audio/", StringComparison.OrdinalIgnoreCase))
        {
            return "audio";
        }

        return "documents";
    }

    private static void EnsureDirectoryExists(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }
}