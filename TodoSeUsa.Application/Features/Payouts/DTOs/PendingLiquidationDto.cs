namespace TodoSeUsa.Application.Features.Payouts.DTOs;

public sealed class PendingLiquidationItemDto
{
    public int SaleItemId { get; init; }
    public int ProductId { get; init; }

    public string Description { get; init; } = string.Empty;

    public decimal Remaining { get; init; }
    public DateTime SoldAt { get; init; }

    public bool HasPartialPayments { get; init; }
}
