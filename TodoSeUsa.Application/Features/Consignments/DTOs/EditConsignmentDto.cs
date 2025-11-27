namespace TodoSeUsa.Application.Features.Consignments.DTOs;

public class EditConsignmentDto
{
    public DateTime DateIssued { get; set; }

    public string? Notes { get; set; }

    public int ProviderId { get; set; }
}