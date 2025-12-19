using Radzen;

namespace TodoSeUsa.BlazorServer.UI.RadzenAdapters;

public static class RadzenDataGridDefaults
{
    public const int PageSize = 5;
    public const bool AllowSorting = true;
    public const bool AllowPaging = true;
    public const bool AllowFiltering = true;
    public const LogicalFilterOperator FilterOperator = LogicalFilterOperator.And;
}