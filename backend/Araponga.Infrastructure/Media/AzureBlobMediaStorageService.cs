using Araponga.Application.Interfaces.Media;
using Araponga.Infrastructure.Media;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Microsoft.Extensions.Options;
using System.Text;

namespace Araponga.Infrastructure.Media;

/// <summary>
/// Serviço de armazenamento de mídias no Azure Blob Storage.
/// </summary>
public sealed class AzureBlobMediaStorageService : IMediaStorageService
{
    private readonly MediaStorageOptions _options;
    private readonly BlobServiceClient _blobServiceClient;
    private readonly BlobContainerClient _containerClient;
    private readonly string _containerName;

    public AzureBlobMediaStorageService(IOptions<MediaStorageOptions> options)
    {
        _options = options.Value;

        if (string.IsNullOrWhiteSpace(_options.AzureBlobConnectionString))
        {
            throw new InvalidOperationException("AzureBlobConnectionString é obrigatório na configuração.");
        }

        if (string.IsNullOrWhiteSpace(_options.AzureBlobContainerName))
        {
            throw new InvalidOperationException("AzureBlobContainerName é obrigatório na configuração.");
        }

        _containerName = _options.AzureBlobContainerName;
        _blobServiceClient = new BlobServiceClient(_options.AzureBlobConnectionString);
        _containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);

        // Criar container se não existir
        _containerClient.CreateIfNotExists(PublicAccessType.None);
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

        // Gerar storage key (caminho no blob)
        var storageKey = GenerateStorageKey(mimeType, fileName);
        var blobClient = _containerClient.GetBlobClient(storageKey);

        var options = new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders
            {
                ContentType = mimeType
            }
        };

        await blobClient.UploadAsync(stream, options, cancellationToken);

        return storageKey;
    }

    public async Task<Stream> DownloadAsync(string storageKey, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(storageKey))
        {
            throw new ArgumentException("Storage key é obrigatório.", nameof(storageKey));
        }

        var blobClient = _containerClient.GetBlobClient(storageKey);

        if (!await blobClient.ExistsAsync(cancellationToken))
        {
            throw new FileNotFoundException($"Blob não encontrado: {storageKey}");
        }

        var response = await blobClient.DownloadStreamingAsync(cancellationToken: cancellationToken);

        // Copiar para MemoryStream para garantir que o stream seja retornável
        var memoryStream = new MemoryStream();
        await response.Value.Content.CopyToAsync(memoryStream, cancellationToken);
        memoryStream.Position = 0;

        return memoryStream;
    }

    public async Task DeleteAsync(string storageKey, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(storageKey))
        {
            throw new ArgumentException("Storage key é obrigatório.", nameof(storageKey));
        }

        var blobClient = _containerClient.GetBlobClient(storageKey);
        await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);
    }

    public Task<string> GetUrlAsync(string storageKey, TimeSpan? expiresIn = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(storageKey))
        {
            throw new ArgumentException("Storage key é obrigatório.", nameof(storageKey));
        }

        var blobClient = _containerClient.GetBlobClient(storageKey);

        // Gerar SAS (Shared Access Signature) URL
        if (blobClient.CanGenerateSasUri)
        {
            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = _containerName,
                BlobName = storageKey,
                Resource = "b", // Blob
                ExpiresOn = DateTimeOffset.UtcNow.Add(expiresIn ?? TimeSpan.FromHours(24))
            };

            sasBuilder.SetPermissions(BlobSasPermissions.Read);

            var sasUri = blobClient.GenerateSasUri(sasBuilder);
            return Task.FromResult(sasUri.ToString());
        }

        // Se não puder gerar SAS, retornar URL direta (requer acesso público ou configuração adicional)
        return Task.FromResult(blobClient.Uri.ToString());
    }

    public async Task<bool> ExistsAsync(string storageKey, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(storageKey))
        {
            return false;
        }

        var blobClient = _containerClient.GetBlobClient(storageKey);
        var response = await blobClient.ExistsAsync(cancellationToken);
        return response.Value;
    }

    private string GenerateStorageKey(string mimeType, string fileName)
    {
        // Determinar subdiretório baseado no tipo MIME
        var subdirectory = GetSubdirectoryForMimeType(mimeType);
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        var guid = Guid.NewGuid().ToString("N");

        // Adicionar prefixo se configurado
        var prefix = !string.IsNullOrWhiteSpace(_options.AzureBlobPrefix)
            ? _options.AzureBlobPrefix.TrimEnd('/') + "/"
            : "";

        // Estrutura: [prefixo]/[tipo]/[ano]/[mês]/[guid].[extensão]
        var year = DateTime.UtcNow.Year.ToString();
        var month = DateTime.UtcNow.Month.ToString("00");
        var storageKey = $"{prefix}{subdirectory}/{year}/{month}/{guid}{extension}";

        return storageKey;
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
}