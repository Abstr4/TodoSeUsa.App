using TodoSeUsa.Application.Features.Boxes.DTOs;

namespace TodoSeUsa.Application.Features.Boxes.Interfaces;
public interface IBoxService
{
    Task<Result<bool>> CreateBoxAsync(CreateBoxDto boxDto, CancellationToken ct);

    Task<Result<PagedItems<BoxDto>>> GetBoxesWithPaginationAsync(QueryItem request, CancellationToken cancellationToken);

    Task<Result<BoxDto>> GetByIdAsync(int boxId, CancellationToken cancellationToken);

    Task<Result<bool>> EditBoxById(int boxId, EditBoxDto boxDto, CancellationToken cancellationToken);
}
