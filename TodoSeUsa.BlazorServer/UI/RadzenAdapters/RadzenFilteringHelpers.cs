using Radzen;

namespace TodoSeUsa.BlazorServer.UI.RadzenAdapters;

public static class RadzenFilteringHelpers
{
    public static IEnumerable<object> LogicalOperators =>
    [
        new { Text = "Y", Value = LogicalFilterOperator.And },
        new { Text = "O", Value = LogicalFilterOperator.Or }
    ];

    public static IEnumerable<object> CaseSensitivities =>
    [
        new { Text = "Distingue mayúsculas/minúsculas", Value = FilterCaseSensitivity.Default },
        new { Text = "No distingue mayúsculas/minúsculas", Value = FilterCaseSensitivity.CaseInsensitive }
    ];
}