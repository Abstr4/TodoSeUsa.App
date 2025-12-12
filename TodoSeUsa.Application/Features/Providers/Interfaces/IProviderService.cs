using TodoSeUsa.Application.Features.Providers.DTOs;

namespace TodoSeUsa.Application.Features.Providers.Interfaces;

public interface IProviderService
{
    Task<Result<PagedItems<ProviderDto>>> GetAllAsync(QueryRequest request, CancellationToken ct);

    Task<Result<int>> CreateAsync(CreateProviderDto createProviderDto, CancellationToken ct);

    Task<Result<ProviderDto>> GetByIdAsync(int providerId, CancellationToken ct);

    Task<Result> EditByIdAsync(int providerId, EditProviderDto editProviderDto, CancellationToken ct);

    Task<Result> DeleteByIdAsync(int providerId, CancellationToken ct);
}