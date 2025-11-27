using Radzen;

namespace TodoSeUsa.BlazorServer.Helpers;

public static class RadzenFilterMapper
{
    public static TodoSeUsa.Application.Common.Enums.FilterOperator Map(FilterOperator op)
    {
        return op switch
        {
            FilterOperator.Equals => TodoSeUsa.Application.Common.Enums.FilterOperator.Equals,
            FilterOperator.NotEquals => TodoSeUsa.Application.Common.Enums.FilterOperator.NotEquals,
            FilterOperator.LessThan => TodoSeUsa.Application.Common.Enums.FilterOperator.LessThan,
            FilterOperator.LessThanOrEquals => TodoSeUsa.Application.Common.Enums.FilterOperator.LessThanOrEqual,
            FilterOperator.GreaterThan => TodoSeUsa.Application.Common.Enums.FilterOperator.GreaterThan,
            FilterOperator.GreaterThanOrEquals => TodoSeUsa.Application.Common.Enums.FilterOperator.GreaterThanOrEqual,
            FilterOperator.Contains => TodoSeUsa.Application.Common.Enums.FilterOperator.Contains,
            FilterOperator.StartsWith => TodoSeUsa.Application.Common.Enums.FilterOperator.StartsWith,
            FilterOperator.EndsWith => TodoSeUsa.Application.Common.Enums.FilterOperator.EndsWith,
            FilterOperator.DoesNotContain => TodoSeUsa.Application.Common.Enums.FilterOperator.DoesNotContain,
            FilterOperator.In => TodoSeUsa.Application.Common.Enums.FilterOperator.In,
            FilterOperator.NotIn => TodoSeUsa.Application.Common.Enums.FilterOperator.NotIn,
            FilterOperator.IsNull => TodoSeUsa.Application.Common.Enums.FilterOperator.IsNull,
            FilterOperator.IsNotNull => TodoSeUsa.Application.Common.Enums.FilterOperator.IsNotNull,
            FilterOperator.IsEmpty => TodoSeUsa.Application.Common.Enums.FilterOperator.IsEmpty,
            FilterOperator.IsNotEmpty => TodoSeUsa.Application.Common.Enums.FilterOperator.IsNotEmpty,
            _ => TodoSeUsa.Application.Common.Enums.FilterOperator.Custom
        };
    }

    public static TodoSeUsa.Application.Common.Enums.LogicalFilterOperator Map(LogicalFilterOperator op)
    {
        return op switch
        {
            LogicalFilterOperator.And => TodoSeUsa.Application.Common.Enums.LogicalFilterOperator.And,
            LogicalFilterOperator.Or => TodoSeUsa.Application.Common.Enums.LogicalFilterOperator.Or,
            _ => TodoSeUsa.Application.Common.Enums.LogicalFilterOperator.And
        };
    }

    public static TodoSeUsa.Application.Common.Enums.SortOrder Map(SortOrder order)
    {
        return order switch
        {
            SortOrder.Ascending => TodoSeUsa.Application.Common.Enums.SortOrder.Ascending,
            SortOrder.Descending => TodoSeUsa.Application.Common.Enums.SortOrder.Descending,
            _ => TodoSeUsa.Application.Common.Enums.SortOrder.Ascending
        };
    }
}