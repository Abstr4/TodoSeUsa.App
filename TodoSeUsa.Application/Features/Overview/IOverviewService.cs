using TodoSeUsa.Application.Features.Overview.DTOs;

namespace TodoSeUsa.Application.Features.Overview;

public interface IOverviewService
{
    Task<Result<int>> GetTotalCountAsync(int year, int month, CancellationToken ct);

    Task<Result<decimal>> GetRevenueAsync(int year, int month, CancellationToken ct);

    Task<Result<IReadOnlyList<SalePointDto>>> GetSalesAsync(int year, int month, CancellationToken ct);

    Task<Result<int>> GetYearlyTotalCountAsync(int year, CancellationToken ct);

    Task<Result<decimal>> GetYearlyRevenueAsync(int year, CancellationToken ct);

    Task<Result<IReadOnlyList<MonthlyCountPointDto>>> GetMonthlyCountAsync(int year, CancellationToken ct);

    Task<Result<IReadOnlyList<MonthlySalePointDto>>> GetMonthlySalesAsync(int year, CancellationToken ct);
}