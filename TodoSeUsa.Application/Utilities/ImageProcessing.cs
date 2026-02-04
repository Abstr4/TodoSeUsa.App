using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace TodoSeUsa.Application.Utilities;

public static class ImageProcessing
{
    private const long _maxUploadSize = 10 * 1024 * 1024; // 10 MB
    private const int _maxSide = 1600;
    private const int _jpegQuality = 80;

    public static void ValidateSize(long length)
    {
        if (length == 0)
            throw new InvalidOperationException("La imagen subida está vacía.");

        if (length > _maxUploadSize)
        {
            var maxMb = _maxUploadSize / (1024 * 1024);
            var actualMb = length / (1024 * 1024);

            throw new InvalidOperationException(
                $"El tamaño de la imagen es {actualMb} MB, pero el tamaño máximo permitido es {maxMb} MB.");
        }
    }

    public static async Task<Image> LoadImageAsync(Stream source, CancellationToken ct)
    {
        if (!source.CanRead)
            throw new InvalidOperationException("El stream de la imagen no es legible.");

        return await Image.LoadAsync(source, ct);
    }

    public static void Resize(Image image)
    {
        if (image.Width <= _maxSide && image.Height <= _maxSide)
            return;

        image.Mutate(x => x.Resize(new ResizeOptions
        {
            Mode = ResizeMode.Max,
            Size = new Size(_maxSide, _maxSide)
        }));
    }

    public static async Task<byte[]> EncodeToJpegAsync(Image image, CancellationToken ct)
    {
        var encoder = new JpegEncoder { Quality = _jpegQuality };

        await using var ms = new MemoryStream();
        await image.SaveAsync(ms, encoder, ct);

        return ms.ToArray();
    }
}