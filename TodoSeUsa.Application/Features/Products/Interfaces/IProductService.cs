using TodoSeUsa.Application.Features.Products.DTOs;

namespace TodoSeUsa.Application.Features.Products.Interfaces;

public interface IProductService
{
    Task<Result<int>> GetAvailableCountAsync(CancellationToken ct);

    Task<Result<decimal>> GetAvailableInventoryValueAsync(CancellationToken ct);

    Task<Result<PagedItems<ProductDto>>> GetAllAsync(QueryRequest request, CancellationToken ct);

    Task<Result<PagedItems<ProductDto>>> GetByBoxIdAsync(QueryRequest request, int boxId, CancellationToken ct);

    Task<Result<PagedItems<ProductDto>>> GetByConsignmentIdAsync(QueryRequest request, int consignmentId, CancellationToken ct);

    Task<Result<PagedItems<ProductDto>>> GetByProviderIdAsync(QueryRequest request, int providerId, CancellationToken ct);

    Task<Result<ProductDto>> GetByIdAsync(int productId, CancellationToken ct);

    Task<Result<ProductDto>> GetByCodeAsync(string productCode, CancellationToken ct);

    Task<Result<int>> CreateAsync(CreateProductDto dto, IReadOnlyList<Stream> imageStreams, CancellationToken ct);

    Task<Result<IReadOnlyList<int>>> CreateBatchAsync(CreateProductDto dto, IReadOnlyList<Stream> imageStreams, CancellationToken ct);

    Task<Result> EditById(int productId, EditProductDto editProductDto, CancellationToken ct);

    Task<Result> DeleteById(int productId, CancellationToken ct);
}