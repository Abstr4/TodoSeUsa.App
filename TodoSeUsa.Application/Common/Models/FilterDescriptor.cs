using TodoSeUsa.Application.Common.Querying.Enums;

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