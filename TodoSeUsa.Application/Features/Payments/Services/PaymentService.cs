using TodoSeUsa.Application.Features.Payments.DTOs;
using TodoSeUsa.Application.Features.Payments.Interfaces;

namespace TodoSeUsa.Application.Features.Payments.Services;

public sealed class PaymentService : IPaymentService
{
    private readonly IApplicationDbContextFactory _contextFactory;
    private readonly ILogger<PaymentService> _logger;

    public PaymentService(IApplicationDbContextFactory contextFactory, ILogger<PaymentService> logger)
    {
        _contextFactory = contextFactory;
        _logger = logger;
    }

    public async Task<Result<PagedItems<PaymentDto>>> GetPaymentsWithPagination(QueryItem request, CancellationToken ct)
    {
        try
        {
            var _context = await _contextFactory.CreateDbContextAsync(ct);

            IQueryable<Payment> query = _context.Payments
                .AsQueryable()
                .AsNoTracking();

            query = query.ApplyFilter(request.Filter);
            // query = query.ApplySorting(request.OrderBy);

            var count = await query.CountAsync(ct);

            var paymentsDtos = await query
                .Skip(request.Skip)
                .Take(request.Take)
                .Select(b => new PaymentDto
                {
                    Id = b.Id,
                    Amount = b.Amount,
                    Date = b.Date,
                    Method = b.Method,
                    SaleId = b.SaleId,
                    CreatedAt = b.CreatedAt
                })
                .ToListAsync(ct);

            var pagedItems = new PagedItems<PaymentDto>() { Items = paymentsDtos, Count = count };

            return Result.Success(pagedItems);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving paged payments.");

            return Result.Failure<PagedItems<PaymentDto>>(PaymentErrors.Failure());
        }
    }
}