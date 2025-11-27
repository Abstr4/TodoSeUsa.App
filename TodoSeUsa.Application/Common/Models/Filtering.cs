using TodoSeUsa.Application.Common.Enums;

namespace TodoSeUsa.Application.Common.Models;

public class FilterDescriptor
{
    public string Property { get; set; } = string.Empty;
    public string FilterProperty { get; set; } = string.Empty;
    public object? FilterValue { get; set; }
    public FilterOperator FilterOperator { get; set; }
    public object? SecondFilterValue { get; set; }
    public FilterOperator SecondFilterOperator { get; set; }
    public LogicalFilterOperator LogicalFilterOperator { get; set; }
}

public class SortDescriptor
{
    public string Property { get; set; } = string.Empty;
    public SortOrder SortOrder { get; set; }
}

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