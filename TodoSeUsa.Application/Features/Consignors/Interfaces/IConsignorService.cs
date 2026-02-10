using TodoSeUsa.Application.Features.Consignors.DTOs;

namespace TodoSeUsa.Application.Features.Consignors.Interfaces;

public interface IConsignorService
{
    Task<Result<PagedItems<ConsignorDto>>> GetAllAsync(QueryRequest request, CancellationToken ct);

    Task<Result<int>> CreateAsync(CreateConsignorDto createConsignorDto, CancellationToken ct);

    Task<Result<ConsignorDto>> GetByIdAsync(int consignorId, CancellationToken ct);

    Task<Result> EditByIdAsync(int consignorId, EditConsignorDto editConsignorDto, CancellationToken ct);

    Task<Result> DeleteByIdAsync(int consignorId, CancellationToken ct);
}