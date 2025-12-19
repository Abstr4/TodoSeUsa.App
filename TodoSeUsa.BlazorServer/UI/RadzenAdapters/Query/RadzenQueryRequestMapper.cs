using Radzen;
using TodoSeUsa.Application.Common.Models;

namespace TodoSeUsa.BlazorServer.UI.RadzenAdapters.Query;

public static class RadzenQueryRequestMapper
{
    public static QueryRequest ConvertToQueryRequest(LoadDataArgs args, LogicalFilterOperator logicalFilterOperator)
    {
        return new QueryRequest
        {
            Skip = args.Skip ?? 0,
            Take = args.Top ?? 10,
            OrderBy = args.OrderBy,
            Filters = args.Filters?.Select(MapFilterDescriptor).ToList(),
            Sorts = args.Sorts?.Select(MapSortDescriptor).ToList(),
            LogicalFilterOperator = RadzenFilterMapper.Map(logicalFilterOperator)
        };
    }

    private static Application.Common.Models.FilterDescriptor MapFilterDescriptor(Radzen.FilterDescriptor f)
    {
        return new Application.Common.Models.FilterDescriptor
        {
            Property = f.Property,
            FilterProperty = f.FilterProperty,
            FilterValue = f.FilterValue,
            SecondFilterValue = f.SecondFilterValue,
            FilterOperator = RadzenFilterMapper.Map(f.FilterOperator),
            SecondFilterOperator = RadzenFilterMapper.Map(f.SecondFilterOperator),
            LogicalFilterOperator = RadzenFilterMapper.Map(f.LogicalFilterOperator)
        };
    }

    private static Application.Common.Models.SortDescriptor MapSortDescriptor(Radzen.SortDescriptor s)
    {
        return new Application.Common.Models.SortDescriptor
        {
            Property = s.Property,
            SortOrder = RadzenFilterMapper.Map(s.SortOrder ?? SortOrder.Ascending)
        };
    }
}