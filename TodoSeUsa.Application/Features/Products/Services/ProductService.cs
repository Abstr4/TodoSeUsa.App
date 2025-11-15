using TodoSeUsa.Application.Common.Services;
using TodoSeUsa.Application.Features.Products.DTOs;
using TodoSeUsa.Application.Features.Products.Interfaces;
using TodoSeUsa.Application.Features.Products.Validators;
using TodoSeUsa.Domain.Enums;

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

    public async Task<Result<PagedItems<ProductDto>>> GetByBoxIdAsync(
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
                Quantity = p.Quantity,
                Category = p.Category,
                Description = p.Description,
                Body = p.Body,
                Size = p.Size,
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

    public async Task<Result<PagedItems<ProductDto>>> GetByConsignmentIdAsync(
    QueryRequest request,
    int consignmentId,
    CancellationToken cancellationToken)
    {
        var query = _context.Products.AsQueryable();

        query = query.Where(p => p.ConsignmentId == consignmentId);

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
                Quantity = p.Quantity,
                Category = p.Category,
                Description = p.Description,
                Body = p.Body,
                Size = p.Size,
                Quality = p.Quality,
                Status = p.Status,
                RefurbishmentCost = p.RefurbishmentCost,
                Season = p.Season != null ? p.Season : null,
                ConsignmentId = p.ConsignmentId,
                SaleId = p.SaleId,
                BoxId = p.BoxId,
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

    public async Task<Result<PagedItems<ProductDto>>> GetAllAsync(
    QueryRequest request,
    CancellationToken cancellationToken)
    {
        var query = _context.Products.AsQueryable();

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
                Quantity = p.Quantity,
                Category = p.Category,
                Description = p.Description,
                Body = p.Body,
                Size = p.Size,
                Quality = p.Quality,
                Status = p.Status,
                RefurbishmentCost = p.RefurbishmentCost,
                Season = p.Season != null ? p.Season : null,
                ConsignmentId = p.ConsignmentId,
                SaleId = p.SaleId,
                BoxId = p.BoxId,
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

    public async Task<Result<ProductDto>> GetByIdAsync(int productId, CancellationToken cancellationToken)
    {
        if (productId <= 0)
            return Result.Failure<ProductDto>(ProductErrors.Failure("El Id debe ser mayor que cero."));
        try
        {
            var product = await _context.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == productId, cancellationToken);

            if (product == null)
            {
                return Result.Failure<ProductDto>(ProductErrors.NotFound(productId));
            }
            var productDto = new ProductDto
            {
                Id = product.Id,
                Price = product.Price,
                Quantity = product.Quantity,
                Category = product.Category,
                Description = product.Description,
                Body = product.Body,
                Size = product.Size,
                Quality = product.Quality,
                Status = product.Status,
                RefurbishmentCost = product.RefurbishmentCost,
                Season = product.Season,
                ConsignmentId = product.ConsignmentId,
                SaleId = product.SaleId,
                BoxId = product.BoxId,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt
            };
            return Result.Success(productDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while trying to retrieve the product with ID {productId}.", productId);
            return Result.Failure<ProductDto>(ProductErrors.Failure($"Ocurrió un error inesperado al intentar recuperar el producto."));
        }
    }

    public async Task<Result<bool>> CreateAsync(CreateProductDto productDto, CancellationToken ct)
    {
        var validator = new CreateProductDtoValidator();
        var validationResult = await validator.ValidateAsync(productDto, ct);

        if (!validationResult.IsValid)
            return Result.Failure<bool>(ProductErrors.Failure(validationResult.ToString()));

        Product product = new()
        {
            Price = productDto.Price,
            Quantity = productDto.Quantity,
            Category = productDto.Category,
            Description = productDto.Description,
            Body = productDto.Body,
            Size = productDto.Size,
            Quality = productDto.Quality,
            Status = ProductStatus.Available,
            Season = productDto.Season,
            RefurbishmentCost = productDto.RefurbishmentCost,
            ConsignmentId = productDto.ConsignmentId,
            BoxId = productDto.BoxId
        };

        try
        {
            await _context.Products.AddAsync(product, ct);
            await _context.SaveChangesAsync(ct);
            return Result.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while trying to create the product.");
            return Result.Failure<bool>(ProductErrors.Failure($"Ocurrió un error inesperado al intentar crear el producto."));
        }
    }

    public async Task<Result<bool>> DeleteProductById(int productId, CancellationToken cancellationToken)
    {
        if (productId <= 0)
            return Result.Failure<bool>(ProductErrors.Failure("El Id debe ser mayor que cero."));
        try
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(b => b.Id == productId, cancellationToken);

            if (product is null)
                return Result.Failure<bool>(ProductErrors.NotFound(productId));

            _context.Products.Remove(product);
            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while trying to delete the product with ID {productId}", productId);
            return Result.Failure<bool>(ProductErrors.Failure($"Ocurrió un error inesperado al intentar borrar la caja."));
        }
    }

    public async Task<Result<bool>> EditProductById(int productId, EditProductDto editProductDto, CancellationToken cancellationToken)
    {
        if (productId <= 0)
            return Result.Failure<bool>(ProductErrors.Failure("El Id debe ser mayor que cero."));

        var validator = new EditProductDtoValidator();
        var validationResult = await validator.ValidateAsync(editProductDto, cancellationToken);

        if (!validationResult.IsValid)
            return Result.Failure<bool>(ProductErrors.Failure(validationResult.ToString()));

        try
        {
            Product? product = await _context.Products.FirstOrDefaultAsync(b => b.Id == productId, cancellationToken);
            if (product == null)
            {
                return Result.Failure<bool>(ProductErrors.NotFound(productId));
            }

            product.Price = editProductDto.Price;
            product.Quantity = editProductDto.Quantity;
            product.Description = editProductDto.Description;
            product.Category = editProductDto.Category;
            product.Body = editProductDto.Body;
            product.Size = editProductDto.Size;
            product.Quality = editProductDto.Quality;
            product.Status = editProductDto.Status;
            product.Season = editProductDto.Season;
            product.RefurbishmentCost = editProductDto.RefurbishmentCost;
            product.ConsignmentId = editProductDto.ConsignmentId;
            product.SaleId = editProductDto.SaleId;
            product.BoxId = editProductDto.BoxId;

            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while trying to edit the product with ID {productId}.", productId);
            return Result.Failure<bool>(ProductErrors.Failure($"Ocurrió un error inesperado al intentar editar el producto."));
        }
    }
}