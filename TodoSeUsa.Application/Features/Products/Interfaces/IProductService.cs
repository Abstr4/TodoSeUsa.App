using TodoSeUsa.Application.Features.Products.DTOs;

namespace TodoSeUsa.Application.Features.Products.Interfaces;

public interface IProductService
{
    Task<Result<PagedItems<ProductDto>>> GetAllAsync(QueryRequest request, CancellationToken cancellationToken);

    Task<Result<PagedItems<ProductDto>>> GetByBoxIdAsync(QueryRequest request, int boxId, CancellationToken cancellationToken);

    Task<Result<PagedItems<ProductDto>>> GetByConsignmentIdAsync(QueryRequest request, int consignmentId, CancellationToken cancellationToken);

    Task<Result<PagedItems<ProductDto>>> GetByProviderIdAsync(QueryRequest request, int providerId, CancellationToken cancellationToken);

    Task<Result<ProductDto>> GetByIdAsync(int productId, CancellationToken cancellationToken);

    Task<Result<bool>> CreateAsync(CreateProductDto productDto, CancellationToken ct);

    Task<Result<bool>> EditById(int productId, EditProductDto editProductDto, CancellationToken cancellationToken);

    Task<Result<bool>> DeleteById(int productId, CancellationToken cancellationToken);
}