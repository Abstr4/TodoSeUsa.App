using TodoSeUsa.Application.Features.Boxes.DTOs;

namespace TodoSeUsa.Application.Features.Boxes.Interfaces;

public interface IBoxService
{
    Task<Result<bool>> CreateAsync(CreateBoxDto createBoxDto, CancellationToken ct);

    Task<Result<PagedItems<BoxDto>>> GetAllAsync(QueryRequest request, CancellationToken ct);

    Task<Result<BoxDto>> GetByIdAsync(int boxId, CancellationToken ct);

    Task<Result<bool>> EditBoxById(int boxId, EditBoxDto editBoxDto, CancellationToken ct);

    Task<Result<bool>> DeleteBoxById(int boxId, CancellationToken ct);
}