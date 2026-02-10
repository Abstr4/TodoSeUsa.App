namespace TodoSeUsa.Application.Features.Consignments.DTOs;

public class CreateConsignmentDto
{
    public DateTime? DateIssued { get; set; }

    public string? Notes { get; set; }

    public int ConsignorId { get; set; }
}