using System.Linq;
using System.Linq.Dynamic.Core;
using TodoSeUsa.Application.Common.Querying.CustomCases;
using TodoSeUsa.Application.Features.Boxes.DTOs;
using TodoSeUsa.Application.Features.Boxes.Interfaces;
using TodoSeUsa.Application.Features.Boxes.Validators;
using TodoSeUsa.Application.Features.Products;

namespace TodoSeUsa.Application.Features.Boxes.Services;
public class BoxService : IBoxService
{
    private readonly ILogger<BoxService> _logger;
    private readonly IApplicationDbContextFactory _contextFactory;

    public BoxService(ILogger<BoxService> logger, IApplicationDbContextFactory contextFactory)
    {
        _logger = logger;
        _contextFactory = contextFactory;
    }

    public async Task<Result<PagedItems<BoxDto>>> GetAllAsync(
        QueryRequest request,
        CancellationToken ct)
    {
        var _context = await _contextFactory.CreateDbContextAsync(ct);

        var query = _context.Boxes.AsQueryable();

        query = QueryableExtensions.ApplyCustomFiltering(query, request.Filters, request.LogicalFilterOperator);

        query = QueryableExtensions.ApplyCustomSorting(query, request.Sorts, QuerySortingCases.BoxCustomSorts);

        var totalCount = await query.CountAsync(ct);

        query = query.Skip(request.Skip).Take(request.Take);

        var items = await query
            .Select(b => new BoxDto
            {
                Id = b.Id,
                TotalProducts = b.Products.Count(),
                Location = b.Location,
                Code = b.Code,
                CreatedAt = b.CreatedAt,
                UpdatedAt = b.UpdatedAt,
            })
            .ToListAsync(ct);

        return Result.Success(new PagedItems<BoxDto>
        {
            Items = items,
            Count = totalCount
        });
    }

    public async Task<Result<BoxDto>> GetByIdAsync(int boxId, CancellationToken ct)
    {
        if (boxId < 1)
            return Result.Failure<BoxDto>(BoxErrors.Failure("El Id debe ser mayor que cero."));

        try
        {
            var _context = await _contextFactory.CreateDbContextAsync(ct);

            var boxDto = await _context.Boxes
                .AsNoTracking()
                .Where(b => b.Id == boxId)
                .Select(b => new BoxDto
                {
                    Id = b.Id,
                    TotalProducts = b.Products.Count(),
                    Location = b.Location,
                    Code = b.Code,
                    CreatedAt = b.CreatedAt,
                    UpdatedAt = b.UpdatedAt
                })
                .FirstOrDefaultAsync(ct);

            if (boxDto == null)
                return Result.Failure<BoxDto>(BoxErrors.NotFound(boxId));

            return Result.Success(boxDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while trying to retrieve the box with ID {boxId}.", boxId);
            return Result.Failure<BoxDto>(BoxErrors.Failure($"Ocurrió un error inesperado al intentar recuperar la caja."));
        }
    }

    public async Task<Result> AddProductsToBoxAsync(int boxId, List<int> productIds, CancellationToken ct)
    {
        if (boxId < 1)
        {
            _logger.LogError("Box ID: '{boxId}' is invalid.", boxId);
            return Result.Failure<BoxDto>(BoxErrors.Failure("El Id debe ser mayor que cero."));
        }

        if (productIds.Any(id => id < 1))
        {
            _logger.LogError("Product IDs: '{productsIds}' are invalid.", string.Join(", ", productIds));
            return Result.Failure<BoxDto>(BoxErrors.Failure("Ids de producto inválido."));
        }

        try
        {
            var context = await _contextFactory.CreateDbContextAsync(ct);

            var box = await context.Boxes
                .SingleOrDefaultAsync(b => b.Id == boxId, ct);

            if (box is null)
                return Result.Failure(BoxErrors.NotFound(boxId));

            var products = await context.Products
                .Where(p => productIds.Contains(p.Id))
                .ToListAsync(ct);

            if (products is null || products.Count == 0)
                return Result.Failure(ProductErrors.Failure("No se encontraron productos."));

            foreach (var product in products)
            {
                product.Box = box;
            }

            var saved = await context.SaveChangesAsync(ct);
            if (saved > 0)
            {
                return Result.Success();
            }

            return Result.Failure(BoxErrors.Failure("No se pudieron agregar los productos a la caja."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while trying to add products to the box Id {boxId}.", boxId);
            return Result.Failure(BoxErrors.Failure($"Ocurrió un error inesperado al intentar agregar productos a la caja con ID {boxId}."));
        }
    }

    public async Task<Result<int>> CreateAsync(CreateBoxDto boxDto, CancellationToken ct)
    {
        var validator = new CreateBoxDtoValidator();
        var validationResult = await validator.ValidateAsync(boxDto, ct);

        if (!validationResult.IsValid)
            return Result.Failure<int>(BoxErrors.Failure(validationResult.ToString()));

        Box box = new()
        {
            Location = boxDto.Location
        };

        try
        {
            var _context = await _contextFactory.CreateDbContextAsync(ct);

            var entity = await _context.Boxes.AddAsync(box, ct);
            var saved = await _context.SaveChangesAsync(ct);

            if (saved > 0)
            {
                return Result.Success(entity.Entity.Id);
            }
            return Result.Failure<int>(BoxErrors.Failure("No se pudo crear la caja."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while trying to create the box.");
            return Result.Failure<int>(BoxErrors.Failure($"Ocurrió un error inesperado al intentar crear la caja."));
        }
    }

    public async Task<Result> EditBoxById(int boxId, EditBoxDto editBoxDto, CancellationToken ct)
    {
        if (boxId < 1)
            return Result.Failure(BoxErrors.Failure("El Id debe ser mayor que cero."));

        var validator = new EditBoxDtoValidator();
        var validationResult = await validator.ValidateAsync(editBoxDto, ct);

        if (!validationResult.IsValid)
            return Result.Failure(BoxErrors.Failure(validationResult.ToString()));

        try
        {
            var _context = await _contextFactory.CreateDbContextAsync(ct);

            Box? box = await _context.Boxes.FirstOrDefaultAsync(b => b.Id == boxId, ct);

            if (box == null)
            {
                return Result.Failure(BoxErrors.NotFound(boxId));
            }

            box.Location = editBoxDto.Location;
            
            await _context.SaveChangesAsync(ct);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while trying to edit the box with ID {boxId}.", boxId);
            return Result.Failure(BoxErrors.Failure($"Ocurrió un error inesperado al intentar editar la caja."));
        }
    }

    public async Task<Result> DeleteBoxById(int boxId, CancellationToken ct)
    {
        if (boxId < 1)
            return Result.Failure(BoxErrors.Failure("El Id debe ser mayor que cero."));

        try
        {
            var _context = await _contextFactory.CreateDbContextAsync(ct);

            var box = await _context.Boxes
                .Include(b => b.Products)
                .FirstOrDefaultAsync(b => b.Id == boxId, ct);

            if (box is null)
                return Result.Failure(BoxErrors.NotFound(boxId));

            if (box.Products.Count > 0)
            {
                return Result.Failure(BoxErrors.Failure("No se puede borrar una caja que contiene productos."));
            }

            _context.Boxes.Remove(box);
            await _context.SaveChangesAsync(ct);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while trying to delete the box with ID {boxId}", boxId);
            return Result.Failure(BoxErrors.Failure($"Ocurrió un error inesperado al intentar borrar la caja."));
        }
    }
}