using TodoSeUsa.Application.Features.Products.DTOs;
using TodoSeUsa.Application.Features.Products.Interfaces;

namespace TodoSeUsa.Application.Features.Products.Services;

public class ProductService : IProductService
{
    private readonly ILogger<ProductService> _logger;
    private readonly IApplicationDbContext _context;

    public ProductService(ILogger<ProductService> logger, IApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<Result<List<ProductDto>>> GetProductsByBoxIdAsync(int boxId, CancellationToken cancellationToken)
    {
        try
        {
            var products = await _context.Products
                .Where(p => p.BoxId == boxId)
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Price = p.Price,
                    Category = p.Category,
                    Description = p.Description,
                    Quality = p.Quality,
                    Status = p.Status,
                    RefurbishmentCost = p.RefurbishmentCost,
                    Season = p.Season,
                    ConsignmentId = p.ConsignmentId,
                    SaleId = p.SaleId,
                    BoxId = boxId,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt
                    
                })
                .ToListAsync(cancellationToken);

            return Result.Success(products);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving paged products.");
            return Result.Failure<List<ProductDto>>(ProductErrors.Failure());
        }
    }


    public async Task<Result<PagedItems<ProductDto>>> GetProductsWithPaginationAsync(QueryItem request, CancellationToken cancellationToken)
    {
        try
        {
            IQueryable<Product> query = _context.Products
                .AsQueryable()
                .AsNoTracking();

            query = query.ApplyFilter(request.Filter);
            query = query.ApplySorting(request.OrderBy);

            var count = await query.CountAsync(cancellationToken);

            var productsDtos = await query
                .Skip(request.Skip)
                .Take(request.Take)
                .Select(b => new ProductDto
                {
                    Id = b.Id,
                    Price = b.Price,
                    Category = b.Category,
                    Description = b.Description,
                    Quality = b.Quality,
                    Status = b.Status,
                    RefurbishmentCost = b.RefurbishmentCost,
                    Season = b.Season,
                    ConsignmentId = b.ConsignmentId,
                    SaleId = b.SaleId,
                    BoxId = b.BoxId,
                    CreatedAt = b.CreatedAt
                })
                .ToListAsync(cancellationToken);

            var pagedItems = new PagedItems<ProductDto>() { Items = productsDtos, Count = count };

            return Result.Success(pagedItems);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving paged products.");

            return Result.Failure<PagedItems<ProductDto>>(ProductErrors.Failure());
        }
    }
}