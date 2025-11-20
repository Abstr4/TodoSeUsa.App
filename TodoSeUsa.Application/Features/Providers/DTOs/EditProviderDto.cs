namespace TodoSeUsa.Application.Features.Providers.DTOs;

public record EditProviderDto
{
    public decimal CommissionPercent { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string? EmailAddress { get; set; } = string.Empty;

    public string? PhoneNumber { get; set; } = string.Empty;

    public string? Address { get; set; } = string.Empty;
}
