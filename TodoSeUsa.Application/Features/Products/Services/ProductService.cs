using TodoSeUsa.Application.Common.Extensions;
using TodoSeUsa.Application.Common.Services;
using TodoSeUsa.Application.Features.Boxes.DTOs;
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

    public async Task<Result<PagedItems<ProductDto>>> GetProductsByBoxIdAsync(
    QueryRequest request,
    int boxId,
    CancellationToken cancellationToken)
    {
        var query = _context.Products.AsQueryable();

        query = query.Where(p => p.BoxId == boxId);

        if (request.Filters != null && request.Filters.Count > 0)
        {
            var predicate = PredicateBuilder.BuildPredicate<Product>(request);
            query = query.Where(predicate);
        }

        if (request.Sorts != null && request.Sorts.Count != 0)
        {
            query = query.ApplySorting(request.Sorts);
        }
        else
            query = query.OrderBy(x => x.Id);

        var totalCount = await query.CountAsync(cancellationToken);

        query = query.Skip(request.Skip).Take(request.Take);

        var items = await query
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Price = p.Price,
                Category = p.Category,
                Description = p.Description,
                Quality = p.Quality,
                Status = p.Status,
                RefurbishmentCost = p.RefurbishmentCost,
                Season = p.Season != null ? p.Season : null,
                ConsignmentId = p.ConsignmentId,
                SaleId = p.SaleId,
                BoxId = boxId,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt

            })
        .ToListAsync(cancellationToken);

        return Result.Success(new PagedItems<ProductDto>
        {
            Items = items,
            Count = totalCount
        });
    }

/*
    public async Task<Result<PagedItems<ProductDto>>> GetProductsByBoxIdAsync(QueryRequest request, int boxId, CancellationToken cancellationToken)
    {
        try
        {
            IQueryable<Product> query = _context.Products
                .AsNoTracking()
                .Where(p => p.BoxId == boxId);

            // Apply filtering from QueryRequest
            if (request.Filters != null && request.Filters.Any())
            {
                var filterExpr = PredicateBuilder.BuildPredicate<Product>(request);
                query = query.Where(filterExpr);
            }

            // Apply ordering
            if (!string.IsNullOrWhiteSpace(request.OrderBy))
            {
                query = query.ApplySorting(request.OrderBy);
            }
            else if (request.Sorts != null && request.Sorts.Any())
            {
                // query = query.ApplySorting(request.Sorts); // You’d need an overload for SortDescriptor list
            }

            // Count total items
            var count = await query.CountAsync(cancellationToken);

            // Apply paging
            var items = await query
                .Skip(request.Skip)
                .Take(request.Take)
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Price = p.Price,
                    Category = p.Category,
                    Description = p.Description,
                    Status = p.Status,
                    Quality = p.Quality,
                    Season = p.Season.HasValue ? p.Season.Value : null,
                    RefurbishmentCost = p.RefurbishmentCost,
                    BoxId = p.BoxId,
                    CreatedAt = p.CreatedAt
                })
                .ToListAsync(cancellationToken);

            return Result.Success(new PagedItems<ProductDto>
            {
                Items = items,
                Count = count
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving products for box {BoxId}", boxId);
            return Result.Failure<PagedItems<ProductDto>>(ProductErrors.Failure());
        }
    }*/

    public async Task<Result<PagedItems<ProductDto>>> GetProductsByBoxIdWithPaginationAsync(QueryItem request, int boxId, CancellationToken cancellationToken)
    {
        try
        {
            IQueryable<Product> query = _context.Products
                .AsNoTracking()
                .AsQueryable();

            query = query.ApplyFilter(request.Filter);
            query = query.ApplySorting(request.OrderBy);

            var count = await query.CountAsync(cancellationToken);

            var productsDtos = await query
                .Skip(request.Skip)
                .Take(request.Take)
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Price = p.Price,
                    Category = p.Category,
                    Description = p.Description,
                    Quality = p.Quality,
                    Status = p.Status,
                    RefurbishmentCost = p.RefurbishmentCost,
                    Season = p.Season != null ? p.Season : null,
                    ConsignmentId = p.ConsignmentId,
                    SaleId = p.SaleId,
                    BoxId = boxId,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt

                })
                .ToListAsync(cancellationToken);

            var pagedItems = new PagedItems<ProductDto>() { Items = productsDtos, Count = count };


            return Result.Success(pagedItems);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An error occurred while retrieving paged products with boxId: {boxId}.");
            return Result.Failure<PagedItems<ProductDto>>(ProductErrors.Failure());
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
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Price = p.Price,
                    Category = p.Category,
                    Description = p.Description,
                    Quality = p.Quality,
                    Status = p.Status,
                    RefurbishmentCost = p.RefurbishmentCost,
                    Season = p.Season != null ? p.Season : null,
                    ConsignmentId = p.ConsignmentId,
                    SaleId = p.SaleId,
                    BoxId = p.BoxId,
                    CreatedAt = p.CreatedAt
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