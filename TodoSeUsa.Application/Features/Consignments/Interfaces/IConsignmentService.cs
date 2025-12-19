using TodoSeUsa.Application.Features.Consignments.DTOs;

namespace TodoSeUsa.Application.Features.Consignments.Interfaces;

public interface IConsignmentService
{
    Task<Result<PagedItems<ConsignmentDto>>> GetAllAsync(QueryRequest request, CancellationToken ct);

    Task<Result<int>> CreateAsync(CreateConsignmentDto createConsignmentDto, CancellationToken ct);

    Task<Result<ConsignmentDto>> GetByIdAsync(int consignmentId, CancellationToken ct);

    Task<Result<PagedItems<ConsignmentDto>>> GetByProviderIdAsync(QueryRequest request, int providerId, CancellationToken ct);

    Task<Result> EditByIdAsync(EditConsignmentDto editConsignmentDto, CancellationToken ct);

    Task<Result> DeleteByIdAsync(int consignmentId, CancellationToken ct);
}