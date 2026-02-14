using TodoSeUsa.Application.Features.Payouts.DTOs;
using TodoSeUsa.Application.Features.Payouts.Interfaces;

namespace TodoSeUsa.Application.Features.Payouts.Services;

public class PayoutService : IPayoutService
{
    private readonly ILogger<PayoutService> _logger;
    private readonly IApplicationDbContextFactory _contextFactory;

    public PayoutService(ILogger<PayoutService> logger, IApplicationDbContextFactory contextFactory)
    {
        _logger = logger;
        _contextFactory = contextFactory;
    }

    public async Task<Result<PagedItems<PayoutDto>>> GetAllAsync(QueryRequest request, CancellationToken ct)
    {
        var context = await _contextFactory.CreateDbContextAsync(ct);

        var query = context.Payouts
            .Include(p => p.Consignor)
                .ThenInclude(c => c.Person)
            .Include(p => p.Lines)
            .AsNoTracking()
            .AsQueryable();

        query = QueryableExtensions.ApplyCustomFiltering(
            query,
            request.Filters,
            request.LogicalFilterOperator);

        query = QueryableExtensions.ApplyCustomSorting(
            query,
            request.Sorts);

        var totalCount = await query.CountAsync(ct);

        query = query
            .Skip(request.Skip)
            .Take(request.Take);

        var items = await query
            .Select(p => new PayoutDto
            {
                Id = p.Id,
                ConsignorId = p.ConsignorId,
                ConsignorName = p.Consignor.Person.FullName,
                TotalAmount = p.TotalAmount,
                LinesCount = p.Lines.Count,
                CreatedAt = p.CreatedAt
            })
            .ToListAsync(ct);

        return Result.Success(new PagedItems<PayoutDto>
        {
            Items = items,
            Count = totalCount
        });
    }

    public async Task<Result<int>> CreateAsync(CreatePayoutDto dto, CancellationToken ct)
    {
        var context = await _contextFactory.CreateDbContextAsync(ct);

        var saleItems = await context.SaleItems
            .Where(si => dto.SaleItemIds.Contains(si.Id))
            .Include(si => si.Product)
            .ToListAsync(ct);

        if (saleItems.Count == 0)
        {
            _logger.LogWarning("No sale items found for payout creation. ConsignorId: {ConsignorId}, SaleItemIds: {SaleItemIds}",
                dto.ConsignorId, string.Join(", ", dto.SaleItemIds));
            return Result.Failure<int>(PayoutErrors.SaleItemsNotFound());
        }

        var payout = BuildPayout(dto.ConsignorId, saleItems);

        context.Payouts.Add(payout);

        await context.SaveChangesAsync(ct);

        return Result.Success(payout.Id);
    }

    public async Task<Result<PayoutDto>> GetByIdAsync(int payoutId, CancellationToken ct)
    {
        var context = await _contextFactory.CreateDbContextAsync(ct);

        var payout = await context.Payouts
            .Include(p => p.Consignor)
                .ThenInclude(c => c.Person)
            .Include(p => p.Lines)
                .ThenInclude(l => l.SaleItem)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == payoutId, ct);

        if (payout is null)
        {
            _logger.LogWarning("Payout not found. PayoutId: {PayoutId}", payoutId);
            return Result.Failure<PayoutDto>(PayoutErrors.NotFound(payoutId));
        }

        return Result.Success(MapToDto(payout));
    }

    public async Task<Result> DeleteByIdAsync(int payoutId, CancellationToken ct)
    {
        var context = await _contextFactory.CreateDbContextAsync(ct);

        var payout = await context.Payouts
            .Include(p => p.Lines)
            .FirstOrDefaultAsync(p => p.Id == payoutId, ct);

        if (payout is null)
        {
            _logger.LogWarning("Payout not found for deletion. PayoutId: {PayoutId}", payoutId);
            return Result.Failure(PayoutErrors.NotFound(payoutId));
        }

        context.PayoutLines.RemoveRange(payout.Lines);
        context.Payouts.Remove(payout);

        await context.SaveChangesAsync(ct);

        return Result.Success();
    }

    private static Payout BuildPayout(int consignorId, List<SaleItem> items)
    {
        var payout = new Payout
        {
            ConsignorId = consignorId,
            CreatedAt = DateTime.UtcNow
        };

        var lines = items.Select(CreateLine).ToList();

        payout.Lines = lines;
        payout.TotalAmount = lines.Sum(x => x.AmountPaid);

        foreach (var item in items)
        {
            item.AmountPaidOut += CalculateAmount(item);

            if (item.Product is not null && item.Product.PayoutDate == null)
                item.Product.PayoutDate = DateTime.UtcNow;
        }

        return payout;
    }

    private static PayoutLine CreateLine(SaleItem item)
    {
        return new PayoutLine
        {
            SaleItemId = item.Id,
            AmountPaid = CalculateAmount(item)
        };
    }

    private static decimal CalculateAmount(SaleItem item)
    {
        var totalDue = item.Price * item.ConsignorPercent;
        var remaining = totalDue - item.AmountPaidOut;
        return remaining < 0 ? 0 : remaining;
    }

    private static PayoutDto MapToDto(Payout payout)
    {
        return new PayoutDto
        {
            Id = payout.Id,
            ConsignorId = payout.ConsignorId,
            TotalAmount = payout.TotalAmount,
            CreatedAt = payout.CreatedAt,
            Lines = payout.Lines.Select(l => new PayoutLineDto
            {
                SaleItemId = l.SaleItemId,
                Amount = l.AmountPaid
            }).ToList()
        };
    }
}
