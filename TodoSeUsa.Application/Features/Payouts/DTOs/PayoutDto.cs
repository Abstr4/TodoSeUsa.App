namespace TodoSeUsa.Application.Features.Payouts.DTOs;

public class PayoutDto
{
    public int Id { get; set; }
    public int ConsignorId { get; set; }
    public string ConsignorName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public int LinesCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<PayoutLineDto> Lines { get; set; } = [];
}