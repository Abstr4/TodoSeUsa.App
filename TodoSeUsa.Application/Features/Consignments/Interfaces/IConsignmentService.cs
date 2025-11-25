using TodoSeUsa.Application.Features.Consignments.DTOs;

namespace TodoSeUsa.Application.Features.Consignments.Interfaces;

public interface IConsignmentService
{
    Task<Result<PagedItems<ConsignmentDto>>> GetAllAsync(QueryRequest request, CancellationToken cancellationToken);

    Task<Result<int>> CreateAsync(CreateConsignmentDto createConsignmentDto, CancellationToken ct);

    Task<Result<ConsignmentDto>> GetByIdAsync(int consignmentId, CancellationToken cancellationToken);

    Task<Result<PagedItems<ConsignmentDto>>> GetByProviderIdAsync(QueryRequest request, int providerId, CancellationToken cancellationToken);

    Task<Result<bool>> EditByIdAsync(int consignmentId, EditConsignmentDto editConsignmentDto, CancellationToken cancellationToken);

    Task<Result<bool>> DeleteByIdAsync(int consignmentId, CancellationToken cancellationToken);
}
