using TodoSeUsa.Application.Features.Boxes.DTOs;

namespace TodoSeUsa.Application.Features.Boxes.Interfaces;

public interface IBoxService
{
    Task<Result<bool>> CreateAsync(CreateBoxDto createBoxDto, CancellationToken ct);

    Task<Result<PagedItems<BoxDto>>> GetAllAsync(QueryRequest request, CancellationToken cancellationToken);

    Task<Result<BoxDto>> GetByIdAsync(int boxId, CancellationToken cancellationToken);

    Task<Result<bool>> EditBoxById(int boxId, EditBoxDto editBoxDto, CancellationToken cancellationToken);

    Task<Result<bool>> DeleteBoxById(int boxId, CancellationToken cancellationToken);
}
