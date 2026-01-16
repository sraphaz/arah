using Araponga.Application.Interfaces.Media;

namespace Araponga.Infrastructure.Media;

/// <summary>
/// Implementação NoOp de IAsyncMediaProcessor (quando processamento assíncrono está desabilitado).
/// </summary>
public sealed class NoOpAsyncMediaProcessor : IAsyncMediaProcessor
{
    public void EnqueueProcessing(Guid mediaAssetId, string storageKey, string mimeType)
    {
        // NoOp - não faz nada
    }
}