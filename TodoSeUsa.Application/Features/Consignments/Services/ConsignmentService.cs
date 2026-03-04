using System.Linq.Dynamic.Core;
using TodoSeUsa.Application.Common.Querying.CustomCases;
using TodoSeUsa.Application.Features.Consignments.DTOs;
using TodoSeUsa.Application.Features.Consignments.Interfaces;
using TodoSeUsa.Application.Features.Consignments.Validators;

namespace TodoSeUsa.Application.Features.Consignments.Services;

public sealed class ConsignmentService : IConsignmentService
{
    private readonly ILogger<ConsignmentService> _logger;
    private readonly IApplicationDbContextFactory _contextFactory;
    private readonly UniqueConsignmentCodeService _uniqueCodeService;

    public ConsignmentService(ILogger<ConsignmentService> logger, IApplicationDbContextFactory contextFactory, UniqueConsignmentCodeService uniqueCodeService)
    {
        _logger = logger;
        _contextFactory = contextFactory;
        _uniqueCodeService = uniqueCodeService;
    }

    public async Task<Result<PagedItems<ConsignmentDto>>> GetAllAsync(QueryRequest request, CancellationToken ct)
    {
        await using var _context = await _contextFactory.CreateDbContextAsync(ct);

        var query = _context.Consignments
            .Include(c => c.Consignor)
                .ThenInclude(p => p.Person)
            .AsQueryable();

        query = QueryableExtensions.ApplyCustomFiltering(query, request.Filters, request.LogicalFilterOperator, QueryFilteringCases.ConsignmentFilters);

        query = QueryableExtensions.ApplyCustomSorting(query, request.Sorts, QuerySortingCases.ConsignmentSorts);

        var totalCount = await query.CountAsync(ct);

        query = query.Skip(request.Skip).Take(request.Take);

        var items = await query
            .Select(c => new ConsignmentDto
            {
                Id = c.Id,
                Code = c.PublicId,
                TotalProducts = c.Products.Count,
                ConsignorFirstName = c.Consignor.Person.FirstName,
                ConsignorLastName = c.Consignor.Person.LastName,
                DateIssued = c.DateIssued,
                Notes = c.Notes,
                ConsignorId = c.ConsignorId,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt,
            })
        .ToListAsync(ct);

        return Result.Success(new PagedItems<ConsignmentDto>
        {
            Items = items,
            Count = totalCount
        });
    }

    public async Task<Result<PagedItems<ConsignmentDto>>> GetByConsignorIdAsync(QueryRequest request, int consignorId, CancellationToken ct)
    {
        await using var _context = await _contextFactory.CreateDbContextAsync(ct);

        var query = _context.Consignments
            .Include(c => c.Consignor)
                .ThenInclude(p => p.Person)
            .Where(c => c.ConsignorId == consignorId);

        query = QueryableExtensions.ApplyCustomFiltering(query, request.Filters, request.LogicalFilterOperator, QueryFilteringCases.ConsignmentFilters);

        query = QueryableExtensions.ApplyCustomSorting(query, request.Sorts, QuerySortingCases.ConsignmentSorts);

        var totalCount = await query.CountAsync(ct);

        query = query.Skip(request.Skip).Take(request.Take);

        var items = await query
            .Select(c => new ConsignmentDto
            {
                Id = c.Id,
                Code = c.PublicId,
                TotalProducts = c.Products.Count,
                ConsignorFirstName = c.Consignor.Person.FirstName,
                ConsignorLastName = c.Consignor.Person.LastName,
                DateIssued = c.DateIssued,
                Notes = c.Notes,
                ConsignorId = consignorId,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt,
            })
        .ToListAsync(ct);

        return Result.Success(new PagedItems<ConsignmentDto>
        {
            Items = items,
            Count = totalCount
        });
    }

