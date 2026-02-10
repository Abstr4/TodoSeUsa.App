namespace TodoSeUsa.Domain.Entities;

public class Person : BaseAuditableEntity
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public string? EmailAddress { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Address { get; set; }

    public string ContactInfo { get; set; } = string.Empty;

    public Client? Client { get; set; }

    public Consignor? Consignor { get; set; }
}