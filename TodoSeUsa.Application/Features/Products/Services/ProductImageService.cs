using TodoSeUsa.Application.Features.Products.Interfaces;

namespace TodoSeUsa.Application.Features.Products.Services;

public sealed class ProductImageService : IProductImageService
{
    private readonly IImageStorageService _storageService;
    private const string DefaultExtension = ".jpg";
    private const long MaxFileSize = 10 * 1024 * 1024; // 10 MB

    public ProductImageService(IImageStorageService storageService)
    {
        _storageService = storageService;
    }

    public Task<string> SaveAsync(int productId, Stream content, CancellationToken ct)
    {
        if (content.Length > MaxFileSize)
            throw new InvalidOperationException($"File exceeds max allowed size of {MaxFileSize} bytes.");

        var randomName = Path.GetRandomFileName();
        var fileName = Path.ChangeExtension(randomName, DefaultExtension);

        var relativePath = Path.Combine("ProductImages", productId.ToString(), fileName);
        return _storageService.SaveAsync(content, relativePath, ct);
    }

    public Task DeleteAsync(string publicPath, CancellationToken ct)
    {
        return _storageService.DeleteAsync(GetRelativePath(publicPath), ct);
    }

    private static string GetRelativePath(string publicPath)
    {
        return publicPath.Replace("/files/", "").Replace("/", Path.DirectorySeparatorChar.ToString());
    }
}