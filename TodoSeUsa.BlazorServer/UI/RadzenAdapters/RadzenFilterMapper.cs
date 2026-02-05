using Radzen;

namespace TodoSeUsa.BlazorServer.UI.RadzenAdapters;

public static class RadzenFilterMapper
{
    public static Application.Common.Querying.Enums.FilterOperator Map(FilterOperator op)
    {
        return op switch
        {
            FilterOperator.Equals => Application.Common.Querying.Enums.FilterOperator.Equals,
            FilterOperator.NotEquals => Application.Common.Querying.Enums.FilterOperator.NotEquals,
            FilterOperator.LessThan => Application.Common.Querying.Enums.FilterOperator.LessThan,
            FilterOperator.LessThanOrEquals => Application.Common.Querying.Enums.FilterOperator.LessThanOrEqual,
            FilterOperator.GreaterThan => Application.Common.Querying.Enums.FilterOperator.GreaterThan,
            FilterOperator.GreaterThanOrEquals => Application.Common.Querying.Enums.FilterOperator.GreaterThanOrEqual,
            FilterOperator.Contains => Application.Common.Querying.Enums.FilterOperator.Contains,
            FilterOperator.StartsWith => Application.Common.Querying.Enums.FilterOperator.StartsWith,
            FilterOperator.EndsWith => Application.Common.Querying.Enums.FilterOperator.EndsWith,
            FilterOperator.DoesNotContain => Application.Common.Querying.Enums.FilterOperator.DoesNotContain,
            FilterOperator.In => Application.Common.Querying.Enums.FilterOperator.In,
            FilterOperator.NotIn => Application.Common.Querying.Enums.FilterOperator.NotIn,
            FilterOperator.IsNull => Application.Common.Querying.Enums.FilterOperator.IsNull,
            FilterOperator.IsNotNull => Application.Common.Querying.Enums.FilterOperator.IsNotNull,
            FilterOperator.IsEmpty => Application.Common.Querying.Enums.FilterOperator.IsEmpty,
            FilterOperator.IsNotEmpty => Application.Common.Querying.Enums.FilterOperator.IsNotEmpty,
            _ => Application.Common.Querying.Enums.FilterOperator.Custom
        };
    }

    public static Application.Common.Querying.Enums.LogicalFilterOperator Map(LogicalFilterOperator op)
    {
        return op switch
        {
            LogicalFilterOperator.And => Application.Common.Querying.Enums.LogicalFilterOperator.And,
            LogicalFilterOperator.Or => Application.Common.Querying.Enums.LogicalFilterOperator.Or,
            _ => Application.Common.Querying.Enums.LogicalFilterOperator.And
        };
    }

    public static Application.Common.Querying.Enums.SortOrder Map(Radzen.SortOrder order)
    {
        return order switch
        {
            Radzen.SortOrder.Ascending => Application.Common.Querying.Enums.SortOrder.Ascending,
            Radzen.SortOrder.Descending => Application.Common.Querying.Enums.SortOrder.Descending,
            _ => Application.Common.Querying.Enums.SortOrder.Ascending
        };
    }
}