    public async Task<Result<ConsignmentDto>> GetByIdAsync(int consignmentId, CancellationToken ct)
    {
        if (consignmentId <= 0)
            return Result.Failure<ConsignmentDto>(ConsignmentErrors.Failure("El Id debe ser mayor que cero."));

        try
        {
            await using var _context = await _contextFactory.CreateDbContextAsync(ct);

            var consignmentDto = await _context.Consignments
                .Where(c => c.Id == consignmentId)
                .Select(c => new ConsignmentDto
                {
                    Id = c.Id,
                    Code = c.PublicId,
                    TotalProducts = c.Products.Count,
                    ConsignorId = c.ConsignorId,
                    ConsignorFirstName = c.Consignor.Person.FirstName,
                    ConsignorLastName = c.Consignor.Person.LastName,
                    DateIssued = c.DateIssued,
                    Notes = c.Notes,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt
                })
                .AsNoTracking()
                .FirstOrDefaultAsync(ct);

            if (consignmentDto == null)
                return Result.Failure<ConsignmentDto>(ConsignmentErrors.NotFound(consignmentId));

            return Result.Success(consignmentDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while trying to retrieve the consignment with ID {consignmentId}.", consignmentId);
            return Result.Failure<ConsignmentDto>(ConsignmentErrors.Failure("Ocurrió un error inesperado al intentar recuperar la consignación."));
        }
    }

    public async Task<Result<int>> CreateAsync(CreateConsignmentDto createConsignmentDto, CancellationToken ct)
    {
        var validator = new CreateConsignmentDtoValidator();
        var validationResult = await validator.ValidateAsync(createConsignmentDto, ct);

        if (!validationResult.IsValid)
            return Result.Failure<int>(ConsignmentErrors.Failure(validationResult.ToString()));

        var code = await _uniqueCodeService.GenerateAsync(ct);

        Consignment consignment = new()
        {
            PublicId = code,
            DateIssued = createConsignmentDto.DateIssued ?? DateTime.Now,
            Notes = createConsignmentDto.Notes,
            ConsignorId = createConsignmentDto.ConsignorId,
            CreatedAt = DateTime.Now
        };

        try
        {
            await using var _context = await _contextFactory.CreateDbContextAsync(ct);

            var entry = await _context.Consignments.AddAsync(consignment, ct);

            var saved = await _context.SaveChangesAsync(ct);

            if (saved > 0)
                return Result.Success(entry.Entity.Id);

            return Result.Failure<int>(ConsignmentErrors.Failure("No se pudo crear la consignación."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while trying to create the consignment.");
            return Result.Failure<int>(ConsignmentErrors.Failure($"Ocurrió un error inesperado al intentar crear la consignación."));
        }
    }

    public async Task<Result> DeleteByIdAsync(int consignmentId, CancellationToken ct)
    {
        if (consignmentId < 1)
            return Result.Failure<bool>(ConsignmentErrors.Failure("El Id debe ser mayor que cero."));

        try
        {
            await using var _context = await _contextFactory.CreateDbContextAsync(ct);

            var consignment = await _context.Consignments
                .Include(b => b.Products)
                .FirstOrDefaultAsync(b => b.Id == consignmentId, ct);

            if (consignment is null)
                return Result.Failure<bool>(ConsignmentErrors.NotFound(consignmentId));

            if (consignment.Products.Count > 0)
            {
                return Result.Failure<bool>(ConsignmentErrors.Failure("No se puede borrar una consignación que contiene productos."));
            }

            _context.Consignments.Remove(consignment);
            await _context.SaveChangesAsync(ct);
            return Result.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while trying to delete the consignment with ID {consignmentId}", consignmentId);
            return Result.Failure<bool>(ConsignmentErrors.Failure($"Ocurrió un error inesperado al intentar borrar la consignación."));
        }
    }

    public async Task<Result> EditByIdAsync(EditConsignmentDto editConsignmentDto, CancellationToken ct)
    {
        var validator = new EditConsignmentDtoValidator();
        var validationResult = await validator.ValidateAsync(editConsignmentDto, ct);

        if (!validationResult.IsValid)
            return Result.Failure<bool>(ConsignmentErrors.Failure(validationResult.ToString()));

        try
        {
            await using var _context = await _contextFactory.CreateDbContextAsync(ct);

            Consignment? consignment = await _context.Consignments.FirstOrDefaultAsync(b => b.Id == editConsignmentDto.Id, ct);
            if (consignment == null)
            {
                return Result.Failure<bool>(ConsignmentErrors.NotFound(editConsignmentDto.Id));
            }

            consignment.DateIssued = editConsignmentDto.DateIssued;
            consignment.Notes = editConsignmentDto.Notes;
            consignment.ConsignorId = editConsignmentDto.ConsignorId;

            await _context.SaveChangesAsync(ct);
            return Result.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while trying to edit the consignment with ID {Id}.", editConsignmentDto.Id);
            return Result.Failure<bool>(ConsignmentErrors.Failure($"Ocurrió un error inesperado al intentar editar la consignación."));
        }
    }
}