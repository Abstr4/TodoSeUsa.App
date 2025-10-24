using TodoSeUsa.Application.Features.Providers.DTOs;

namespace TodoSeUsa.Application.Features.Providers.Interfaces;

public interface IProviderService
{
    Task<Result<PagedItems<ProviderDto>>> GetProvidersWithPagination(QueryItem request, CancellationToken cancellationToken);
}
