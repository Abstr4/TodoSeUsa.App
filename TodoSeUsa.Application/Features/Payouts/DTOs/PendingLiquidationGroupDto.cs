namespace TodoSeUsa.Application.Features.Payouts.DTOs;

public sealed class PendingLiquidationGroupDto
{
    public int ConsignorId { get; init; }
    public string ConsignorName { get; init; } = string.Empty;

    public decimal TotalPending { get; init; }
    public int ItemsCount { get; init; }

    public List<PendingLiquidationItemDto> Items { get; init; } = [];
}
