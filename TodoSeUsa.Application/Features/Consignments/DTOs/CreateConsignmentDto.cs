namespace TodoSeUsa.Application.Features.Consignments.DTOs;

public class CreateConsignmentDto
{
    public DateTime DateIssued { get; set; } = DateTime.Now;

    public string? Notes { get; set; }

    public int ProviderId { get; set; }
}