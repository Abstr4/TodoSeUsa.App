using TodoSeUsa.Application.Features.Products.DTOs;

namespace TodoSeUsa.Application.Features.Products.Interfaces;

public interface IProductService
{
    Task<Result<PagedItems<ProductDto>>> GetProductsWithPagination(QueryItem request, CancellationToken cancellationToken);

    Task<Result<List<ProductDto>>> GetProductsByBoxIdAsync(int boxId, CancellationToken cancellationToken);
}