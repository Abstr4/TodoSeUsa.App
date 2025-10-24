using TodoSeUsa.Application.Features.Consignments.DTOs;

namespace TodoSeUsa.Application.Features.Consignments.Interfaces;

public interface IConsignmentService
{
    Task<Result<PagedItems<ConsignmentDto>>> GetConsignmentsWithPagination(QueryItem request, CancellationToken cancellationToken);
}
