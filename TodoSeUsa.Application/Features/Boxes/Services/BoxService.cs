using System.Linq.Dynamic.Core;
using TodoSeUsa.Application.Common.Enums;
using TodoSeUsa.Application.Common.Services;
using TodoSeUsa.Application.Common.Validators;
using TodoSeUsa.Application.Features.Boxes.DTOs;
using TodoSeUsa.Application.Features.Boxes.Interfaces;
using TodoSeUsa.Application.Features.Boxes.Validators;
using TodoSeUsa.Application.Features.Products.DTOs;

namespace TodoSeUsa.Application.Features.Boxes.Services;
public class BoxService : IBoxService
{
    private readonly ILogger<BoxService> _logger;
    private readonly IApplicationDbContext _context;

    public BoxService(ILogger<BoxService> logger, IApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }
    
    public async Task<Result<PagedItems<BoxDto>>> GetAllAsync(
        QueryRequest request,
        CancellationToken cancellationToken)
    {
        var query = _context.Boxes.AsQueryable();

        if (request.Filters != null && request.Filters.Count > 0)
        {
            var predicate = PredicateBuilder.BuildPredicate<Box>(request);
            query = query.Where(predicate);
        }

        if (request.Sorts != null && request.Sorts.Count != 0)
        {
            query = ApplyCustomSorting(query, request.Sorts);
        }
        else
        {
            query = query.OrderBy(x => x.Id);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        query = query.Skip(request.Skip).Take(request.Take);

        var items = await query
            .Select(b => new BoxDto
            {
                Id = b.Id,
                TotalProducts = b.Products.Count(),
                Location = b.Location,
                BoxCode = $"BOX-{b.Id:D5}",
                CreatedAt = b.CreatedAt,
                UpdatedAt = b.UpdatedAt,
            })
        .ToListAsync(cancellationToken);

        return Result.Success(new PagedItems<BoxDto>
        {
            Items = items,
            Count = totalCount
        });
    }

    public async Task<Result<BoxDto>> GetByIdAsync(int boxId, CancellationToken cancellationToken)
    {
        if (boxId <= 0)
            return Result.Failure<BoxDto>(BoxErrors.Failure("El Id debe ser mayor que cero."));

        try
        {
            var boxDto = await _context.Boxes
                .AsNoTracking()
                .Where(b => b.Id == boxId)
                .Select(b => new BoxDto
                {
                    Id = b.Id,
                    TotalProducts = b.Products.Count(),
                    Location = b.Location,
                    BoxCode = $"BOX-{b.Id:D5}",
                    CreatedAt = b.CreatedAt,
                    UpdatedAt = b.UpdatedAt
                })
                .FirstOrDefaultAsync(cancellationToken);

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

    public async Task<Result<bool>> CreateAsync(CreateBoxDto boxDto, CancellationToken ct)
    {
        var validator = new CreateBoxDtoValidator();
        var validationResult = await validator.ValidateAsync(boxDto, ct);

        if (!validationResult.IsValid)
            return Result.Failure<bool>(BoxErrors.Failure(validationResult.ToString()));

        Box box = new() 
        {
            Location = boxDto.Location
        };

        try
        {
            await _context.Boxes.AddAsync(box, ct);
            await _context.SaveChangesAsync(ct);
            return Result.Success(true);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while trying to create the box.");
            return Result.Failure<bool>(BoxErrors.Failure($"Ocurrió un error inesperado al intentar crear la caja."));
        }

    }

    public async Task<Result<bool>> EditBoxById(int boxId, EditBoxDto editBoxDto, CancellationToken cancellationToken)
    {
        if (boxId <= 0)
            return Result.Failure<bool>(BoxErrors.Failure("El Id debe ser mayor que cero."));

        var validator = new EditBoxDtoValidator();
        var validationResult = await validator.ValidateAsync(editBoxDto, cancellationToken);

        if (!validationResult.IsValid)
            return Result.Failure<bool>(BoxErrors.Failure(validationResult.ToString()));

        try
        {
            Box? box = await _context.Boxes.FirstOrDefaultAsync(b => b.Id == boxId, cancellationToken);

            if (box == null)
            {
                return Result.Failure<bool>(BoxErrors.NotFound(boxId));
            }
            if (!string.IsNullOrWhiteSpace(editBoxDto.Location))
            {
                box.Location = editBoxDto.Location;
            }
            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while trying to edit the box with ID {boxId}.", boxId);
            return Result.Failure<bool>(BoxErrors.Failure($"Ocurrió un error inesperado al intentar editar la caja."));
        }
    }

    public async Task<Result<bool>> DeleteBoxById(int boxId, CancellationToken cancellationToken)
    {
        if (boxId <= 0)
            return Result.Failure<bool>(BoxErrors.Failure("El Id debe ser mayor que cero."));

        try
        {
            var box = await _context.Boxes
                .FirstOrDefaultAsync(b => b.Id == boxId, cancellationToken);

            if (box is null)
                return Result.Failure<bool>(BoxErrors.NotFound(boxId));

            if(box.Products.Count > 0)
            {
                return Result.Failure<bool>(BoxErrors.Failure("No se puede borrar una caja que contiene productos."));
            }

            _context.Boxes.Remove(box);
            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while trying to delete the box with ID {boxId}", boxId);
            return Result.Failure<bool>(BoxErrors.Failure($"Ocurrió un error inesperado al intentar borrar la caja."));
        }
    }

    public static IQueryable<Box> ApplyCustomSorting(IQueryable<Box> query, IEnumerable<SortDescriptor>? sorts)
    {
        var sort = sorts?.FirstOrDefault();
        if (sort == null || string.IsNullOrWhiteSpace(sort.Property))
            return query;

        var property = sort.Property;
        var isDescending = sort.SortOrder == SortOrder.Descending;

        if (property.Equals("BoxCode", StringComparison.OrdinalIgnoreCase))
        {
            return isDescending
                ? query.OrderByDescending(b => b.Id)
                : query.OrderBy(b => b.Id);
        }

        if (property.Equals("TotalProducts", StringComparison.OrdinalIgnoreCase))
        {
            return isDescending
                ? query.OrderByDescending(b => b.Products.Count)
                : query.OrderBy(b => b.Products.Count);
        }
        return isDescending
            ? query.OrderByDescending(e => EF.Property<object>(e, property))
            : query.OrderBy(e => EF.Property<object>(e, property));
    }
}
