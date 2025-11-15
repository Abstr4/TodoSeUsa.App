using TodoSeUsa.Application.Features.Consignments.DTOs;

namespace TodoSeUsa.Application.Features.Consignments.Interfaces;

public interface IConsignmentService
{
    Task<Result<PagedItems<ConsignmentDto>>> GetAllAsync(QueryRequest request, CancellationToken cancellationToken);

    Task<Result<bool>> CreateConsignmentAsync(CreateConsignmentDto createConsignmentDto, CancellationToken ct);

    Task<Result<ConsignmentDto>> GetByIdAsync(int consignmentId, CancellationToken cancellationToken);

    Task<Result<bool>> EditConsignmentById(int consignmentId, EditConsignmentDto editConsignmentDto, CancellationToken cancellationToken);

    Task<Result<bool>> DeleteConsignmentById(int consignmentId, CancellationToken cancellationToken);
}
