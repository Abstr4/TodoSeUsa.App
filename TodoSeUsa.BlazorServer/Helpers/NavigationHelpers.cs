using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace TodoSeUsa.BlazorServer.Helpers;

public static class NavigationHelpers
{
    public static async Task GoBackOrHomeAsync(this IJSRuntime js, NavigationManager nav, string fallbackUri = "/")
    {
        try
        {
            await js.InvokeVoidAsync("history.back");
        }
        catch
        {
            nav.NavigateTo(fallbackUri);
        }
    }
}