namespace TodoSeUsa.Application.Common.Interfaces;

public interface IImageStorageService
{
    Task<string> SaveAsync(Stream content, string relativePath, CancellationToken ct);

    Task DeleteAsync(string relativePath, CancellationToken ct);
}