using System.Linq.Dynamic.Core;
using TodoSeUsa.Application.Common.Querying.CustomCases;
using TodoSeUsa.Application.Common.Services;
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

    public async Task<Result<PagedItems<ConsignmentDto>>> GetAllAsync(QueryRequest request, CancellationToken cancellationToken)
    {
        var _context = await _contextFactory.CreateDbContextAsync(cancellationToken);

        var query = _context.Consignments
            .Include(c => c.Consignor)
                .ThenInclude(p => p.Person)
            .AsQueryable();

        query = QueryableExtensions.ApplyCustomFiltering(query, request.Filters, request.LogicalFilterOperator, QueryFilteringCases.ConsignmentFilters);

        query = QueryableExtensions.ApplyCustomSorting(query, request.Sorts, QuerySortingCases.ConsignmentSorts);

        var totalCount = await query.CountAsync(cancellationToken);

        query = query.Skip(request.Skip).Take(request.Take);

        var items = await query
            .Select(c => new ConsignmentDto
            {
                Id = c.Id,
                Code = c.Code,
                TotalProducts = c.Products.Count,
                ConsignorFirstName = c.Consignor.Person.FirstName,
                ConsignorLastName = c.Consignor.Person.LastName,
                DateIssued = c.DateIssued,
                Notes = c.Notes,
                ConsignorId = c.ConsignorId,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt,
            })
        .ToListAsync(cancellationToken);

        return Result.Success(new PagedItems<ConsignmentDto>
        {
            Items = items,
            Count = totalCount
        });
    }

    public async Task<Result<PagedItems<ConsignmentDto>>> GetByConsignorIdAsync(QueryRequest request, int consignorId, CancellationToken cancellationToken)
    {
        var _context = await _contextFactory.CreateDbContextAsync(cancellationToken);

        var query = _context.Consignments
            .Include(c => c.Consignor)
                .ThenInclude(p => p.Person)
            .Where(c => c.ConsignorId == consignorId);

        query = QueryableExtensions.ApplyCustomFiltering(query, request.Filters, request.LogicalFilterOperator, QueryFilteringCases.ConsignmentFilters);

        query = QueryableExtensions.ApplyCustomSorting(query, request.Sorts, QuerySortingCases.ConsignmentSorts);

        var totalCount = await query.CountAsync(cancellationToken);

        query = query.Skip(request.Skip).Take(request.Take);

        var items = await query
            .Select(c => new ConsignmentDto
            {
                Id = c.Id,
                Code = c.Code,
                TotalProducts = c.Products.Count,
                ConsignorFirstName = c.Consignor.Person.FirstName,
                ConsignorLastName = c.Consignor.Person.LastName,
                DateIssued = c.DateIssued,
                Notes = c.Notes,
                ConsignorId = consignorId,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt,
            })
        .ToListAsync(cancellationToken);

        return Result.Success(new PagedItems<ConsignmentDto>
        {
            Items = items,
            Count = totalCount
        });
    }

    public async Task<Result<ConsignmentDto>> GetByIdAsync(int consignmentId, CancellationToken cancellationToken)
    {
        if (consignmentId <= 0)
            return Result.Failure<ConsignmentDto>(ConsignmentErrors.Failure("El Id debe ser mayor que cero."));

        try
        {
            var _context = await _contextFactory.CreateDbContextAsync(cancellationToken);

            var consignmentDto = await _context.Consignments
                .Where(c => c.Id == consignmentId)
                .Select(c => new ConsignmentDto
                {
                    Id = c.Id,
                    Code = c.Code,
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
                .FirstOrDefaultAsync(cancellationToken);

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

    public async Task<Result<int>> CreateAsync(CreateConsignmentDto createConsignmentDto, CancellationToken cancellationToken)
    {
        var validator = new CreateConsignmentDtoValidator();
        var validationResult = await validator.ValidateAsync(createConsignmentDto, cancellationToken);

        if (!validationResult.IsValid)
            return Result.Failure<int>(ConsignmentErrors.Failure(validationResult.ToString()));

        var code = await _uniqueCodeService.GenerateAsync(cancellationToken);

        Consignment consignment = new()
        {
            Code = code,
            DateIssued = createConsignmentDto.DateIssued ?? DateTime.Now,
            Notes = createConsignmentDto.Notes,
            ConsignorId = createConsignmentDto.ConsignorId,
            CreatedAt = DateTime.Now
        };

        try
        {
            var _context = await _contextFactory.CreateDbContextAsync(cancellationToken);

            var entry = await _context.Consignments.AddAsync(consignment, cancellationToken);

            var saved = await _context.SaveChangesAsync(cancellationToken);

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

    public async Task<Result> DeleteByIdAsync(int consignmentId, CancellationToken cancellationToken)
    {
        if (consignmentId < 1)
            return Result.Failure<bool>(ConsignmentErrors.Failure("El Id debe ser mayor que cero."));

        try
        {
            var _context = await _contextFactory.CreateDbContextAsync(cancellationToken);

            var consignment = await _context.Consignments
                .Include(b => b.Products)
                .FirstOrDefaultAsync(b => b.Id == consignmentId, cancellationToken);

            if (consignment is null)
                return Result.Failure<bool>(ConsignmentErrors.NotFound(consignmentId));

            if (consignment.Products.Count > 0)
            {
                return Result.Failure<bool>(ConsignmentErrors.Failure("No se puede borrar una consignación que contiene productos."));
            }

            _context.Consignments.Remove(consignment);
            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while trying to delete the consignment with ID {consignmentId}", consignmentId);
            return Result.Failure<bool>(ConsignmentErrors.Failure($"Ocurrió un error inesperado al intentar borrar la consignación."));
        }
    }

    public async Task<Result> EditByIdAsync(EditConsignmentDto editConsignmentDto, CancellationToken cancellationToken)
    {
        var validator = new EditConsignmentDtoValidator();
        var validationResult = await validator.ValidateAsync(editConsignmentDto, cancellationToken);

        if (!validationResult.IsValid)
            return Result.Failure<bool>(ConsignmentErrors.Failure(validationResult.ToString()));

        try
        {
            var _context = await _contextFactory.CreateDbContextAsync(cancellationToken);

            Consignment? consignment = await _context.Consignments.FirstOrDefaultAsync(b => b.Id == editConsignmentDto.Id, cancellationToken);
            if (consignment == null)
            {
                return Result.Failure<bool>(ConsignmentErrors.NotFound(editConsignmentDto.Id));
            }

            consignment.DateIssued = editConsignmentDto.DateIssued;
            consignment.Notes = editConsignmentDto.Notes;
            consignment.ConsignorId = editConsignmentDto.ConsignorId;

            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while trying to edit the consignment with ID {Id}.", editConsignmentDto.Id);
            return Result.Failure<bool>(ConsignmentErrors.Failure($"Ocurrió un error inesperado al intentar editar la consignación."));
        }
    }
}