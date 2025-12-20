using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using TodoSeUsa.Application.Common.Interfaces;

namespace TodoSeUsa.Infrastructure.FileSystem;
public sealed class ImageStorageService : IImageStorageService
{
    private readonly string _basePath;
    private const string DefaultExtension = ".jpg";

    public ImageStorageService(IConfiguration configuration)
    {
        _basePath = configuration["Storage:BasePath"]!;
    }

    public async Task<string> SaveAsync(byte[] fileBytes, int ownerId, CancellationToken ct)
    {
        var folder = ImagePathUtility.GetProductFolder(_basePath, ownerId);
        Directory.CreateDirectory(folder);

        var fileName = $"{Guid.NewGuid()}{DefaultExtension}";
        var physicalPath = Path.Combine(folder, fileName);

        await File.WriteAllBytesAsync(physicalPath, fileBytes, ct);

        return ImagePathUtility.GetPublicPath(ownerId, fileName);
    }

    public async Task<string> SaveAsync(IFormFile file, int ownerId, CancellationToken ct)
    {
        var folder = ImagePathUtility.GetProductFolder(_basePath, ownerId);
        Directory.CreateDirectory(folder);

        var extension = Path.GetExtension(file.FileName);
        var fileName = $"{Guid.NewGuid()}{extension}";
        var physicalPath = Path.Combine(folder, fileName);

        await using var stream = new FileStream(physicalPath, FileMode.Create);
        await file.CopyToAsync(stream, ct);

        return ImagePathUtility.GetPublicPath(ownerId, fileName);
    }

    public Task DeleteAsync(string publicPath, CancellationToken ct)
    {
        var relative = publicPath.Replace("/files/", string.Empty)
                                 .Replace("/", Path.DirectorySeparatorChar.ToString());

        var physicalPath = Path.Combine(_basePath, relative);

        if (File.Exists(physicalPath))
            File.Delete(physicalPath);

        return Task.CompletedTask;
    }
}
