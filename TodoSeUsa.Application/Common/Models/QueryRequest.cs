using TodoSeUsa.Application.Common.Querying.Enums;

namespace TodoSeUsa.Application.Common.Models;

public class QueryRequest
{
    public int Skip { get; set; }
    public int Take { get; set; }
    public string? OrderBy { get; set; }
    public string? Filter { get; set; }
    public List<FilterDescriptor>? Filters { get; set; }
    public List<SortDescriptor>? Sorts { get; set; }
    public LogicalFilterOperator LogicalFilterOperator { get; set; } = LogicalFilterOperator.And;
}