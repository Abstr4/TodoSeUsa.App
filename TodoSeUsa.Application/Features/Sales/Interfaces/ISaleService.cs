using TodoSeUsa.Application.Features.Sales.DTOs;

namespace TodoSeUsa.Application.Features.Sales.Interfaces;

public interface ISaleService
{
    Task<Result<PagedItems<SaleDto>>> GetSalesWithPagination(QueryItem request, CancellationToken cancellationToken);
}
