using TodoSeUsa.Application.Features.Providers.DTOs;

namespace TodoSeUsa.Application.Features.Providers.Interfaces;

public interface IProviderService
{
    Task<Result<PagedItems<ProviderDto>>> GetAllAsync(QueryRequest request, CancellationToken cancellationToken);

    Task<Result<int>> CreateAsync(CreateProviderDto createProviderDto, CancellationToken cancellationToken);

    Task<Result<ProviderDto>> GetByIdAsync(int providerId, CancellationToken cancellationToken);

    Task<Result<bool>> EditByIdAsync(int providerId, EditProviderDto editProviderDto, CancellationToken cancellationToken);

    Task<Result<bool>> DeleteByIdAsync(int providerId, CancellationToken cancellationToken);
}
