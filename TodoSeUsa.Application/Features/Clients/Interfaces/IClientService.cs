using TodoSeUsa.Application.Features.Clients.DTOs;

namespace TodoSeUsa.Application.Features.Clients.Interfaces;

public interface IClientService
{
    Task<Result<PagedItems<ClientDto>>> GetClientsWithPagination(QueryItem request, CancellationToken cancellationToken);
}
