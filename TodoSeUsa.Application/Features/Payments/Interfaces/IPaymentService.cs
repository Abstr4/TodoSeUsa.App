using TodoSeUsa.Application.Features.Payments.DTOs;

namespace TodoSeUsa.Application.Features.Payments.Interfaces;

public interface IPaymentService
{
    Task<Result<PagedItems<PaymentDto>>> GetPaymentsWithPagination(QueryItem request, CancellationToken ct);
}