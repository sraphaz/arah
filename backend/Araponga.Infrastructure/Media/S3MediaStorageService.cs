using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Araponga.Application.Interfaces.Media;
using Araponga.Infrastructure.Media;
using Microsoft.Extensions.Options;
using System.Text;

namespace Araponga.Infrastructure.Media;

/// <summary>
/// Serviço de armazenamento de mídias no Amazon S3.
/// </summary>
public sealed class S3MediaStorageService : IMediaStorageService, IDisposable
{
    private readonly MediaStorageOptions _options;
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName;

    public S3MediaStorageService(IOptions<MediaStorageOptions> options)
    {
        _options = options.Value;

        if (string.IsNullOrWhiteSpace(_options.S3BucketName))
        {
            throw new InvalidOperationException("S3BucketName é obrigatório na configuração.");
        }

        _bucketName = _options.S3BucketName;

        // Configurar cliente S3
        var config = new AmazonS3Config
        {
            RegionEndpoint = !string.IsNullOrWhiteSpace(_options.S3Region)
                ? RegionEndpoint.GetBySystemName(_options.S3Region)
                : RegionEndpoint.USEast1
        };

        // Criar credenciais se fornecidas
        if (!string.IsNullOrWhiteSpace(_options.S3AccessKeyId) && !string.IsNullOrWhiteSpace(_options.S3SecretAccessKey))
        {
            _s3Client = new AmazonS3Client(_options.S3AccessKeyId, _options.S3SecretAccessKey, config);
        }
        else
        {
            // Usar credenciais padrão do ambiente (IAM role, etc.)
            _s3Client = new AmazonS3Client(config);
        }
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

        // Gerar storage key (caminho no S3)
        var storageKey = GenerateStorageKey(mimeType, fileName);

        var request = new PutObjectRequest
        {
            BucketName = _bucketName,
            Key = storageKey,
            InputStream = stream,
            ContentType = mimeType,
            ServerSideEncryptionMethod = ServerSideEncryptionMethod.AES256,
            CannedACL = S3CannedACL.Private // URLs assinadas são usadas para acesso
        };

        await _s3Client.PutObjectAsync(request, cancellationToken);

        return storageKey;
    }

    public async Task<Stream> DownloadAsync(string storageKey, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(storageKey))
        {
            throw new ArgumentException("Storage key é obrigatório.", nameof(storageKey));
        }

        var request = new GetObjectRequest
        {
            BucketName = _bucketName,
            Key = storageKey
        };

        var response = await _s3Client.GetObjectAsync(request, cancellationToken);

        // Retornar stream do S3 (pode ser lido diretamente)
        var memoryStream = new MemoryStream();
        await response.ResponseStream.CopyToAsync(memoryStream, cancellationToken);
        memoryStream.Position = 0;

        return memoryStream;
    }

    public async Task DeleteAsync(string storageKey, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(storageKey))
        {
            throw new ArgumentException("Storage key é obrigatório.", nameof(storageKey));
        }

        var request = new DeleteObjectRequest
        {
            BucketName = _bucketName,
            Key = storageKey
        };

        await _s3Client.DeleteObjectAsync(request, cancellationToken);
    }

    public Task<string> GetUrlAsync(string storageKey, TimeSpan? expiresIn = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(storageKey))
        {
            throw new ArgumentException("Storage key é obrigatório.", nameof(storageKey));
        }

        // Gerar URL pré-assinada
        var request = new GetPreSignedUrlRequest
        {
            BucketName = _bucketName,
            Key = storageKey,
            Expires = DateTime.UtcNow.Add(expiresIn ?? TimeSpan.FromHours(24)),
            Verb = HttpVerb.GET
        };

        var url = _s3Client.GetPreSignedURL(request);
        return Task.FromResult(url);
    }

    public async Task<bool> ExistsAsync(string storageKey, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(storageKey))
        {
            return false;
        }

        try
        {
            var request = new GetObjectMetadataRequest
            {
                BucketName = _bucketName,
                Key = storageKey
            };

            await _s3Client.GetObjectMetadataAsync(request, cancellationToken);
            return true;
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return false;
        }
    }

    private string GenerateStorageKey(string mimeType, string fileName)
    {
        // Determinar subdiretório baseado no tipo MIME
        var subdirectory = GetSubdirectoryForMimeType(mimeType);
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        var guid = Guid.NewGuid().ToString("N");

        // Adicionar prefixo se configurado
        var prefix = !string.IsNullOrWhiteSpace(_options.S3Prefix)
            ? _options.S3Prefix.TrimEnd('/') + "/"
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

    public void Dispose()
    {
        _s3Client?.Dispose();
    }
}