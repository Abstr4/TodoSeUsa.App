using Microsoft.Extensions.Configuration;
using TodoSeUsa.Application.Common.Interfaces;

namespace TodoSeUsa.Infrastructure.FileSystem;

public sealed class ImageStorageService : IImageStorageService
{
    private readonly string _storageRoot;

    public ImageStorageService(IConfiguration configuration)
    {
        _storageRoot = configuration["Storage:BasePath"]!;
    }

    public async Task<string> SaveAsync(Stream content, string relativePath, CancellationToken ct)
    {
        var physicalPath = Path.Combine(_storageRoot, relativePath);

        Directory.CreateDirectory(Path.GetDirectoryName(physicalPath)!);

        if (content.CanSeek)
            content.Position = 0;

        await using var fileStream = new FileStream(
            physicalPath,
            FileMode.Create,
            FileAccess.Write,
            FileShare.None,
            81920,
            true
        );

        await content.CopyToAsync(fileStream, ct);

        return relativePath;
    }

    public Task DeleteAsync(string relativePath, CancellationToken ct)
    {
        var physicalPath = Path.Combine(_storageRoot, relativePath);

        if (File.Exists(physicalPath))
            File.Delete(physicalPath);

        return Task.CompletedTask;
    }
}