using System.Text.RegularExpressions;
using TodoSeUsa.Application.Common.Helpers;
using TodoSeUsa.Application.Features.Products.DTOs;
using TodoSeUsa.Application.Features.Products.Interfaces;
using TodoSeUsa.Application.Features.Products.Validators;
using TodoSeUsa.Domain.Enums;

namespace TodoSeUsa.Application.Features.Products.Services;

public partial class ProductService : IProductService
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
        if (boxId < 1)
        {
            return Result.Failure<PagedItems<ProductDto>>(ProductErrors.Failure("El Id de la caja debe ser mayor que cero."));
        }
        try
        {
            var _context = await _contextFactory.CreateDbContextAsync(ct);

            var query = _context.Products
                .Include(p => p.Consignment)
                        .ThenInclude(c => c.Provider)
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving products for box ID {boxId}.", boxId);
            return Result.Failure<PagedItems<ProductDto>>(ProductErrors.Failure("Ocurrió un error inesperado al intentar recuperar los productos de la caja."));
        }

    }

    public async Task<Result<PagedItems<ProductDto>>> GetByConsignmentIdAsync(
    QueryRequest request,
    int consignmentId,
    CancellationToken ct)
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
                    .ThenInclude(c => c.Provider)
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving products for consignment ID {boxId}.", consignmentId);
            return Result.Failure<PagedItems<ProductDto>>(ProductErrors.Failure("Ocurrió un error inesperado al intentar recuperar los productos de la consignación."));
        }
    }

    public async Task<Result<PagedItems<ProductDto>>> GetByProviderIdAsync(
        QueryRequest request,
        int providerId,
        CancellationToken ct)
    {
        if (providerId < 1)
        {
            return Result.Failure<PagedItems<ProductDto>>(ProductErrors.Failure("El Id del proveedor debe ser mayor que cero."));
        }
        try
        {
            var _context = await _contextFactory.CreateDbContextAsync(ct);

            var query = _context.Products
                .Include(p => p.Consignment)
                    .ThenInclude(c => c.Provider)
                        .ThenInclude(pr => pr.Person)
                .Where(p => p.Consignment.ProviderId == providerId);

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
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving products for provider ID {boxId}.", providerId);
            return Result.Failure<PagedItems<ProductDto>>(ProductErrors.Failure("Ocurrió un error inesperado al intentar recuperar los productos del proveedor."));
        }
    }

    public async Task<Result<PagedItems<ProductDto>>> GetAllAsync(
    QueryRequest request,
    CancellationToken ct)
    {
        try
        {
            var _context = await _contextFactory.CreateDbContextAsync(ct);

            var query = _context.Products
                .Include(p => p.Consignment)
                    .ThenInclude(c => c.Provider)
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
                    ProductCode = p.ProductCode,
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

    public async Task<Result<ProductDto>> GetByCodeAsync(string productCode, CancellationToken ct)
    {
        productCode = NormalizeProductCode(productCode);

        if (!Regex.IsMatch(productCode, @"^TSU-\d+$"))
        {
            return Result.Failure<ProductDto>(
                ProductErrors.Failure("El código es inválido, debe comenzar con 'TSU-' y continuar con números.")
            );
        }

        try
        {
            var context = await _contextFactory.CreateDbContextAsync(ct);

            var product = await context.Products
                .AsNoTracking()
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    ProductCode = p.ProductCode,
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
                    ProviderId = p.Consignment.ProviderId,
                    ProviderFirstName = p.Consignment.Provider.Person.FirstName,
                    ProviderLastName = p.Consignment.Provider.Person.LastName,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt
                })
                .Where(p => p.ProductCode == productCode)
                .FirstOrDefaultAsync(ct);

            if (product == null)
            {
                return Result.Failure<ProductDto>(ProductErrors.NotFound(productCode));
            }

            return Result.Success(product);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting product by code {ProductCode}", productCode);
            return Result.Failure<ProductDto>(
                ProductErrors.Failure("Ocurrió un error inesperado al buscar el producto.")
            );
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

    public async Task<Result<bool>> CreateAsync(List<CreateProductDto> productDtos, CancellationToken ct)
    {
        if (productDtos == null || productDtos.Count == 0)
            return Result.Failure<bool>(ProductErrors.Failure("No products provided."));

        var validator = new CreateProductDtoValidator();

        var validationErrors = ValidateOriginalDtos(productDtos, validator, ct);
        if (validationErrors.Count > 0)
            return Result.Failure<bool>(ProductErrors.Failure(string.Join("; ", validationErrors)));

        var expandedDtos = ExpandByQuantity(productDtos).ToList();

        try
        {
            var context = await _contextFactory.CreateDbContextAsync(ct);

            var products = expandedDtos.Select(MapToEntity).ToList();

            await context.Products.AddRangeAsync(products, ct);
            await context.SaveChangesAsync(ct);

            return Result.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while trying to create the products.");
            return Result.Failure<bool>(ProductErrors.Failure("Ocurrió un error inesperado creando los productos."));
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

    private static List<string> ValidateOriginalDtos(IEnumerable<CreateProductDto> dtos, CreateProductDtoValidator validator, CancellationToken ct)
    {
        var errors = new List<string>();

        foreach (var dto in dtos)
        {
            var result = validator.ValidateAsync(dto, ct).Result;
            if (!result.IsValid)
                errors.Add(result.ToString());
        }

        return errors;
    }

    private static string NormalizeProductCode(string code)
    {
        var trimmed = code.Trim();

        if (trimmed.StartsWith("TSU-", StringComparison.OrdinalIgnoreCase))
            return trimmed.ToUpperInvariant();

        return $"TSU-{trimmed}";
    }

    private static IEnumerable<CreateProductDto> ExpandByQuantity(IEnumerable<CreateProductDto> dtos)
    {
        foreach (var dto in dtos)
            for (int i = 0; i < dto.Quantity; i++)
                yield return dto;
    }

    private Product MapToEntity(CreateProductDto dto) => new()
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
}