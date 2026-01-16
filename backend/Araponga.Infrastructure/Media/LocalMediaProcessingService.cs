using Araponga.Application.Interfaces.Media;
using Araponga.Infrastructure.Media;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace Araponga.Infrastructure.Media;

/// <summary>
/// Serviço de processamento de mídias usando ImageSharp para imagens.
/// </summary>
public sealed class LocalMediaProcessingService : IMediaProcessingService
{
    private readonly MediaStorageOptions _options;

    public LocalMediaProcessingService(IOptions<MediaStorageOptions> options)
    {
        _options = options.Value;
    }

    public async Task<Stream> ResizeImageAsync(Stream stream, int maxWidth, int maxHeight, CancellationToken cancellationToken = default)
    {
        if (stream == null || !stream.CanRead)
        {
            throw new ArgumentException("Stream inválido.", nameof(stream));
        }

        var memoryStream = new MemoryStream();
        
        try
        {
            using var image = await Image.LoadAsync(stream, cancellationToken);
            
            // Calcular novas dimensões mantendo aspect ratio
            var (newWidth, newHeight) = CalculateDimensions(image.Width, image.Height, maxWidth, maxHeight);

            // Redimensionar se necessário
            if (newWidth < image.Width || newHeight < image.Height)
            {
                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Size = new Size(newWidth, newHeight),
                    Mode = ResizeMode.Max
                }));
            }

            // Salvar imagem redimensionada
            var encoder = GetEncoderForImage(image);
            await image.SaveAsync(memoryStream, encoder, cancellationToken);
            
            memoryStream.Position = 0;
            return memoryStream;
        }
        catch
        {
            memoryStream.Dispose();
            throw;
        }
    }

    public async Task<Stream> OptimizeImageAsync(Stream stream, string mimeType, CancellationToken cancellationToken = default)
    {
        if (stream == null || !stream.CanRead)
        {
            throw new ArgumentException("Stream inválido.", nameof(stream));
        }

        var memoryStream = new MemoryStream();

        try
        {
            using var image = await Image.LoadAsync(stream, cancellationToken);

            // Redimensionar se exceder limites automáticos
            if (image.Width > _options.AutoResizeMaxWidthPx || image.Height > _options.AutoResizeMaxHeightPx)
            {
                var (newWidth, newHeight) = CalculateDimensions(
                    image.Width,
                    image.Height,
                    _options.AutoResizeMaxWidthPx,
                    _options.AutoResizeMaxHeightPx);

                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Size = new Size(newWidth, newHeight),
                    Mode = ResizeMode.Max
                }));
            }

            // Salvar com otimização baseada no tipo MIME
            var encoder = GetEncoderForMimeType(mimeType);
            await image.SaveAsync(memoryStream, encoder, cancellationToken);

            memoryStream.Position = 0;
            return memoryStream;
        }
        catch
        {
            memoryStream.Dispose();
            throw;
        }
    }

    public async Task<(int Width, int Height)?> GetImageDimensionsAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        if (stream == null || !stream.CanRead)
        {
            return null;
        }

        try
        {
            using var image = await Image.LoadAsync(stream, cancellationToken);
            return (image.Width, image.Height);
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> ValidateImageAsync(Stream stream, string mimeType, CancellationToken cancellationToken = default)
    {
        if (stream == null || !stream.CanRead)
        {
            return false;
        }

        try
        {
            // Tentar carregar a imagem
            using var image = await Image.LoadAsync(stream, cancellationToken);

            // Validar dimensões
            if (image.Width > _options.MaxImageWidthPx || image.Height > _options.MaxImageHeightPx)
            {
                return false;
            }

            return true;
        }
        catch
        {
            return false;
        }
    }

    private static (int Width, int Height) CalculateDimensions(int originalWidth, int originalHeight, int maxWidth, int maxHeight)
    {
        if (originalWidth <= maxWidth && originalHeight <= maxHeight)
        {
            return (originalWidth, originalHeight);
        }

        var ratioX = (double)maxWidth / originalWidth;
        var ratioY = (double)maxHeight / originalHeight;
        var ratio = Math.Min(ratioX, ratioY);

        return ((int)(originalWidth * ratio), (int)(originalHeight * ratio));
    }

    private static IImageEncoder GetEncoderForImage(Image image)
    {
        return image.Configuration.ImageFormatsManager.FindEncoderByMimeType(image.Metadata.DecodedImageFormat!.DefaultMimeType)
               ?? new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder();
    }

    private static IImageEncoder GetEncoderForMimeType(string mimeType)
    {
        return mimeType.ToLowerInvariant() switch
        {
            "image/jpeg" or "image/jpg" => new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder
            {
                Quality = 85 // Compressão moderada
            },
            "image/png" => new SixLabors.ImageSharp.Formats.Png.PngEncoder
            {
                CompressionLevel = SixLabors.ImageSharp.Formats.Png.PngCompressionLevel.BestCompression
            },
            "image/webp" => new SixLabors.ImageSharp.Formats.Webp.WebpEncoder
            {
                Quality = 85
            },
            _ => new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder
            {
                Quality = 85
            }
        };
    }
}