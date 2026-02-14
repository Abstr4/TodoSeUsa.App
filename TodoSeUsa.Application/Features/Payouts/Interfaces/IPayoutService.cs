using TodoSeUsa.Application.Features.Payouts.DTOs;

namespace TodoSeUsa.Application.Features.Payouts.Interfaces;

public interface IPayoutService
{
    Task<Result<PagedItems<PayoutDto>>> GetAllAsync(QueryRequest request, CancellationToken ct);

    Task<Result<int>> CreateAsync(CreatePayoutDto dto, CancellationToken ct);

    Task<Result<PayoutDto>> GetByIdAsync(int payoutId, CancellationToken ct);

    Task<Result> DeleteByIdAsync(int payoutId, CancellationToken ct);
}