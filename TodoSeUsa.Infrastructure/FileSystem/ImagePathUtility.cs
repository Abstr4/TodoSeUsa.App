namespace TodoSeUsa.Infrastructure.FileSystem;

public static class ImagePathUtility
{
    public static string GetProductFolder(string basePath, int productId)
    {
        return Path.Combine(basePath, productId.ToString());
    }

    public static string GenerateFileName(string originalFileName)
    {
        var extension = Path.GetExtension(originalFileName);
        return $"{Guid.NewGuid()}{extension}";
    }

    public static string GetPublicPath(int productId, string fileName)
    {
        return $"/files/{productId}/{fileName}";
    }
}