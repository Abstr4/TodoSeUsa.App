using TodoSeUsa.Application.Common.Enums;
using TodoSeUsa.Application.Common.Services;
using TodoSeUsa.Application.Features.Products.DTOs;
using TodoSeUsa.Application.Features.Products.Interfaces;
using TodoSeUsa.Application.Features.Products.Validators;
using TodoSeUsa.Domain.Enums;

namespace TodoSeUsa.Application.Features.Products.Services;

public class ProductService : IProductService
{
    private readonly ILogger<ProductService> _logger;
    private readonly IApplicationDbContextFactory _contextFactory;

    public ProductService(
        ILogger<ProductService> logger, IApplicationDbContextFactory contextFactory)
    {
        _logger = logger;
        _contextFactory = contextFactory;
    }

    public async Task<Result<PagedItems<ProductDto>>> GetByBoxIdAsync(
    QueryRequest request,
    int boxId,
    CancellationToken ct)
    {
        var _context = await _contextFactory.CreateDbContextAsync(ct);

        var query = _context.Products
            .Include(p => p.Consignment)
                .ThenInclude(c => c.Provider)
                    .ThenInclude(pr => pr.Person)
            .Where(p => p.BoxId == boxId);

        if (request.Filters != null && request.Filters.Count > 0)
        {
            query = ApplyCustomFilter(query, request);
        }
        if (request.Sorts != null && request.Sorts.Count > 0)
        {
            query = ApplyCustomSorting(query, request.Sorts);
        }
        else
        {
            query = query.OrderBy(x => x.Id);
        }

        var totalCount = await query.CountAsync(ct);

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
                Season = p.Season,
                ConsignmentId = p.ConsignmentId,
                SaleId = p.SaleId,
                BoxId = boxId,
                ProviderId = p.Consignment.ProviderId,
                ProviderFirstName = p.Consignment.Provider.Person.FirstName,
                ProviderLastName = p.Consignment.Provider.Person.LastName,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            })
        .ToListAsync(ct);

        return Result.Success(new PagedItems<ProductDto>
        {
            Items = items,
            Count = totalCount
        });
    }

    public async Task<Result<PagedItems<ProductDto>>> GetByConsignmentIdAsync(
    QueryRequest request,
    int consignmentId,
    CancellationToken ct)
    {
        var _context = await _contextFactory.CreateDbContextAsync(ct);

        var query = _context.Products
            .Include(p => p.Consignment)
                .ThenInclude(c => c.Provider)
                    .ThenInclude(pr => pr.Person)
            .Where(p => p.ConsignmentId == consignmentId);

        if (request.Filters != null && request.Filters.Count > 0)
        {
            query = ApplyCustomFilter(query, request);
        }
        if (request.Sorts != null && request.Sorts.Count > 0)
        {
            query = ApplyCustomSorting(query, request.Sorts);
        }
        else
        {
            query = query.OrderBy(x => x.Id);
        }

        var totalCount = await query.CountAsync(ct);

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
                Season = p.Season,
                ConsignmentId = p.ConsignmentId,
                SaleId = p.SaleId,
                BoxId = p.BoxId,
                ProviderId = p.Consignment.ProviderId,
                ProviderFirstName = p.Consignment.Provider.Person.FirstName,
                ProviderLastName = p.Consignment.Provider.Person.LastName,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            })
        .ToListAsync(ct);

        return Result.Success(new PagedItems<ProductDto>
        {
            Items = items,
            Count = totalCount
        });
    }

    public async Task<Result<PagedItems<ProductDto>>> GetByProviderIdAsync(
        QueryRequest request,
        int providerId,
        CancellationToken ct)
    {
        var _context = await _contextFactory.CreateDbContextAsync(ct);

        var query = _context.Products
            .Include(p => p.Consignment)
                .ThenInclude(c => c.Provider)
                    .ThenInclude(pr => pr.Person)
            .Where(p => p.Consignment.ProviderId == providerId);

        if (request.Filters != null && request.Filters.Count > 0)
        {
            query = ApplyCustomFilter(query, request);
        }
        if (request.Sorts != null && request.Sorts.Count > 0)
        {
            query = ApplyCustomSorting(query, request.Sorts);
        }
        else
        {
            query = query.OrderBy(x => x.Id);
        }

        var total = await query.CountAsync(ct);

        var items = await query
            .Skip(request.Skip)
            .Take(request.Take)
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
                Season = p.Season,
                ConsignmentId = p.ConsignmentId,
                ProviderId = p.Consignment.ProviderId,
                ProviderFirstName = p.Consignment.Provider.Person.FirstName,
                ProviderLastName = p.Consignment.Provider.Person.LastName,
                SaleId = p.SaleId,
                BoxId = p.BoxId,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            })
            .ToListAsync(ct);

        return Result.Success(new PagedItems<ProductDto> { Items = items, Count = total });
    }

    public async Task<Result<PagedItems<ProductDto>>> GetAllAsync(
    QueryRequest request,
    CancellationToken ct)
    {
        var _context = await _contextFactory.CreateDbContextAsync(ct);

        var query = _context.Products
            .Include(p => p.Consignment)
                .ThenInclude(c => c.Provider)
                    .ThenInclude(pr => pr.Person)
            .AsQueryable();

        if (request.Filters != null && request.Filters.Count > 0)
        {
            query = ApplyCustomFilter(query, request);
        }
        if (request.Sorts != null && request.Sorts.Count > 0)
        {
            query = ApplyCustomSorting(query, request.Sorts);
        }
        else
        {
            query = query.OrderBy(x => x.Id);
        }

        var totalCount = await query.CountAsync(ct);

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
                Season = p.Season,
                ConsignmentId = p.ConsignmentId,
                SaleId = p.SaleId,
                BoxId = p.BoxId,
                ProviderId = p.Consignment.ProviderId,
                ProviderFirstName = p.Consignment.Provider.Person.FirstName,
                ProviderLastName = p.Consignment.Provider.Person.LastName,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            })
        .ToListAsync(ct);

        return Result.Success(new PagedItems<ProductDto>
        {
            Items = items,
            Count = totalCount
        });
    }

    public async Task<Result<ProductDto>> GetByIdAsync(int productId, CancellationToken ct)
    {
        if (productId <= 0)
        {
            return Result.Failure<ProductDto>(ProductErrors.Failure("El Id debe ser mayor que cero."));
        }

        try
        {
            var _context = await _contextFactory.CreateDbContextAsync(ct);

            var productDto = await _context.Products
                .AsNoTracking()
                .Where(p => p.Id == productId)
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
                    Season = p.Season,
                    ConsignmentId = p.ConsignmentId,
                    SaleId = p.SaleId,
                    BoxId = p.BoxId,
                    ProviderId = p.Consignment.ProviderId,
                    ProviderFirstName = p.Consignment.Provider.Person.FirstName,
                    ProviderLastName = p.Consignment.Provider.Person.LastName,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt
                })
                .FirstOrDefaultAsync(ct);

            if (productDto == null)
                return Result.Failure<ProductDto>(ProductErrors.NotFound(productId));

            return Result.Success(productDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving product ID {productId}.", productId);
            return Result.Failure<ProductDto>(ProductErrors.Failure("Ocurrió un error inesperado al intentar recuperar el producto."));
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
            var _context = await _contextFactory.CreateDbContextAsync(ct);

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

    public async Task<Result<bool>> DeleteById(int productId, CancellationToken ct)
    {
        if (productId <= 0)
            return Result.Failure<bool>(ProductErrors.Failure("El Id debe ser mayor que cero."));
        try
        {
            var _context = await _contextFactory.CreateDbContextAsync(ct);

            var product = await _context.Products
                .FirstOrDefaultAsync(b => b.Id == productId, ct);

            if (product is null)
                return Result.Failure<bool>(ProductErrors.NotFound(productId));

            _context.Products.Remove(product);
            await _context.SaveChangesAsync(ct);
            return Result.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while trying to delete the product with ID {productId}", productId);
            return Result.Failure<bool>(ProductErrors.Failure($"Ocurrió un error inesperado al intentar borrar la caja."));
        }
    }

    public async Task<Result<bool>> EditById(int productId, EditProductDto editProductDto, CancellationToken ct)
    {
        if (productId <= 0)
            return Result.Failure<bool>(ProductErrors.Failure("El Id debe ser mayor que cero."));

        var validator = new EditProductDtoValidator();
        var validationResult = await validator.ValidateAsync(editProductDto, ct);

        if (!validationResult.IsValid)
            return Result.Failure<bool>(ProductErrors.Failure(validationResult.ToString()));

        try
        {
            var _context = await _contextFactory.CreateDbContextAsync(ct);

            Product? product = await _context.Products.FirstOrDefaultAsync(b => b.Id == productId, ct);
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

            await _context.SaveChangesAsync(ct);
            return Result.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while trying to edit the product with ID {productId}.", productId);
            return Result.Failure<bool>(ProductErrors.Failure($"Ocurrió un error inesperado al intentar editar el producto."));
        }
    }

    public static IQueryable<Product> ApplyCustomFilter(IQueryable<Product> query, QueryRequest request)
    {
        if (request.Filters == null || request.Filters.Count == 0)
            return query;

        var remainingFilters = new List<FilterDescriptor>();

        foreach (var filter in request.Filters)
        {
            if (string.IsNullOrWhiteSpace(filter.Property) || filter.FilterValue == null)
                continue;

            switch (filter.Property)
            {
                case "ProviderInfo":
                    var val = filter.FilterValue.ToString();
                    query = query.Where(c =>
                        EF.Functions.Like(c.Consignment.Provider.Person.FirstName, $"%{val}%") ||
                        EF.Functions.Like(c.Consignment.Provider.Person.LastName, $"%{val}%") ||
                        EF.Functions.Like(c.Consignment.ProviderId.ToString(), $"%{val}%")
                    );
                    break;

                default:
                    remainingFilters.Add(filter);
                    break;
            }
        }

        if (remainingFilters.Count > 0)
        {
            var subRequest = new QueryRequest
            {
                Filters = remainingFilters,
                LogicalFilterOperator = request.LogicalFilterOperator
            };
            var predicate = PredicateBuilder.BuildPredicate<Product>(subRequest);
            query = query.Where(predicate);
        }

        return query;
    }

    public static IQueryable<Product> ApplyCustomSorting(IQueryable<Product> query, IEnumerable<SortDescriptor>? sorts)
    {
        var sort = sorts?.FirstOrDefault();
        if (sort == null || string.IsNullOrWhiteSpace(sort.Property))
            return query;

        var property = sort.Property;
        var isDescending = sort.SortOrder == SortOrder.Descending;

        if (sort.Property == nameof(ProductDto.ProviderInfo))
        {
            return isDescending
                 ? query.OrderByDescending(p => p.Consignment.Provider.Person.FirstName)
                            .ThenByDescending(p => p.Consignment.Provider.Person.LastName)
                 : query.OrderBy(p => p.Consignment.Provider.Person.FirstName).ThenBy(p => p.Consignment.Provider.Person.LastName);
        }

        return isDescending
            ? query.OrderByDescending(e => EF.Property<object>(e, property))
            : query.OrderBy(e => EF.Property<object>(e, property));
    }
}