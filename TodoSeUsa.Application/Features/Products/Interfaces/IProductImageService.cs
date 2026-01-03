namespace TodoSeUsa.Application.Features.Products.Interfaces;

public interface IProductImageService
{
    Task<string> SaveAsync(int productId, Stream content, CancellationToken ct);
    Task DeleteAsync(string publicPath, CancellationToken ct);
}
