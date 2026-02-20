using TodoSeUsa.Application.Features.Overview.DTOs;
using TodoSeUsa.Application.Features.Sales;

namespace TodoSeUsa.Application.Features.Overview;

public class OverviewService : IOverviewService
{
    private readonly ILogger<OverviewService> _logger;
    private readonly IApplicationDbContextFactory _contextFactory;

    public OverviewService(ILogger<OverviewService> logger, IApplicationDbContextFactory contextFactory)
    {
        _logger = logger;
        _contextFactory = contextFactory;
    }

    public async Task<Result<IReadOnlyList<SalePointDto>>> GetSalesAsync(int year, int month, CancellationToken ct)
    {
        if (year < 1 || month < 1 || month > 12)
        {
            return Result.Failure<IReadOnlyList<SalePointDto>>(
                SaleErrors.Failure("El año o el mes proporcionado no es válido.")
            );
        }

        try
        {
            var context = await _contextFactory.CreateDbContextAsync(ct);

            var data = await context.Sales
                .Where(s => s.DateIssued.Year == year && s.DateIssued.Month == month)
                .GroupBy(s => s.DateIssued.Day)
                .Select(g => new SalePointDto
                {
                    Day = g.Key,
                    Total = g.Sum(x => x.TotalAmount)
                })
                .OrderBy(x => x.Day)
                .ToListAsync(ct);

            return Result.Success<IReadOnlyList<SalePointDto>>(data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving sales data for {year}/{month}.", year, month);

            return Result.Failure<IReadOnlyList<SalePointDto>>(
                SaleErrors.Failure("Ocurrió un error inesperado al recuperar las ventas.")
            );
        }
    }

    public async Task<Result<int>> GetYearlyTotalCountAsync(int year, CancellationToken ct)
    {
        if (year < 1)
            return Result.Failure<int>(SaleErrors.Failure("El año proporcionado no es válido."));

        try
        {
            var context = await _contextFactory.CreateDbContextAsync(ct);

            var count = await context.Sales
                .CountAsync(s => s.DateIssued.Year == year, ct);

            return Result.Success(count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error counting yearly sales for {year}.", year);

            return Result.Failure<int>(
                SaleErrors.Failure("Ocurrió un error inesperado al contar las ventas del año.")
            );
        }
    }

    public async Task<Result<decimal>> GetYearlyRevenueAsync(int year, CancellationToken ct)
    {
        if (year < 1)
            return Result.Failure<decimal>(SaleErrors.Failure("El año proporcionado no es válido."));

        try
        {
            var context = await _contextFactory.CreateDbContextAsync(ct);

            var revenue = await context.Sales
                .Where(s => s.DateIssued.Year == year)
                .SumAsync(s => s.TotalAmount, ct);

            return Result.Success(revenue);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating yearly revenue for {year}.", year);

            return Result.Failure<decimal>(
                SaleErrors.Failure("Ocurrió un error inesperado al calcular la facturación del año.")
            );
        }
    }

    public async Task<Result<IReadOnlyList<MonthlySalePointDto>>> GetMonthlySalesAsync(int year, CancellationToken ct)
    {
        if (year < 1)
            return Result.Failure<IReadOnlyList<MonthlySalePointDto>>(
                SaleErrors.Failure("El año proporcionado no es válido.")
            );

        try
        {
            var context = await _contextFactory.CreateDbContextAsync(ct);

            var data = await context.Sales
                .Where(s => s.DateIssued.Year == year)
                .GroupBy(s => s.DateIssued.Month)
                .Select(g => new MonthlySalePointDto
                {
                    Month = g.Key,
                    Total = g.Sum(x => x.TotalAmount)
                })
                .OrderBy(x => x.Month)
                .ToListAsync(ct);

            return Result.Success<IReadOnlyList<MonthlySalePointDto>>(data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving monthly sales for {year}.", year);

            return Result.Failure<IReadOnlyList<MonthlySalePointDto>>(
                SaleErrors.Failure("Ocurrió un error inesperado al recuperar las ventas mensuales.")
            );
        }
    }

    public async Task<Result<IReadOnlyList<MonthlyCountPointDto>>> GetMonthlyCountAsync(int year, CancellationToken ct)
    {
        if (year < 1)
            return Result.Failure<IReadOnlyList<MonthlyCountPointDto>>(
                SaleErrors.Failure("El año proporcionado no es válido.")
            );

        try
        {
            var context = await _contextFactory.CreateDbContextAsync(ct);

            var data = await context.Sales
                .Where(s => s.DateIssued.Year == year)
                .GroupBy(s => s.DateIssued.Month)
                .Select(g => new MonthlyCountPointDto
                {
                    Month = g.Key,
                    Count = g.Count()
                })
                .OrderBy(x => x.Month)
                .ToListAsync(ct);

            return Result.Success<IReadOnlyList<MonthlyCountPointDto>>(data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving monthly count for {year}.", year);

            return Result.Failure<IReadOnlyList<MonthlyCountPointDto>>(
                SaleErrors.Failure("Ocurrió un error inesperado al recuperar el volumen mensual de ventas.")
            );
        }
    }

    public async Task<Result<int>> GetTotalCountAsync(int year, int month, CancellationToken ct)
    {
        if (year < 1 || month < 1 || month > 12)
        {
            return Result.Failure<int>(
                SaleErrors.Failure("El año o el mes proporcionado no es válido.")
            );
        }

        try
        {
            var context = await _contextFactory.CreateDbContextAsync(ct);

            var count = await context.Sales
                .CountAsync(s => s.DateIssued.Year == year && s.DateIssued.Month == month, ct);

            return Result.Success(count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error counting sales for {year}/{month}.", year, month);

            return Result.Failure<int>(
                SaleErrors.Failure("Ocurrió un error inesperado al contar las ventas.")
            );
        }
    }

    public async Task<Result<decimal>> GetRevenueAsync(int year, int month, CancellationToken ct)
    {
        if (year < 1 || month < 1 || month > 12)
        {
            return Result.Failure<decimal>(
                SaleErrors.Failure("El año o el mes proporcionado no es válido.")
            );
        }

        try
        {
            var context = await _contextFactory.CreateDbContextAsync(ct);

            var revenue = await context.Sales
                .Where(s => s.DateIssued.Year == year && s.DateIssued.Month == month)
                .SumAsync(s => s.TotalAmount, ct);

            return Result.Success(revenue);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating revenue for {year}/{month}.", year, month);

            return Result.Failure<decimal>(
                SaleErrors.Failure("Ocurrió un error inesperado al calcular la facturación.")
            );
        }
    }
}
