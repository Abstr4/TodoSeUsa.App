using TodoSeUsa.Application.Common.Querying.CustomCases;
using TodoSeUsa.Application.Features.Products.DTOs;
using TodoSeUsa.Application.Features.Products.Interfaces;
using TodoSeUsa.Application.Features.Products.Validators;
using TodoSeUsa.Domain.Enums;

namespace TodoSeUsa.Application.Features.Products.Services;

public partial class ProductService : IProductService
{
    private readonly ILogger<ProductService> _logger;
    private readonly IApplicationDbContextFactory _contextFactory;
    private readonly IProductImageService _productImageService;

    public ProductService(
        ILogger<ProductService> logger,
        IApplicationDbContextFactory contextFactory,
        IProductImageService productImageService)
    {
        _logger = logger;
        _contextFactory = contextFactory;
        _productImageService = productImageService;
    }

    public async Task<Result<int>> GetAvailableCountAsync(CancellationToken ct)
    {
        try
        {
            var context = await _contextFactory.CreateDbContextAsync(ct);

            var count = await context.Products
                .Where(p => p.Status == ProductStatus.Available)
                .CountAsync(ct);

            return Result.Success(count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while counting available products.");

            return Result.Failure<int>(
                ProductErrors.Failure("Ocurrió un error inesperado al intentar contar los productos disponibles.")
            );
        }
    }

    public async Task<Result<decimal>> GetAvailableInventoryValueAsync(CancellationToken ct)
    {
        try
        {
            var context = await _contextFactory.CreateDbContextAsync(ct);

            var totalValue = await context.Products
                .Where(p => p.Status == ProductStatus.Available)
                .SumAsync(p => p.Price, ct);

            return Result.Success(totalValue);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while calculating available inventory value.");

            return Result.Failure<decimal>(
                ProductErrors.Failure("Ocurrió un error inesperado al intentar calcular el valor del inventario disponible.")
            );
        }
    }

    public async Task<Result<PagedItems<ProductDto>>> GetByBoxIdAsync(QueryRequest request, int boxId, CancellationToken ct)
    {
        if (boxId < 1)
        {
            return Result.Failure<PagedItems<ProductDto>>(ProductErrors.Failure("El Id de la caja debe ser mayor que cero."));
        }
        try
        {
            var _context = await _contextFactory.CreateDbContextAsync(ct);

            var query = _context.Products
                .Include(p => p.Consignment)
                        .ThenInclude(c => c.Consignor)
                            .ThenInclude(pr => pr.Person)
                .Where(p => p.BoxId == boxId);

            query = QueryableExtensions.ApplyCustomFiltering(query, request.Filters, request.LogicalFilterOperator, QueryFilteringCases.ProductFilters);

            query = QueryableExtensions.ApplyCustomSorting(query, request.Sorts, QuerySortingCases.ProductSorts);

            var totalCount = await query.CountAsync(ct);

            query = query.Skip(request.Skip).Take(request.Take);

            var items = await query
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Price = p.Price,
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
                    ConsignorId = p.Consignment.ConsignorId,
                    ConsignorFirstName = p.Consignment.Consignor.Person.FirstName,
                    ConsignorLastName = p.Consignment.Consignor.Person.LastName,
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving products for box ID {boxId}.", boxId);
            return Result.Failure<PagedItems<ProductDto>>(ProductErrors.Failure("Ocurrió un error inesperado al intentar recuperar los productos de la caja."));
        }
    }

    public async Task<Result<PagedItems<ProductDto>>> GetByConsignmentIdAsync(QueryRequest request, int consignmentId, CancellationToken ct)
    {
        if (consignmentId < 1)
        {
            return Result.Failure<PagedItems<ProductDto>>(ProductErrors.Failure("El Id de la consignación debe ser mayor que cero."));
        }
        try
        {
            var _context = await _contextFactory.CreateDbContextAsync(ct);

            var query = _context.Products
                .Include(p => p.Consignment)
                    .ThenInclude(c => c.Consignor)
                        .ThenInclude(pr => pr.Person)
                .Where(p => p.ConsignmentId == consignmentId);

            query = QueryableExtensions.ApplyCustomFiltering(query, request.Filters, request.LogicalFilterOperator, QueryFilteringCases.ProductFilters);

            query = QueryableExtensions.ApplyCustomSorting(query, request.Sorts, QuerySortingCases.ProductSorts);

            var totalCount = await query.CountAsync(ct);

            query = query.Skip(request.Skip).Take(request.Take);

            var items = await query
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Price = p.Price,
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
                    ConsignorId = p.Consignment.ConsignorId,
                    ConsignorFirstName = p.Consignment.Consignor.Person.FirstName,
                    ConsignorLastName = p.Consignment.Consignor.Person.LastName,
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving products for consignment ID {boxId}.", consignmentId);
            return Result.Failure<PagedItems<ProductDto>>(ProductErrors.Failure("Ocurrió un error inesperado al intentar recuperar los productos de la consignación."));
        }
    }

    public async Task<Result<PagedItems<ProductDto>>> GetByConsignorIdAsync(QueryRequest request, int consignorId, CancellationToken ct)
    {
        if (consignorId < 1)
        {
            return Result.Failure<PagedItems<ProductDto>>(ProductErrors.Failure("El Id del consignador debe ser mayor que cero."));
        }
        try
        {
            var _context = await _contextFactory.CreateDbContextAsync(ct);

            var query = _context.Products
                .Include(p => p.Consignment)
                    .ThenInclude(c => c.Consignor)
                        .ThenInclude(pr => pr.Person)
                .Where(p => p.Consignment.ConsignorId == consignorId);

            query = QueryableExtensions.ApplyCustomFiltering(query, request.Filters, request.LogicalFilterOperator, QueryFilteringCases.ProductFilters);

            query = QueryableExtensions.ApplyCustomSorting(query, request.Sorts, QuerySortingCases.ProductSorts);

            var total = await query.CountAsync(ct);

            var items = await query
                .Skip(request.Skip)
                .Take(request.Take)
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Price = p.Price,
                    Category = p.Category,
                    Description = p.Description,
                    Body = p.Body,
                    Size = p.Size,
                    Quality = p.Quality,
                    Status = p.Status,
                    RefurbishmentCost = p.RefurbishmentCost,
                    Season = p.Season,
                    ConsignmentId = p.ConsignmentId,
                    ConsignorId = p.Consignment.ConsignorId,
                    ConsignorFirstName = p.Consignment.Consignor.Person.FirstName,
                    ConsignorLastName = p.Consignment.Consignor.Person.LastName,
                    SaleId = p.SaleId,
                    BoxId = p.BoxId,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt
                })
                .ToListAsync(ct);

            return Result.Success(new PagedItems<ProductDto> { Items = items, Count = total });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving products for consignor ID {boxId}.", consignorId);
            return Result.Failure<PagedItems<ProductDto>>(ProductErrors.Failure("Ocurrió un error inesperado al intentar recuperar los productos del consignador."));
        }
    }

    public async Task<Result<PagedItems<ProductDto>>> GetAllAsync(QueryRequest request, CancellationToken ct)
    {
        try
        {
            var _context = await _contextFactory.CreateDbContextAsync(ct);

            var query = _context.Products
                .Include(p => p.Consignment)
                    .ThenInclude(c => c.Consignor)
                        .ThenInclude(pr => pr.Person)
                .AsQueryable();

            query = QueryableExtensions.ApplyCustomFiltering(query, request.Filters, request.LogicalFilterOperator, QueryFilteringCases.ProductFilters);

            query = QueryableExtensions.ApplyCustomSorting(query, request.Sorts, QuerySortingCases.ProductSorts);

            var totalCount = await query.CountAsync(ct);

            query = query.Skip(request.Skip).Take(request.Take);

            var items = await query
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Price = p.Price,
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
                    ConsignorId = p.Consignment.ConsignorId,
                    ConsignorFirstName = p.Consignment.Consignor.Person.FirstName,
                    ConsignorLastName = p.Consignment.Consignor.Person.LastName,
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving products.");
            return Result.Failure<PagedItems<ProductDto>>(ProductErrors.Failure("Ocurrió un error inesperado al intentar recuperar los productos."));
        }
    }

    public async Task<Result<ProductDto>> GetByIdAsync(int productId, CancellationToken ct)
    {
        if (productId < 1)
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
                    ConsignorId = p.Consignment.ConsignorId,
                    ConsignorFirstName = p.Consignment.Consignor.Person.FirstName,
                    ConsignorLastName = p.Consignment.Consignor.Person.LastName,
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

    public async Task<Result<int>> CreateAsync(CreateProductDto dto, IReadOnlyList<Stream> imageStreams, CancellationToken ct)
    {
        var validator = new CreateProductDtoValidator();
        var validationResult = await validator.ValidateAsync(dto, ct);

        if (!validationResult.IsValid)
            return Result.Failure<int>(ProductErrors.Failure(validationResult.ToString()));

        var context = await _contextFactory.CreateDbContextAsync(ct);
        await using var transaction = await context.BeginTransactionAsync(ct);

        try
        {
            var product = new Product
            {
                Price = dto.Price,
                Category = dto.Category,
                Description = dto.Description,
                Body = dto.Body,
                Size = dto.Size,
                Quality = dto.Quality,
                Status = ProductStatus.Available,
                Season = dto.Season,
                RefurbishmentCost = dto.RefurbishmentCost,
                ConsignmentId = dto.ConsignmentId,
                BoxId = dto.BoxId
            };

            var entity = await context.Products.AddAsync(product, ct);
            var save = await context.SaveChangesAsync(ct);

            if (save == 0)
                return Result.Failure<int>(ProductErrors.Failure("No se pudo crear el producto."));

            var attachResult = await TryAttachImagesAsync(product, imageStreams, context, ct);
            if (attachResult.IsFailure)
            {
                return Result.Failure<int>(attachResult.Error);
            }

            var saveImages = await context.SaveChangesAsync(ct);
            if (saveImages == 0 && imageStreams.Count > 0)
            {
                return Result.Failure<int>(ProductErrors.Failure("No se pudo crear el producto."));
            }

            await transaction.CommitAsync(ct);

            return Result.Success(entity.Entity.Id);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(ct);

            _logger.LogError(ex, "Error creating product");

            return Result.Failure<int>(ProductErrors.Failure("Ocurrió un error inesperado al intentar crear el producto."
            ));
        }
    }

    public async Task<Result<int>> CreateAsync(CreateProductDto dto, CancellationToken ct)
    {
        var validator = new CreateProductDtoValidator();
        var validationResult = await validator.ValidateAsync(dto, ct);

        if (!validationResult.IsValid)
            return Result.Failure<int>(ProductErrors.Failure(validationResult.ToString()));

        var context = await _contextFactory.CreateDbContextAsync(ct);

        try
        {
            var product = new Product
            {
                Price = dto.Price,
                Category = dto.Category,
                Description = dto.Description,
                Body = dto.Body,
                Size = dto.Size,
                Quality = dto.Quality,
                Status = ProductStatus.Available,
                Season = dto.Season,
                RefurbishmentCost = dto.RefurbishmentCost,
                ConsignmentId = dto.ConsignmentId,
                BoxId = dto.BoxId
            };

            var entity = await context.Products.AddAsync(product, ct);

            var save = await context.SaveChangesAsync(ct);

            if (save == 0)
                return Result.Failure<int>(ProductErrors.Failure("No se pudo crear el producto."));

            return Result.Success(entity.Entity.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating product");

            return Result.Failure<int>(ProductErrors.Failure("Ocurrió un error inesperado al intentar crear el producto."
            ));
        }
    }

    public async Task<Result<IReadOnlyList<int>>> CreateBatchAsync(CreateProductDto dto, IReadOnlyList<Stream> imageStreams, CancellationToken ct)
    {
        var validator = new CreateProductDtoValidator();
        var validationResult = await validator.ValidateAsync(dto, ct);

        if (!validationResult.IsValid)
            return Result.Failure<IReadOnlyList<int>>(ProductErrors.Failure(validationResult.ToString()));

        var context = await _contextFactory.CreateDbContextAsync(ct);
        await using var transaction = await context.BeginTransactionAsync(ct);

        try
        {
            var createdIds = new List<int>(dto.Quantity);

            for (int i = 0; i < dto.Quantity; i++)
            {
                var product = new Product
                {
                    Price = dto.Price,
                    Category = dto.Category,
                    Description = dto.Description,
                    Body = dto.Body,
                    Size = dto.Size,
                    Quality = dto.Quality,
                    Status = ProductStatus.Available,
                    Season = dto.Season,
                    RefurbishmentCost = dto.RefurbishmentCost,
                    ConsignmentId = dto.ConsignmentId,
                    BoxId = dto.BoxId
                };

                await context.Products.AddAsync(product, ct);

                var saveProduct = await context.SaveChangesAsync(ct);

                if (saveProduct == 0)
                    return Result.Failure<IReadOnlyList<int>>(ProductErrors.Failure("No se pudo crear el producto."));

                var attachResult = await TryAttachImagesAsync(product, imageStreams, context, ct);
                if (attachResult.IsFailure)
                    return Result.Failure<IReadOnlyList<int>>(attachResult.Error);

                var saveImages = await context.SaveChangesAsync(ct);
                if (saveImages == 0 && imageStreams.Count > 0)
                    return Result.Failure<IReadOnlyList<int>>(ProductErrors.Failure("No se pudo crear el producto."));

                createdIds.Add(product.Id);
            }

            await transaction.CommitAsync(ct);

            return Result.Success<IReadOnlyList<int>>(createdIds);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(ct);

            _logger.LogError(ex, "Error creating products");

            return Result.Failure<IReadOnlyList<int>>(
                ProductErrors.Failure("Ocurrió un error inesperado al intentar crear los productos."));
        }
    }

    public async Task<Result<IReadOnlyList<int>>> CreateBatchAsync(CreateProductDto dto, CancellationToken ct)
    {
        var validator = new CreateProductDtoValidator();
        var validationResult = await validator.ValidateAsync(dto, ct);

        if (!validationResult.IsValid)
            return Result.Failure<IReadOnlyList<int>>(ProductErrors.Failure(validationResult.ToString()));

        var context = await _contextFactory.CreateDbContextAsync(ct);
        await using var transaction = await context.BeginTransactionAsync(ct);

        try
        {
            var createdIds = new List<int>(dto.Quantity);

            for (int i = 0; i < dto.Quantity; i++)
            {
                var product = new Product
                {
                    Price = dto.Price,
                    Category = dto.Category,
                    Description = dto.Description,
                    Body = dto.Body,
                    Size = dto.Size,
                    Quality = dto.Quality,
                    Status = ProductStatus.Available,
                    Season = dto.Season,
                    RefurbishmentCost = dto.RefurbishmentCost,
                    ConsignmentId = dto.ConsignmentId,
                    BoxId = dto.BoxId
                };

                await context.Products.AddAsync(product, ct);

                var saveProduct = await context.SaveChangesAsync(ct);

                if (saveProduct == 0)
                    return Result.Failure<IReadOnlyList<int>>(ProductErrors.Failure("No se pudo crear el producto."));

                createdIds.Add(product.Id);
            }

            await transaction.CommitAsync(ct);

            return Result.Success<IReadOnlyList<int>>(createdIds);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(ct);

            _logger.LogError(ex, "Error creating products");

            return Result.Failure<IReadOnlyList<int>>(
                ProductErrors.Failure("Ocurrió un error inesperado al intentar crear los productos."));
        }
    }

    private async Task<Result> TryAttachImagesAsync(Product product, IReadOnlyList<Stream> imageStreams, IApplicationDbContext context, CancellationToken ct)
    {
        if (imageStreams.Count == 0)
            return Result.Success();

        var savedPaths = new List<string>();

        foreach (var stream in imageStreams)
        {
            var saveResult = await _productImageService.SaveAsync(product.Id, stream, ct);
            if (saveResult is null)
            {
                foreach (var path in savedPaths)
                {
                    await _productImageService.DeleteAsync(path, ct);
                }

                return Result.Failure(ProductErrors.Failure("No se pudieron adjuntar una o más imágenes."));
            }

            savedPaths.Add(saveResult);

            product.Images.Add(new ProductImage
            {
                Path = saveResult
            });
        }

        var saveImages = await context.SaveChangesAsync(ct);
        if (saveImages == 0)
        {
            return Result.Failure<int>(ProductErrors.Failure("No se pudieron guardar las imágenes."));
        }

        return Result.Success();
    }

    public async Task<Result> DeleteById(int productId, CancellationToken ct)
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

    public async Task<Result> EditById(int productId, EditProductDto editProductDto, CancellationToken ct)
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
}