using Radzen;
using TodoSeUsa.Application.Common.Models;

namespace TodoSeUsa.BlazorServer.Helpers;

public static class QueryRequestHelper
{
    public static QueryRequest ConvertToQueryRequest(LoadDataArgs args)
    {
        return new QueryRequest
        {
            Skip = args.Skip ?? 0,
            Take = args.Top ?? 10,
            OrderBy = args.OrderBy,
            Filters = args.Filters?.Select(MapFilterDescriptor).ToList(),
            Sorts = args.Sorts?.Select(MapSortDescriptor).ToList()
        };
    }

    private static TodoSeUsa.Application.Common.Models.FilterDescriptor MapFilterDescriptor(Radzen.FilterDescriptor f)
    {
        return new TodoSeUsa.Application.Common.Models.FilterDescriptor
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

    private static TodoSeUsa.Application.Common.Models.SortDescriptor MapSortDescriptor(Radzen.SortDescriptor s)
    {
        return new TodoSeUsa.Application.Common.Models.SortDescriptor
        {
            Property = s.Property,
            SortOrder = RadzenFilterMapper.Map(s.SortOrder ?? SortOrder.Ascending)
        };
    }
}
