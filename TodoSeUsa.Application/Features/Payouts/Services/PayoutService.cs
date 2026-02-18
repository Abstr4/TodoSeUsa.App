using System.Linq.Expressions;
using TodoSeUsa.Application.Features.Payouts.DTOs;
using TodoSeUsa.Application.Features.Payouts.Interfaces;
using TodoSeUsa.Domain.Enums;

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

    public async Task<int> CountPendingNotStartedAsync(CancellationToken ct)
    {
        var context = await _contextFactory.CreateDbContextAsync(ct);

        return await PendingQuery(context)
            .Where(x => x.AmountPaidOut == 0)
            .CountAsync(ct);
    }

    public async Task<int> CountPendingStartedAsync(CancellationToken ct)
    {
        var context = await _contextFactory.CreateDbContextAsync(ct);

        return await PendingQuery(context)
            .Where(x => x.AmountPaidOut > 0)
            .CountAsync(ct);
    }

    public async Task<Result<List<PendingLiquidationGroupDto>>> GetPendingGroupedAsync(CancellationToken ct)
    {
        var context = await _contextFactory.CreateDbContextAsync(ct);

        var data = await context.SaleItems
            .AsNoTracking()
            .Where(p => p.Product != null &&
                        p.Product.PayoutDate == null &&
                        p.ReturnedAt == null &&
                        p.Sale.Status != SaleStatus.Cancelled &&
                        (p.Price * p.ConsignorPercent) > p.AmountPaidOut)
            .Select(x => new
            {
                x.Id,
                x.ProductId,
                x.Description,
                Remaining = (x.Price * x.ConsignorPercent) - x.AmountPaidOut,
                x.CreatedAt,
                ConsignorId = x.Product!.Consignment.ConsignorId,
                ConsignorName = x.Product.Consignment.Consignor.Person.FullName,
                HasPartialPayments =
                    x.Sale.AmountPaid > 0 &&
                    x.Sale.AmountPaid < x.Sale.TotalAmount
            })
            .ToListAsync(ct);

        var result = data
            .GroupBy(x => new { x.ConsignorId, x.ConsignorName })
            .Select(g => new PendingLiquidationGroupDto
            {
                ConsignorId = g.Key.ConsignorId,
                ConsignorName = g.Key.ConsignorName,
                TotalPending = g.Sum(x => x.Remaining),
                ItemsCount = g.Count(),
                Items = g.Select(x => new PendingLiquidationItemDto
                {
                    SaleItemId = x.Id,
                    ProductId = x.ProductId!.Value,
                    Description = x.Description,
                    Remaining = x.Remaining,
                    SoldAt = x.CreatedAt,
                    HasPartialPayments = x.HasPartialPayments
                }).ToList()
            })
            .OrderByDescending(x => x.TotalPending)
            .ToList();

        return Result.Success(result);
    }

    private static IQueryable<SaleItem> PendingQuery(IApplicationDbContext context)
    {
        return context.SaleItems
            .AsNoTracking()
            .Where(si =>
                si.ProductId != null &&
                si.Product != null &&
                si.Product.PayoutDate == null &&
                si.ReturnedAt == null &&
                si.Sale.Status != SaleStatus.Cancelled &&
                (si.Price * si.ConsignorPercent / 100m) > si.AmountPaidOut);
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
            .Include(si => si.Sale)
                .ThenInclude(s => s.Items)
            .Include(si => si.Sale)
                .ThenInclude(s => s.Payments)
            .ToListAsync(ct);

        if (saleItems.Count == 0)
        {
            _logger.LogWarning("No sale items found for payout creation. ConsignorId: {ConsignorId}, SaleItemIds: {SaleItemIds}",
                dto.ConsignorId, string.Join(", ", dto.SaleItemIds));
            return Result.Failure<int>(PayoutErrors.SaleItemsNotFound());
        }

        var payout = BuildPayout(dto.ConsignorId, saleItems);

        if (payout.Lines.Count == 0)
            return Result.Failure<int>(PayoutErrors.NothingToPay());

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
        var lines = items
            .Select(CreateLine)
            .Where(l => l.AmountPaid > 0)
            .ToList();

        var payout = new Payout
        {
            ConsignorId = consignorId,
            CreatedAt = DateTime.UtcNow,
            Lines = lines,
            TotalAmount = lines.Sum(x => x.AmountPaid)
        };

        ApplyStateChanges(items, lines);

        return payout;
    }

    private static void ApplyStateChanges(List<SaleItem> items, List<PayoutLine> lines)
    {
        var map = lines.ToDictionary(x => x.SaleItemId, x => x.AmountPaid);

        foreach (var item in items)
        {
            if (!map.TryGetValue(item.Id, out var amount))
                continue;

            item.AmountPaidOut += amount;

            if (item.Product is { PayoutDate: null })
                item.Product.PayoutDate = DateTime.UtcNow;
        }
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
        var ratio = CalculateRatio(item.Sale);

        if (ratio <= 0)
            return 0;

        var payable = item.Price * ratio * item.ConsignorPercent;

        var remaining = payable - item.AmountPaidOut;

        return remaining <= 0 ? 0 : remaining;
    }

    private static decimal CalculateRatio(Sale sale)
    {
        var total = sale.Items.Sum(i => i.Price);
        if (total <= 0)
            return 0;

        var paid = sale.Payments
            .Where(p => p.RefundedAt == null)
            .Sum(p => p.Amount);

        var ratio = paid / total;

        return ratio > 1 ? 1 : ratio;
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
