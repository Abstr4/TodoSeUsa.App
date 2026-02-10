namespace TodoSeUsa.Application.Features.Consignors.DTOs;

public record CreateConsignorDto
{
    public decimal CommissionPercent { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string? EmailAddress { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Address { get; set; }
}