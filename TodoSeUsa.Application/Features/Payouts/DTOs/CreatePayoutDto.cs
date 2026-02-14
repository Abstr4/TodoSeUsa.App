namespace TodoSeUsa.Application.Features.Payouts.DTOs;

public class CreatePayoutDto
{
    public int ConsignorId { get; set; }
    public List<int> SaleItemIds { get; set; } = [];
}