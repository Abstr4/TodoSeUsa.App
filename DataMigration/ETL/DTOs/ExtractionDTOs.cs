namespace DataMigration.ETL.DTOs;

public class ExtractionDTOs
{
    public sealed class OldClientRow
    {
        public int ClientId { get; init; }
        public string FirstName { get; init; } = "";
        public string LastName { get; init; } = "";
        public string? Email { get; init; }
        public string? PhoneNumber { get; init; }
        public string? Address { get; init; }
        public bool Active { get; init; }
    }

    public sealed class OldBillRow
    {
        public int BillId { get; init; }
        public DateTime DateCreated { get; init; }
        public int ClientId { get; init; }
        public bool Closed { get; init; }
        public bool Active { get; init; }
    }

    public sealed class OldProductRow
    {
        public int ProductId { get; init; }
        public string Type { get; init; } = "";
        public string? Description { get; init; }
        public string? Condition { get; init; }
        public string State { get; init; } = "";
        public int Price { get; init; }
        public bool Sold { get; init; }
        public bool Active { get; init; }
        public bool Returned { get; init; }
        public bool MustReturn { get; init; }
        public int BillId { get; init; }
        public int? SaleId { get; init; }
    }
}
