namespace Araponga.Application.Interfaces.Media;

/// <summary>
/// Serviço responsável pelo armazenamento de arquivos de mídia (local, S3, Azure Blob, etc.).
/// </summary>
public interface IMediaStorageService
{
    /// <summary>
    /// Faz upload de um arquivo para o sistema de armazenamento.
    /// </summary>
    /// <param name="stream">Stream do arquivo a ser armazenado.</param>
    /// <param name="mimeType">Tipo MIME do arquivo (ex: "image/jpeg").</param>
    /// <param name="fileName">Nome original do arquivo (usado para determinar extensão).</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Chave de armazenamento (storage key) do arquivo armazenado.</returns>
    Task<string> UploadAsync(Stream stream, string mimeType, string fileName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Faz download de um arquivo do sistema de armazenamento.
    /// </summary>
    /// <param name="storageKey">Chave de armazenamento do arquivo.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Stream do arquivo.</returns>
    Task<Stream> DownloadAsync(string storageKey, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deleta um arquivo do sistema de armazenamento.
    /// </summary>
    /// <param name="storageKey">Chave de armazenamento do arquivo.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    Task DeleteAsync(string storageKey, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém a URL pública ou assinada para acesso ao arquivo.
    /// </summary>
    /// <param name="storageKey">Chave de armazenamento do arquivo.</param>
    /// <param name="expiresIn">Tempo de expiração da URL (para URLs assinadas).</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>URL pública ou assinada do arquivo.</returns>
    Task<string> GetUrlAsync(string storageKey, TimeSpan? expiresIn = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifica se um arquivo existe no sistema de armazenamento.
    /// </summary>
    /// <param name="storageKey">Chave de armazenamento do arquivo.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>True se o arquivo existe, false caso contrário.</returns>
    Task<bool> ExistsAsync(string storageKey, CancellationToken cancellationToken = default);
}