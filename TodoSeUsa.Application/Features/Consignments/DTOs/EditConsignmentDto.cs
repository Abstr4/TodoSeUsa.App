namespace TodoSeUsa.Application.Features.Consignments.DTOs;

public class EditConsignmentDto
{
    public int Id { get; set; }

    public DateTime DateIssued { get; set; }

    public string? Notes { get; set; }

    public int ConsignorId { get; set; }
}