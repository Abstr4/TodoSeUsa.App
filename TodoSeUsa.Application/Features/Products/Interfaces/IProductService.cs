using TodoSeUsa.Application.Features.Products.DTOs;

namespace TodoSeUsa.Application.Features.Products.Interfaces;

public interface IProductService
{
    Task<Result<PagedItems<ProductDto>>> GetProductsWithPaginationAsync(QueryItem request, CancellationToken cancellationToken);

    Task<Result<PagedItems<ProductDto>>> GetProductsByBoxIdWithPaginationAsync(QueryItem request, int boxId, CancellationToken cancellationToken);

    Task<Result<PagedItems<ProductDto>>> GetProductsByBoxIdAsync(QueryRequest request, int boxId, CancellationToken cancellationToken);
}