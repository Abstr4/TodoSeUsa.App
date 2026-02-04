using TodoSeUsa.Application.Features.Payments.DTOs;
using TodoSeUsa.Application.Features.Sales.DTOs;

namespace TodoSeUsa.Application.Features.Sales.Interfaces;

public interface ISaleService
{
    Task<Result<int>> GetTotalCountAsync(int year, int month, CancellationToken ct);

    Task<Result<decimal>> GetRevenueAsync(int year, int month, CancellationToken ct);

    Task<Result<IReadOnlyList<SalePointDto>>> GetSalesAsync(int year, int month, CancellationToken ct);

    Task<Result<int>> GetYearlyTotalCountAsync(int year, CancellationToken ct);

    Task<Result<decimal>> GetYearlyRevenueAsync(int year, CancellationToken ct);

    Task<Result<IReadOnlyList<MonthlyCountPointDto>>> GetMonthlyCountAsync(int year, CancellationToken ct);

    Task<Result<IReadOnlyList<MonthlySalePointDto>>> GetMonthlySalesAsync(int year, CancellationToken ct);

    Task<Result<PagedItems<SaleDto>>> GetAllAsync(QueryRequest request, CancellationToken ct);

    Task<Result<DetailedSaleDto>> GetByIdAsync(int saleId, CancellationToken ct);

    Task<Result<PagedItems<SaleItemDto>>> GetItemsAsync(int saleId, QueryRequest request, CancellationToken ct);

    Task<Result<PagedItems<PaymentDto>>> GetPaymentsAsync(int saleId, QueryRequest request, CancellationToken ct);

    Task<Result> AddProductAsync(int saleId, int productId, CancellationToken ct);

    Task<Result> ReturnProductAsync(int saleId, int saleItemId, CancellationToken ct);

    Task<Result> RegisterPaymentAsync(int saleId, CreatePaymentDto paymentDto, CancellationToken ct);

    Task<Result> RefundPaymentAsync(int saleId, RefundPaymentDto refundPayment, CancellationToken ct);

    Task<Result<int>> CreateAsync(CreateSaleDto createSaleDto, CancellationToken ct);

    Task<Result> EditAsync(EditSaleDto editSaleDto, CancellationToken ct);

    Task<Result> CancelByIdAsync(int id, string? reason, CancellationToken ct);

    Task<Result> CancelByCodeAsync(string code, string? reason, CancellationToken ct);
}