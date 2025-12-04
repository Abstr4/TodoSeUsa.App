using TodoSeUsa.Application.Features.Products.DTOs;

namespace TodoSeUsa.Application.Features.Products.Interfaces;

public interface IProductService
{
    Task<Result<PagedItems<ProductDto>>> GetAllAsync(QueryRequest request, CancellationToken ct);

    Task<Result<PagedItems<ProductDto>>> GetByBoxIdAsync(QueryRequest request, int boxId, CancellationToken ct);

    Task<Result<PagedItems<ProductDto>>> GetByConsignmentIdAsync(QueryRequest request, int consignmentId, CancellationToken ct);

    Task<Result<PagedItems<ProductDto>>> GetByProviderIdAsync(QueryRequest request, int providerId, CancellationToken ct);

    Task<Result<ProductDto>> GetByIdAsync(int productId, CancellationToken ct);

    Task<Result<bool>> CreateAsync(CreateProductDto productDto, CancellationToken ct);

    Task<Result<bool>> CreateAsync(List<CreateProductDto> productDtos, CancellationToken ct);

    Task<Result<bool>> EditById(int productId, EditProductDto editProductDto, CancellationToken ct);

    Task<Result<bool>> DeleteById(int productId, CancellationToken ct);
}