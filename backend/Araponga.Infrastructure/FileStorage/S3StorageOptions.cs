namespace Araponga.Infrastructure.FileStorage;

public sealed class S3StorageOptions
{
    public string Bucket { get; init; } = "";
    public string Region { get; init; } = "us-east-1";

    /// <summary>
    /// Opcional: endpoint customizado (ex.: MinIO http://localhost:9000).
    /// Se vazio, usa AWS padrão.
    /// </summary>
    public string? ServiceUrl { get; init; }

    public bool ForcePathStyle { get; init; } = true;

    public string AccessKey { get; init; } = "";
    public string SecretKey { get; init; } = "";
}

