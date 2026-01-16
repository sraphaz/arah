namespace Araponga.Application.Interfaces.Media;

/// <summary>
/// Interface para enfileirar processamento assíncrono de mídias grandes.
/// </summary>
public interface IAsyncMediaProcessor
{
    /// <summary>
    /// Enfileira uma tarefa de processamento assíncrono de mídia.
    /// </summary>
    /// <param name="mediaAssetId">ID do MediaAsset a ser processado.</param>
    /// <param name="storageKey">Chave de armazenamento do arquivo.</param>
    /// <param name="mimeType">Tipo MIME do arquivo.</param>
    void EnqueueProcessing(Guid mediaAssetId, string storageKey, string mimeType);
}