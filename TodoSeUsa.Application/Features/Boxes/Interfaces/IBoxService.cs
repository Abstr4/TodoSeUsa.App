using TodoSeUsa.Application.Features.Boxes.DTOs;

namespace TodoSeUsa.Application.Features.Boxes.Interfaces;

public interface IBoxService
{
    Task<Result<PagedItems<BoxDto>>> GetAllAsync(QueryRequest request, CancellationToken ct);

    Task<Result<BoxDto>> GetByIdAsync(int boxId, CancellationToken ct);

    Task<Result> AddProductsToBoxAsync(int boxId, List<int> productIds, CancellationToken ct);

    Task<Result<int>> CreateAsync(CreateBoxDto createBoxDto, CancellationToken ct);

    Task<Result> EditBoxById(int boxId, EditBoxDto editBoxDto, CancellationToken ct);

    Task<Result> DeleteBoxById(int boxId, CancellationToken ct);
}