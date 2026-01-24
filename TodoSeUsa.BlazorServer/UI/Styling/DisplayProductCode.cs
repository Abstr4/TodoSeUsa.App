namespace TodoSeUsa.BlazorServer.UI.Styling;

internal static class DisplayProductCode
{
    internal static string IdToProductCode(int productId)
    {
        return $"TSU-{productId}";
    }
}
