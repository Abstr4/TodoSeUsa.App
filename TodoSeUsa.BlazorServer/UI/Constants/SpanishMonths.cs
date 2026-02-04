using TodoSeUsa.BlazorServer.UI.Models;

namespace TodoSeUsa.BlazorServer.UI.Constants;

public static class SpanishMonths
{
    public static readonly IReadOnlyList<Option<int>> All =
    [
        new() { Value = 1, Text = "Enero" },
        new() { Value = 2, Text = "Febrero" },
        new() { Value = 3, Text = "Marzo" },
        new() { Value = 4, Text = "Abril" },
        new() { Value = 5, Text = "Mayo" },
        new() { Value = 6, Text = "Junio" },
        new() { Value = 7, Text = "Julio" },
        new() { Value = 8, Text = "Agosto" },
        new() { Value = 9, Text = "Septiembre" },
        new() { Value = 10, Text = "Octubre" },
        new() { Value = 11, Text = "Noviembre" },
        new() { Value = 12, Text = "Diciembre" }
    ];
}