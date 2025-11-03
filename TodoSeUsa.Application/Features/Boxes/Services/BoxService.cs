using System.Linq.Dynamic.Core;
using TodoSeUsa.Application.Features.Boxes.DTOs;
using TodoSeUsa.Application.Features.Boxes.Interfaces;

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
    
    public async Task<Result<bool>> CreateBoxAsync(CreateBoxDto boxDto, CancellationToken ct)
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

    public async Task<Result<PagedItems<BoxDto>>> GetBoxesWithPaginationAsync(QueryItem request, CancellationToken cancellationToken)
    {
        try
        {
            IQueryable<Box> query = _context.Boxes
                .Include(b => b.Products)
                .AsQueryable()
                .AsNoTracking();

            query = query.ApplyFilter(request.Filter);
            query = query.ApplySorting(request.OrderBy, ApplyCustomSorting);

            var count = await query.CountAsync(cancellationToken);

            var boxesDtos = await query
                .Skip(request.Skip)
                .Take(request.Take)
                .Select(b => new BoxDto
                {
                    Id = b.Id,
                    TotalProducts = b.Products.Count,
                    Location = b.Location,
                    BoxCode = $"BOX-{b.Id:D5}",
                    CreatedAt = b.CreatedAt,
                    UpdatedAt = b.UpdatedAt,
                })
                .ToListAsync(cancellationToken);

            var pagedItems = new PagedItems<BoxDto>() { Items = boxesDtos, Count = count };

            return Result.Success(pagedItems);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving paged boxes.");

            return Result.Failure<PagedItems<BoxDto>>(BoxErrors.Failure());
        }
    }

    public async Task<Result<BoxDto>> GetByIdAsync(int boxId, CancellationToken cancellationToken)
    {
        try
        {
            var box = await _context.Boxes
                .Include(b => b.Products)
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == boxId, cancellationToken);
            if (box == null)
            {
                return Result.Failure<BoxDto>(BoxErrors.NotFound(boxId));
            }
            var boxDto = new BoxDto
            {
                Id = box.Id,
                TotalProducts = box.Products.Count,
                Location = box.Location,
                BoxCode = $"BOX-{box.Id:D5}",
                CreatedAt = box.CreatedAt
            };
            return Result.Success(boxDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving the box with ID {BoxId}.", boxId);
            return Result.Failure<BoxDto>(BoxErrors.Failure());
    public async Task<Result<bool>> EditBoxById(int boxId, EditBoxDto boxDto, CancellationToken cancellationToken)
    {
        try
        {
            Box? box = await _context.Boxes.FirstOrDefaultAsync(b => b.Id == boxId, cancellationToken);
            if (box == null)
            {
                return Result.Failure<bool>(BoxErrors.NotFound(boxId));
            }
            if (!string.IsNullOrWhiteSpace(boxDto.Location))
            {
                box.Location = boxDto.Location;
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

    private static IQueryable<Box>? ApplyCustomSorting(IQueryable<Box> query, string orderBy)
    {
        if (orderBy.StartsWith("BoxCode", StringComparison.OrdinalIgnoreCase))
        {
            return orderBy.EndsWith("desc", StringComparison.OrdinalIgnoreCase)
                ? query.OrderByDescending(b => b.Id)
                : query.OrderBy(b => b.Id);
        }

        if (orderBy.StartsWith("TotalProducts", StringComparison.OrdinalIgnoreCase))
        {
            return orderBy.EndsWith("desc", StringComparison.OrdinalIgnoreCase)
                ? query.OrderByDescending(b => b.Products.Count)
                : query.OrderBy(b => b.Products.Count);
        }
        return null;
    }

}
